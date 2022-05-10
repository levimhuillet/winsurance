using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TowerPlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject exitButton;

    private Tilemap tilemap;
    private Camera cam;

    [SerializeField]
    private Tower.Type[] m_unlockedTowers;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private TowerInfoDisplay towerDisplay;

    private TowerData targetTowerData = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Tower.Type towerType in m_unlockedTowers) {
            Instantiate(buttonPrefab, buttonHolder.transform).GetComponent<TowerPlacementButton>().SetTower(towerType, towerDisplay);
        }
        cam = FindObjectOfType<Camera>();
        tilemap = FindObjectOfType<Tilemap>();
        targetTowerData = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetTowerData != null) {
            Vector3 currPos = cam.WorldToScreenPoint(tilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition)));
            placementIndicator.transform.position = currPos;
            
        }
    }

    public void PlaceTower() {
        if (targetTowerData == null) {
            return;
        }

        // Check funds
        if (!LevelManager.instance.CheckFunds(targetTowerData.Cost)) {
            return;
        }

        // get the potential placement position
        Vector3 potentialTowerPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // TODO: check if a tower already exists on this square
        bool validCell = TilemapManager.instance.IsValidPlacement(potentialTowerPos);

        if (validCell) {
            GameObject newTowerObj = Instantiate(towerPrefab, tilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition)), towerPrefab.transform.rotation);
            Tower newTower = newTowerObj.GetComponent<Tower>();
            newTower.SetFields(targetTowerData);

            LevelManager.instance.AttemptPurchase(targetTowerData.Cost, LevelManager.PurchaseType.Tower);
        }
        else {
            // TODO: handle full cell case
        }

    }

    public void SetPlacable(Tower.Type towerType) {
        targetTowerData = GameDB.instance.GetTowerData(towerType);
        placementIndicator.SetActive(true);
        exitButton.SetActive(true);
        placementIndicator.GetComponent<Image>().sprite = targetTowerData.Sprite;
    }

    public void RevokePlacable() {
        targetTowerData = null;
        placementIndicator.SetActive(false);
        exitButton.SetActive(false);
    }
}
