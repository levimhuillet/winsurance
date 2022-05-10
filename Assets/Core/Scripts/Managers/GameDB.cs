using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameDB : MonoBehaviour {
    public static GameDB instance;

    private Dictionary<string, AudioData> m_audioMap;
    private Dictionary<Nexus.Type, NexusData> m_nexusMap;
    private Dictionary<Oncomer.Type, OncomerData> m_oncomerMap;
    private Dictionary<Tower.Type, TowerData> m_towerMap;
    private Dictionary<string, LevelData> m_levelMap;

    #region Editor

    [SerializeField]
    private AudioData[] m_audioData;
    [SerializeField]
    private NexusData[] m_nexusData;
    [SerializeField]
    private OncomerData[] m_oncomerData;
    [SerializeField]
    private TowerData[] m_towerData;
    [SerializeField]
    private LevelData[] m_levelData;
    [SerializeField]
    private Color m_deluvianNexusColor;
    [SerializeField]
    private Color m_fireSwatheNexusColor;
    [SerializeField]
    private Color m_stormNexusColor;

    [SerializeField]
    private List<TileData> m_tileDataList;

    private Dictionary<TileBase, TileData> m_tileDataDict;

    #endregion // Editor

    public AudioData GetAudioData(string id) {
        // initialize the map if it does not exist
        if (instance.m_audioMap == null) {
            instance.m_audioMap = new Dictionary<string, AudioData>();
            foreach (AudioData data in instance.m_audioData) {
                instance.m_audioMap.Add(data.ID, data);
            }
        }
        if (instance.m_audioMap.ContainsKey(id)) {
            return instance.m_audioMap[id];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Audio " +
                "with id `{0}' is in the database", id
            ));
        }
    }

    public NexusData GetNexusData(Nexus.Type type) {
        // initialize the map if it does not exist
        if (instance.m_nexusMap == null) {
            instance.m_nexusMap = new Dictionary<Nexus.Type, NexusData>();
            foreach (NexusData data in instance.m_nexusData) {
                instance.m_nexusMap.Add(data.Type, data);
            }
        }
        if (instance.m_nexusMap.ContainsKey(type)) {
            return instance.m_nexusMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Nexus " +
                "with type `{0}' is in the database", type
            ));
        }
    }

    public OncomerData GetOncomerData(Oncomer.Type type) {
        // initialize the map if it does not exist
        if (instance.m_oncomerMap == null) {
            instance.m_oncomerMap = new Dictionary<Oncomer.Type, OncomerData>();
            foreach (OncomerData data in instance.m_oncomerData) {
                instance.m_oncomerMap.Add(data.Type, data);
            }
        }
        if (instance.m_oncomerMap.ContainsKey(type)) {
            return instance.m_oncomerMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Oncomer " +
                "with type `{0}' is in the database", type
            ));
        }
    }

    public TowerData GetTowerData(Tower.Type type) {
        // initialize the map if it does not exist
        if (instance.m_towerMap == null) {
            instance.m_towerMap = new Dictionary<Tower.Type, TowerData>();
            foreach (TowerData data in instance.m_towerData) {
                instance.m_towerMap.Add(data.Type, data);
            }
        }
        if (instance.m_towerMap.ContainsKey(type)) {
            return instance.m_towerMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Tower " +
                "with type `{0}' is in the database", type
            ));
        }
    }

    public Color GetNexusColor(Nexus.Type type) {
        switch (type) {
            case Nexus.Type.Deluvian:
                return m_deluvianNexusColor;
            case Nexus.Type.FireSwathe:
                return m_fireSwatheNexusColor;
            case Nexus.Type.Storm:
                return m_stormNexusColor;
            default:
                return m_stormNexusColor;
        }
    }

    public LevelData GetLevelData(string id) {
        // initialize the map if it does not exist
        if (instance.m_levelMap == null) {
            instance.m_levelMap = new Dictionary<string, LevelData>();
            foreach (LevelData data in instance.m_levelData) {
                instance.m_levelMap.Add(data.ID, data);
            }
        }
        if (instance.m_levelMap.ContainsKey(id)) {
            return instance.m_levelMap[id];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Level " +
                "with id `{0}' is in the database", id
            ));
        }
    }

    public Dictionary<TileBase, TileData> GetTileDataDict() {
        return m_tileDataDict;
    }

    #region Unity Callbacks

    private void OnEnable() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this.gameObject);
            return;
        }

        m_tileDataDict = ConstructTileDataDict();
    }

    public Dictionary<TileBase, TileData> ConstructTileDataDict() {
        Dictionary<TileBase, TileData> dict = new Dictionary<TileBase, TileData>();

        foreach (TileData tileData in m_tileDataList) {
            foreach (var tile in tileData.Tiles) {
                dict.Add(tile, tileData);
            }
        }

        return dict;
    }

    public List<TileData> GetTileDataList() {
        return m_tileDataList;
    }

    #endregion
}
