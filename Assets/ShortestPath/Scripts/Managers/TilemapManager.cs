
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager instance;

    #region TileData

    private Dictionary<TileBase, TileData> m_tileDataDict;

    [SerializeField]
    private Transform m_deluvianNexusHub, m_fireSwatheNexusHub, m_stormNexusHub;

    #endregion

    //  TODO: dynamically load this for each level
    [SerializeField]
    private Tilemap m_map;

    [SerializeField]
    private Destination m_destination;

    private LayerMask m_towerMask;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (this != instance) {
            Debug.Log("Warning! You have multiple TilemapManagers simultaneously. This may result in unexpected behavior.");
        }

        m_tileDataDict = GameDB.instance.GetTileDataDict();

        if (m_map == null) {
            Debug.Log("No Tilemap assigned. Shortest paths cannot be calculated.");
        }

        m_towerMask = 1 << LayerMask.NameToLayer("Tower"); // weird layerMask bit magic
    }

    public List<Vector2> CalculatePath(List<TileData.WalkType> canWalkOn, Vector2 startPos, bool movesDiagonal) {

        // canWalkOn : the types of terrain the given oncomer can walk through
        int[,] mapArray = ConvertMapToArray(canWalkOn);

        // get starting position in array terms

        Vector3Int startGridPos = m_map.WorldToCell(startPos);
        Vector2Int startArrPos = new Vector2Int(startGridPos.x - (m_map.cellBounds.min.x), (m_map.cellBounds.max.y - 1) - startGridPos.y);

        Vector3Int endGridPos = m_map.WorldToCell(m_destination.transform.position);
        Vector2Int endArrPos = new Vector2Int(endGridPos.x - (m_map.cellBounds.min.x), (m_map.cellBounds.max.y - 1) - endGridPos.y);

        List<Vector2Int> shortestWaypoints = null;
        List<Vector2> currWaypoints = new List<Vector2>();

        // TODO: Change this to A* shortest path algorithm for improved efficiency
        /*CalculatePathHelper(
            mapArray,
            startArrPos,
            new Vector2Int(endArrPos.x, endArrPos.y),
            ref currWaypoints,
            ref shortestWaypoints,
            movesDiagonal
            );
        */

        shortestWaypoints = AStarPath(startArrPos, endArrPos, mapArray, movesDiagonal);

        // convert wayPoints from arr to Tilemap

        List<Vector2> adjustedWaypoints = ConvertArrayPointsToMap(shortestWaypoints);

        return adjustedWaypoints;
    }

    private List<Vector2Int> AStarPath(Vector2Int start, Vector2Int end, int[,] mapArray, bool movesDiagonal) {
        List<Vector2Int> finalPath = new List<Vector2Int>();

        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        // This is usually implemented as a min-heap or priority queue rather than a hash-set.
        PriorityQueue<Vector2Int, float> openSet = new PriorityQueue<Vector2Int, float>();
        List<Vector2Int> openSetKeys = new List<Vector2Int>();
        openSet.Enqueue(start, 0);
        openSetKeys.Add(start);

        // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
        // to n currently known.
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        gScore[start] = 0;

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        // how cheap a path could be from start to finish if it goes through n.
        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();
        fScore[start] = Heuristic(start, start);

        while (openSet.Count > 0) {
            Vector2Int current = openSet.Dequeue();
            openSetKeys.Remove(current);
            if (current == end) {
                finalPath = ReconstructPath(cameFrom, current);
                finalPath.Reverse();
                return finalPath;
            }

            List<Vector2Int> connectedRoads = GetValidConnections(mapArray, current, movesDiagonal);
            for (int r = 0; r < connectedRoads.Count; r++) {
                Vector2Int neighbor = connectedRoads[r];
                if (!gScore.ContainsKey(neighbor)) {
                    gScore.Add(neighbor, int.MaxValue);
                }
                if (!fScore.ContainsKey(neighbor)) {
                    fScore.Add(neighbor, int.MaxValue);
                }
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // tentative_gScore is the distance from start to the neighbor through current
                float tentativeGScore = gScore[current] + Vector2.Distance(current, neighbor);
                if (tentativeGScore < gScore[neighbor]) {
                    // This path to neighbor is better than any previous one. Record it!
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    float hNeighbor = Heuristic(start, neighbor);
                    fScore[neighbor] = tentativeGScore + hNeighbor;
                    if (!openSetKeys.Contains(neighbor)) {
                        openSet.Enqueue(neighbor, hNeighbor);
                        openSetKeys.Add(neighbor);
                    }
                }
            }
        }

        return finalPath;
    }

    private float Heuristic(Vector2Int start, Vector2Int curr) {
        // returns a lower value for closer to destination
        return Vector2.Distance(start, curr);
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current) {
        List<Vector2Int> totalPath = new List<Vector2Int>();
        totalPath.Add(current);
        Debug.Log("[A*] Path reconstruct start with " + current);

        while (cameFrom.Keys.Contains(current)) {
            current = cameFrom[current];
            totalPath.Add(current);
            Debug.Log("[A*] Adding " + current);
        }

        Debug.Log("[A*] Path reconstructing finished with length " + totalPath.Count);

        return totalPath;
    }

    private List<Vector2Int> GetValidConnections(int[,] mapArray, Vector2Int queryPos, bool movesDiagonal) {
        List<Vector2Int> connections = new List<Vector2Int>();

        // Recurse on the bottom cell
        Vector2Int bottom = new Vector2Int(queryPos.x, queryPos.y + 1);
        if (CanMove(bottom.y, bottom.x, mapArray)) {
            connections.Add(bottom);
        }

        // Recurse on the top cell
        Vector2Int top = new Vector2Int(queryPos.x, queryPos.y - 1);
        if (CanMove(top.y, top.x, mapArray)) {
            connections.Add(top);
        }

        // Recurse on the left cell
        Vector2Int left = new Vector2Int(queryPos.x - 1, queryPos.y);
        if (CanMove(left.y, left.x, mapArray)) {
            connections.Add(left);
        }

        // Recurse on the right cell
        Vector2Int right = new Vector2Int(queryPos.x + 1, queryPos.y);
        if (CanMove(right.y, right.x, mapArray)) {
            connections.Add(right);
        }


        if (movesDiagonal) {
            // Recurse on the bottom-left cell
            Vector2Int bl = new Vector2Int(queryPos.x - 1, queryPos.y + 1);
            if (CanMove(bl.y, bl.x, mapArray)) {
                connections.Add(bl);
            }

            // Recurse on the bottom-right cell
            Vector2Int br = new Vector2Int(queryPos.x + 1, queryPos.y + 1);
            if (CanMove(br.y, br.x, mapArray)) {
                connections.Add(br);
            }

            // Recurse on the top-left cell
            Vector2Int tl = new Vector2Int(queryPos.x - 1, queryPos.y - 1);
            if (CanMove(tl.y, tl.x, mapArray)) {
                connections.Add(tl);
            }

            // Recurse on the top-right cell
            Vector2Int tr = new Vector2Int(queryPos.x + 1, queryPos.y - 1);
            if (CanMove(tr.y, tr.x, mapArray)) {
                connections.Add(tr);
            }
        }

        return connections;
    }

    // returns true if end path was reached (or could be on shorter path), somewhere down the recursion, false otherwise
    private bool CalculatePathHelper(int[,] mapArray, Vector2Int startPos, Vector2Int endPos, ref List<Vector2> currWaypoints, ref List<Vector2> shortestWaypoints, bool movesDiagonal) {
        if (startPos.x == endPos.x && startPos.y == endPos.y) {
            // base case: found a path to the end
            if (shortestWaypoints == null) {
                shortestWaypoints = new List<Vector2>();
                currWaypoints.Add(startPos);
                foreach (Vector2 waypoint in currWaypoints) {
                    shortestWaypoints.Add(waypoint);
                }
            }
            else if (currWaypoints.Count < shortestWaypoints.Count - 2) {
                shortestWaypoints.Clear();
                currWaypoints.Add(startPos);
                foreach (Vector2 waypoint in currWaypoints) {
                    shortestWaypoints.Add(waypoint);
                }
            }
            // backtrack
            currWaypoints.Remove(startPos);
            return true;
        }
        else {
            // mark cell as visited
            mapArray[startPos.y, startPos.x] = 2;
            currWaypoints.Add(startPos);

            if (shortestWaypoints != null && currWaypoints.Count >= shortestWaypoints.Count) {
                currWaypoints.Remove(startPos);
                mapArray[startPos.y, startPos.x] = 0;
                return true;
            }

            bool foundPath = false;

            // Recurse on the bottom cell
            if (CanMove(startPos.y + 1, startPos.x, mapArray)) {
                if (CalculatePathHelper(
                    mapArray,
                    new Vector2Int(startPos.x, startPos.y + 1),
                    endPos,
                    ref currWaypoints,
                    ref shortestWaypoints,
                    movesDiagonal
                    )) {
                    foundPath = true;
                }
            }

            if ((startPos - endPos).x > 0) {
                // Recurse on the left cell
                if (CanMove(startPos.y, startPos.x - 1, mapArray)) {
                    if (CalculatePathHelper(
                        mapArray,
                        new Vector2Int(startPos.x - 1, startPos.y),
                        endPos,
                        ref currWaypoints,
                        ref shortestWaypoints,
                        movesDiagonal
                        )) {
                        foundPath = true;
                    }
                }

                // Recurse on the right cell
                if (CanMove(startPos.y, startPos.x + 1, mapArray)) {
                    if (CalculatePathHelper(
                       mapArray,
                       new Vector2Int(startPos.x + 1, startPos.y),
                       endPos,
                       ref currWaypoints,
                       ref shortestWaypoints,
                       movesDiagonal
                       )) {
                        foundPath = true;
                    }
                }
            }
            else {
                // Recurse on the right cell
                if (CanMove(startPos.y, startPos.x + 1, mapArray)) {
                    if (CalculatePathHelper(
                       mapArray,
                       new Vector2Int(startPos.x + 1, startPos.y),
                       endPos,
                       ref currWaypoints,
                       ref shortestWaypoints,
                       movesDiagonal
                       )) {
                        foundPath = true;
                    }
                }

                // Recurse on the left cell
                if (CanMove(startPos.y, startPos.x - 1, mapArray)) {
                    if (CalculatePathHelper(
                        mapArray,
                        new Vector2Int(startPos.x - 1, startPos.y),
                        endPos,
                        ref currWaypoints,
                        ref shortestWaypoints,
                        movesDiagonal
                        )) {
                        foundPath = true;
                    }
                }
            }

            // Recurse on the top cell
            if (CanMove(startPos.y - 1, startPos.x, mapArray)) {
                if (CalculatePathHelper(
                   mapArray,
                   new Vector2Int(startPos.x, startPos.y - 1),
                   endPos,
                   ref currWaypoints,
                   ref shortestWaypoints,
                   movesDiagonal
                   )) {
                    foundPath = true;
                }
            }

            #region Diagonal
            if (movesDiagonal) {

                if ((startPos - endPos).x > 0) {
                    // Recurse on the bottom-right cell
                    if (CanMove(startPos.y + 1, startPos.x + 1, mapArray)) {
                        if (CalculatePathHelper(
                            mapArray,
                            new Vector2Int(startPos.x + 1, startPos.y + 1),
                            endPos,
                            ref currWaypoints,
                            ref shortestWaypoints,
                            movesDiagonal
                            )) {
                            foundPath = true;
                        }
                    }

                    // Recurse on the bottom-left cell
                    if (CanMove(startPos.y + 1, startPos.x - 1, mapArray)) {
                        if (CalculatePathHelper(
                           mapArray,
                           new Vector2Int(startPos.x - 1, startPos.y + 1),
                           endPos,
                           ref currWaypoints,
                           ref shortestWaypoints,
                           movesDiagonal
                           )) {
                            foundPath = true;
                        }
                    }
                }
                else {
                    // Recurse on the bottom-left cell
                    if (CanMove(startPos.y + 1, startPos.x - 1, mapArray)) {
                        if (CalculatePathHelper(
                           mapArray,
                           new Vector2Int(startPos.x - 1, startPos.y + 1),
                           endPos,
                           ref currWaypoints,
                           ref shortestWaypoints,
                           movesDiagonal
                           )) {
                            foundPath = true;
                        }
                    }

                    // Recurse on the bottom-right cell
                    if (CanMove(startPos.y + 1, startPos.x + 1, mapArray)) {
                        if (CalculatePathHelper(
                            mapArray,
                            new Vector2Int(startPos.x + 1, startPos.y + 1),
                            endPos,
                            ref currWaypoints,
                            ref shortestWaypoints,
                            movesDiagonal
                            )) {
                            foundPath = true;
                        }
                    }
                }

                // Recurse on the top-right cell
                if (CanMove(startPos.y - 1, startPos.x + 1, mapArray)) {
                    if (CalculatePathHelper(
                       mapArray,
                       new Vector2Int(startPos.x + 1, startPos.y - 1),
                       endPos,
                       ref currWaypoints,
                       ref shortestWaypoints,
                       movesDiagonal
                       )) {
                        foundPath = true;
                    }
                }

                // Recurse on the top-left cell
                if (CanMove(startPos.y - 1, startPos.x - 1, mapArray)) {
                    if (CalculatePathHelper(
                        mapArray,
                        new Vector2Int(startPos.x - 1, startPos.y - 1),
                        endPos,
                        ref currWaypoints,
                        ref shortestWaypoints,
                        movesDiagonal
                        )) {
                        foundPath = true;
                    }
                }
            }

            #endregion

            // Backtrack
            if (foundPath) {
                mapArray[startPos.y, startPos.x] = 0;
            }
            else {
                // path cannot be found down this cell, so treat as impassable
                mapArray[startPos.y, startPos.x] = 1;
            }
            currWaypoints.Remove(startPos);

            return foundPath;
        }
    }

    private bool CanMove(int y, int x, int[,] mapArray) {
        try {
            return mapArray[y, x] == 0;
        }
        catch (Exception e) {
            return false;
        }
    }

    private int[,] ConvertMapToArray(List<TileData.WalkType> canWalkOn) {

        m_map.CompressBounds(); // avoids tricky debugging issue with lopsided maps

        int mapX = m_map.size.x;
        int mapY = m_map.size.y;

        int[,] arr = new int[mapY, mapX];

        // for each tile in the tilemap, in the array place a:
        // 0 for tiles the given Oncomer can move through
        // 2 for tiles the given Oncomer cannot move through
        // (1 is reserved for marking a cell as "visited" in the shortest path algorithm)

        int rowIndex = 0;
        int colIndex = 0;
        for (int row = m_map.cellBounds.max.y - 1; row > m_map.cellBounds.min.y - 1; --row) {
            for (int col = m_map.cellBounds.min.x; col < m_map.cellBounds.max.x; ++col) {

                Vector3Int gridPos = new Vector3Int(col, row, 0);
                TileBase currTile = m_map.GetTile(gridPos);

                if (currTile == null) {
                    arr[rowIndex, colIndex] = 2;
                    colIndex++;
                    continue;
                }

                if (!m_tileDataDict.ContainsKey(currTile)) {
                    arr[rowIndex, colIndex] = 2;
                    colIndex++;
                    continue;
                }

                TileData.WalkType walkType = m_tileDataDict[currTile].GetWalkType();

                if (canWalkOn.Contains(walkType)) {
                    // set a 0
                    arr[rowIndex, colIndex] = 0;
                }
                else {
                    // set a 2
                    arr[rowIndex, colIndex] = 2;
                }
                colIndex++;
            }
            colIndex = 0;
            rowIndex++;
        }

        // DebugArray(mapX, mapY, arr);        

        return arr;
    }

    private void DebugArray(int mapX, int mapY, int[,] arr) {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < mapY; i++) {
            for (int j = 0; j < mapX; j++) {
                sb.Append(arr[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    private List<Vector2> ConvertArrayPointsToMap(List<Vector2Int> waypoints) {
        List<Vector2> convertedPoints = new List<Vector2>();

        int adjustX = -m_map.cellBounds.min.x;
        int adjustY = m_map.cellBounds.max.y - 1;

        if (waypoints == null) {
            return null;
        }

        foreach (Vector2 point in waypoints) {
            Vector2 correctedPoint = new Vector2(point.x - adjustX, -(point.y - adjustY));

            convertedPoints.Add(correctedPoint);
        }

        return convertedPoints;
    }

    public float GetBound(string side) {
        if (side == "left") {
            return (m_map.localBounds.center - m_map.localBounds.extents).x;
        }
        else {
            return (m_map.localBounds.center + m_map.localBounds.extents).x;
        }
    }

    public float GetRandomX() {
        int margin = 1;
        float min = (m_map.localBounds.center - m_map.localBounds.extents).x + margin;
        float max = (m_map.localBounds.center + m_map.localBounds.extents).x - margin;
        return UnityEngine.Random.Range(min, max);
    }

    public float GetRandomY() {
        int margin = 3;
        float min = (m_map.localBounds.center - m_map.localBounds.extents).y + margin;
        float max = (m_map.localBounds.center + m_map.localBounds.extents).y - margin;
        float givenY = UnityEngine.Random.Range(min, max);
        return givenY;
    }

    public Transform GetNexusHubTransform(Nexus.Type type) {
        switch (type) {
            case Nexus.Type.Deluvian:
                return m_deluvianNexusHub;
            case Nexus.Type.FireSwathe:
                return m_fireSwatheNexusHub;
            case Nexus.Type.Storm:
                return m_stormNexusHub;
            default: return m_deluvianNexusHub;
        }
    }

    public bool IsValidPlacement(Vector3 towerPos) {
        Vector3Int towerGridPos = m_map.WorldToCell(towerPos);
        TileBase currTile = m_map.GetTile(towerGridPos);

        // check if cell allows tower placement
        bool canPlace = m_tileDataDict[currTile].GetTowerPlaceable();
        if (!canPlace) {
            return false;
        }

        // check if another tower already exists
        return !TowerExists(towerPos);
    }

    private bool TowerExists(Vector3 gridPos) {
        // raycast for a tower
        Collider2D collider = Physics2D.OverlapPoint(gridPos, m_towerMask);

        // return true if a tower was detected, false otherwise
        return collider != null;
    }

    #region Grid Loading

    public void LoadGridFromArray(TextAsset inputTA) {
        if (inputTA == null) {
            Debug.Log("Failed to load grid from array: input text is null");
            return;
        }

        m_map.ClearAllTiles();
        m_map.CompressBounds();

        List<string> inputStrings = TextIO.TextAssetToList(inputTA, '|');

        LoadStringsIntoGrid(inputStrings);

        Debug.Log("successfully loaded grid from array");
    }

    private void LoadStringsIntoGrid(List<string> inputStrings) {
        List<TileData> tileDataList = GameDB.instance.GetTileDataList();

        // first two strings are mapX, mapY, adjustX, adjustY
        int mapX = int.Parse(inputStrings[0]);
        int mapY = int.Parse(inputStrings[1]);
        int adjustX = int.Parse(inputStrings[2]);
        int adjustY = int.Parse(inputStrings[3]);

        // load in
        int inputListIndex = 4;
        int rowIndex = 0;
        int colIndex = 0;
        for (int row = 0; row < mapY; ++row) {
            for (int col = 0; col < mapX; ++col) {
                // get tile and data
                string dataTileStr = inputStrings[inputListIndex];
                inputListIndex++; // increment for future loops
                int breakIndex = dataTileStr.IndexOf('.');
                int dataIndex = int.Parse(dataTileStr.Substring(0, breakIndex));
                int tileIndex = int.Parse(dataTileStr.Substring(breakIndex + 1));

                if (dataIndex == -1 || tileIndex == -1) {
                    // tile is empty; leave empty
                    continue;
                }

                TileBase tile = tileDataList[dataIndex].Tiles[tileIndex];

                // calculate adjusted position on grid
                Vector3Int gridPos = new Vector3Int(col - adjustX, -(row - adjustY), 0);

                // set tile
                m_map.SetTile(gridPos, tile);

                colIndex++;
            }
            colIndex = 0;
            rowIndex++;
        }

        return;
    }

    #endregion // Grid Loading
}