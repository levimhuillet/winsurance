using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
    [SerializeField]
    private GameDB m_gameDB;
    [SerializeField]
    private Tilemap m_map;

    private List<TileData> m_tileDataList;
    private Dictionary<TileBase, TileData> m_tileDataDict;

    [SerializeField]
    private string m_basePath;

    [SerializeField]
    private string m_outputFileName;

    [SerializeField]
    private TextAsset m_inputTxt;

    private const string INVALID_TILE_KEY = "-1.0"; // key for invalid tile

    [ContextMenu("Convert Grid to Array")]
    private void ConvertGridToArray() {
        Debug.Log("Beginning Conversion from Grid to Array...");
        m_tileDataList = m_gameDB.GetTileDataList();
        m_tileDataDict = m_gameDB.ConstructTileDataDict();

        m_map.CompressBounds();
        int mapX = m_map.size.x;
        int mapY = m_map.size.y;
        int adjustX = -m_map.cellBounds.min.x;
        int adjustY = m_map.cellBounds.max.y - 1;
        string mapArrayStr = mapX + "|" + mapY + "|" + adjustX + "|" + adjustY + "|\n";
        mapArrayStr += GridToArray();

        TextIO.WriteString(m_basePath + m_outputFileName + ".txt", mapArrayStr);

        Debug.Log("Conversion was successful! Output file: " + m_outputFileName);
    }

    [ContextMenu("Load Grid from Array")]
    private void LoadGridFromArray() {
        if (m_inputTxt == null) {
            Debug.Log("Failed to load grid from array: input text is null");
            return;
        }

        m_map.ClearAllTiles();
        m_map.CompressBounds();

        m_tileDataList = m_gameDB.GetTileDataList();
        m_tileDataDict = m_gameDB.ConstructTileDataDict();

        Debug.Log("Input file: " + m_inputTxt.name + ". Loading Grid from Array...");

        List<string> inputStrings = TextIO.TextAssetToList(m_inputTxt, '|');

        LoadStringsIntoGrid(inputStrings);

        Debug.Log("Load was successful!");
    }

    #region Helper Methods

    private string GridToArray() {
        m_map.CompressBounds(); // avoids tricky debugging issue with lopsided maps

        int mapX = m_map.size.x;
        int mapY = m_map.size.y;

        string[,] arr = new string[mapY, mapX];

        // for each tile in the tilemap, in the array place a key (e.g. "2.1")
        // that corresponds to the entry in the TileData list and index of the exact tile

        int rowIndex = 0;
        int colIndex = 0;
        for (int row = m_map.cellBounds.max.y - 1; row > m_map.cellBounds.min.y - 1; --row) {
            for (int col = m_map.cellBounds.min.x; col < m_map.cellBounds.max.x; ++col) {

                Vector3Int gridPos = new Vector3Int(col, row, 0);
                TileBase currTile = m_map.GetTile(gridPos);

                if (currTile == null) {
                    arr[rowIndex, colIndex] = INVALID_TILE_KEY;
                    colIndex++;
                    continue;
                }

                if (!m_tileDataDict.ContainsKey(currTile)) {
                    arr[rowIndex, colIndex] = INVALID_TILE_KEY;
                    colIndex++;
                    continue;
                }

                int dataIndex = -1; // index of data within TileData List
                int tileIndex = -1; // tile index within data

                // get tiledata index
                int tileDataListSize = m_tileDataList.Count;
                for (int i = 0; i < tileDataListSize; ++i) {
                    TileData data = m_tileDataList[i];
                    int arrIndex = ArrIndexOf(data.Tiles, currTile);
                    if (arrIndex != -1) {
                        // found tile
                        dataIndex = i;
                        tileIndex = arrIndex;
                        break;
                    }
                }

                if (dataIndex == -1 || tileIndex == -1) {
                    Debug.Log("Error: either missing data or tile information at (row: " + row + ", col: " + col + ")");
                }

                arr[rowIndex, colIndex] = dataIndex + "." + tileIndex;

                colIndex++;
            }
            colIndex = 0;
            rowIndex++;
        }

        // convert arr to string
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < mapY; i++) {
            for (int j = 0; j < mapX; j++) {
                sb.Append(arr[i, j]);
                sb.Append('|');
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private int ArrIndexOf(TileBase[] arr, TileBase tile) {
        int arrSize = arr.Length;
        for (int i = 0; i < arrSize; ++i) {
            if (arr[i] == tile) {
                return i;
            }
        }

        return -1;
    }

    private void LoadStringsIntoGrid(List<string> inputStrings) {
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

                TileBase tile = m_tileDataList[dataIndex].Tiles[tileIndex];

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

    #endregion

}
