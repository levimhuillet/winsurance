using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevelSelect : MonoBehaviour {
    [SerializeField]
    private Button m_returnButton;

    [SerializeField]
    private float m_colSpacing;
    [SerializeField]
    private float m_rowSpacing;
    [SerializeField]
    private int m_numCols;

    [SerializeField]
    private GameObject m_levelButtonPrefab;
    [SerializeField]
    private GameObject m_buttonHolder;

    private List<GameObject> m_buttonObjs;

    [SerializeField]
    private List<LevelData> m_levels;

    private void OnEnable() {
        m_returnButton.onClick.AddListener(HandleReturn);

        GenerateButtons();
    }

    private void OnDisable() {
        m_returnButton.onClick.RemoveAllListeners();

        Cleanup();
    }

    [ContextMenu("Generate Buttons")]
    private void GenerateButtons() {
        if (m_buttonObjs == null) {
            m_buttonObjs = new List<GameObject>();
        }
        Cleanup();

        int colIndex = 0;
        foreach (LevelData level in m_levels) {
            // instantiate button
            GameObject levelButton = Instantiate(m_levelButtonPrefab, m_buttonHolder.transform);

            // set spacing
            float horizSpacing = (colIndex % m_numCols) * m_colSpacing;
            float vertSpacing = (colIndex / m_numCols) * -m_rowSpacing;
            levelButton.gameObject.transform.position += new Vector3(horizSpacing, vertSpacing, 0);

            // assign scene and text
            levelButton.GetComponent<Button>().onClick.AddListener(delegate { PrepareLevel(level.ID); });
            levelButton.GetComponent<LevelButton>().SetText("" + (colIndex + 1));

            // save to buttons
            m_buttonObjs.Add(levelButton.gameObject);

            // move to next column
            ++colIndex;
        }
    }

    private void Cleanup() {
        /*
        foreach (Button button in m_buttons) {
            button.onClick.RemoveAllListeners();
        }
        m_buttons.Clear();
        */

        foreach (GameObject obj in m_buttonObjs) {
#if UNITY_EDITOR
            DestroyImmediate(obj);
#else
            Destroy(obj);
#endif
        }
        m_buttonObjs.Clear();
    }

    private void PrepareLevel(string levelID) {
        GameManager.instance.CurrLevelID = levelID;
        SceneManager.LoadScene("LevelHolder");
    }

    private void HandleReturn() {
        SceneManager.LoadScene("MainMenu");
    }
}
