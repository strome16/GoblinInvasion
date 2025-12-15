using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }

    [Header("HUD Elements")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Game Over Elements")]
    [SerializeField] private GameObject gameoverPanel;

    private bool isGameOver = false;

    private void Awake()
    {
        // simple singleton so other scripts can call MainUI.Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (gameoverPanel != null)
            gameoverPanel.SetActive(false);
        if (hudPanel != null)
            hudPanel.SetActive(true);
    }

   public void UpdateHealth(int current, int max)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {current}";
        }
    }

    public void UpdateWave(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {waveNumber}";
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (hudPanel != null)
            hudPanel.SetActive(false);

        if (gameoverPanel != null)
            gameoverPanel.SetActive(true);
    }

    void Update()
    {
        if (!isGameOver) return;

        // Space = restart current scene
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.buildIndex);
        }

        // Esc = go to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("menu");
        }
    }
}
