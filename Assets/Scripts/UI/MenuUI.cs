using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel; // title and buttons
    [SerializeField] private GameObject controlsPanel; // control info (hidden at start)


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // enables mouse function
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // show main menu, hide controls on start
        ShowMainMenu();
    }

    void Update()
    {
        // if controls panel is open, press Esc to go back to main menu
        if (controlsPanel != null && 
            controlsPanel.activeSelf &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMainMenu();
        }
    }

    public void StartGame()
    {
        // load main game scene
        SceneManager.LoadScene("main");
    }

    public void ShowControls()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
