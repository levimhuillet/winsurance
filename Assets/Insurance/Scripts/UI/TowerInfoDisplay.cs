using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerInfoDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_info;

    public void Populate(TowerData data) {
        //m_info.text = "Radius: " + data.Radius + "\n";
        m_info.text = "Power: " + data.ProjectileDamage + "\n";

        string targetStr = "Target(s): ";

        if (data.OncomerTargets.Length > 0) {
            targetStr += "Element";
            if (data.NexusTargets.Length > 0) {
                targetStr += ",\n\t      Nexus";
            }
        }
        else if (data.NexusTargets.Length > 0) {
            targetStr += "Nexus";
        }
        else {
            targetStr += "None";
        }
        targetStr += "\n";
        m_info.text += targetStr;

        m_info.text += "Cost: $" + data.Cost + "\n";
    }
}
