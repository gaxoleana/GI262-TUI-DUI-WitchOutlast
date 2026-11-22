using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Player player;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text resultsText;
    [SerializeField] private Button restartButton;

    private bool isRestarting;

    private void OnEnable()
    {
        gameManager.RunEnded += ShowResults;
    }

    private void OnDisable()
    {
        gameManager.RunEnded -= ShowResults;
    }

    private void Start()
    {
        endPanel.SetActive(false);
    }

    private void ShowResults()
    {
        int totalSeconds = Mathf.FloorToInt(gameManager.ElapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        resultsText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00")
            + "\nKills: " + gameManager.KillCount
            + "\nLevel: " + player.Level;

        endPanel.SetActive(true);
        restartButton.Select();
    }

    public void RestartGame()
    {
        if (isRestarting || gameManager.IsRunning || !endPanel.activeSelf)
        {
            return;
        }

        isRestarting = true;
        restartButton.interactable = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameObject.scene.path);
    }
}
