using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject healthBarContainer;
    [SerializeField] private SpriteRenderer healthBar;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private Image expBar;
    [SerializeField] private TMP_Text levelText;

    private Vector3 fullHealthScale;
    private Vector3 fullHealthPosition;
    private float spriteLeft;
    private float healthBarLeftEdge;
    private int displayedSeconds = -1;
    private int displayedKills = -1;
    private int displayedLevel = -1;

    private void Awake()
    {
        fullHealthScale = healthBar.transform.localScale;
        fullHealthPosition = healthBar.transform.localPosition;
        spriteLeft = healthBar.sprite.bounds.min.x;
        healthBarLeftEdge = fullHealthPosition.x + spriteLeft * fullHealthScale.x;
    }

    private void LateUpdate()
    {
        UpdateHealthBar();
        UpdateTimer();
        UpdateKillCount();
        UpdateExpBar();
        UpdateLevel();
    }

    private void UpdateHealthBar()
    {
        bool showHealthBar = player.CurrentHealth < player.MaxHealth;

        if (healthBarContainer.activeSelf != showHealthBar)
        {
            healthBarContainer.SetActive(showHealthBar);
        }

        float healthRatio = 0f;

        if (player.MaxHealth > 0)
        {
            healthRatio = (float)player.CurrentHealth / player.MaxHealth;
        }

        healthRatio = Mathf.Clamp01(healthRatio);

        Vector3 barScale = fullHealthScale;
        barScale.x = fullHealthScale.x * healthRatio;
        healthBar.transform.localScale = barScale;

        Vector3 barPosition = fullHealthPosition;
        barPosition.x = healthBarLeftEdge - spriteLeft * barScale.x;
        healthBar.transform.localPosition = barPosition;
    }

    private void UpdateExpBar()
    {
        float expRatio = 0f;

        if (player.ExpRequired > 0)
        {
            expRatio = (float)player.CurrentExp / player.ExpRequired;
        }

        expBar.fillAmount = Mathf.Clamp01(expRatio);
    }

    private void UpdateLevel()
    {
        int level = player.Level;

        if (level == displayedLevel)
        {
            return;
        }

        levelText.text = "LV " + level.ToString();
        displayedLevel = level;
    }

    private void UpdateTimer()
    {
        int totalSeconds = Mathf.FloorToInt(gameManager.ElapsedTime);

        if (totalSeconds == displayedSeconds)
        {
            return;
        }

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        displayedSeconds = totalSeconds;
    }

    private void UpdateKillCount()
    {
        int kills = gameManager.KillCount;

        if (kills == displayedKills)
        {
            return;
        }

        killCountText.text = kills.ToString();
        displayedKills = kills;
    }
}
