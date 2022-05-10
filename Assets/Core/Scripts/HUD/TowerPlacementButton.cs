using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TowerPlacementButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    [SerializeField]
    private TowerInfoDisplay m_towerInfoDisplay;
    private Tower.Type towerType;
    private TowerPlacementManager manager;
    private Button m_button;

    void Awake() {
        manager = transform.parent.parent.GetComponent<TowerPlacementManager>();
        m_button = this.GetComponent<Button>();
    }

    public void SetTower(Tower.Type type, TowerInfoDisplay infoDisplay) {
        TowerData data = GameDB.instance.GetTowerData(type);
        towerType = data.Type;
        image.sprite = data.Sprite;
        m_towerInfoDisplay = infoDisplay;
    }

    public void SetPlacable() {
        manager.SetPlacable(towerType);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        m_towerInfoDisplay.gameObject.SetActive(true);
        TowerData data = GameDB.instance.GetTowerData(towerType);
        m_towerInfoDisplay.Populate(data);
    }

    public void OnPointerExit(PointerEventData eventData) {
        m_towerInfoDisplay.gameObject.SetActive(false);
    }
}
