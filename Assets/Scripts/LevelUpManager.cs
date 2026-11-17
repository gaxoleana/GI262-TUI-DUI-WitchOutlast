using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Staff staff;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button staffButton;
    [SerializeField] private TMP_Text staffName;
    [SerializeField] private TMP_Text staffDescription;

    private int pendingUpgrades;

    public int PendingUpgrades
    {
        get { return pendingUpgrades; }
    }

    public bool IsChoosingUpgrade
    {
        get { return pendingUpgrades > 0 && upgradePanel.activeSelf; }
    }

    private void Awake()
    {
        ResetChoices();
    }

    private void OnEnable()
    {
        player.LeveledUp += QueueUpgrade;
        gameManager.RunStarted += ResetChoices;
        gameManager.RunEnded += ResetChoices;
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.LeveledUp -= QueueUpgrade;
        }

        if (gameManager != null)
        {
            gameManager.RunStarted -= ResetChoices;
            gameManager.RunEnded -= ResetChoices;
            gameManager.ResumeRun();
        }

        ResetChoices();
    }

    private void QueueUpgrade()
    {
        if (!gameManager.IsRunning || player.CurrentHealth <= 0 || !staff.CanUpgrade)
        {
            return;
        }

        pendingUpgrades++;
        ShowUpgradeChoices();
    }

    public void ShowUpgradeChoices()
    {
        if (pendingUpgrades <= 0 || !staff.CanUpgrade || !gameManager.IsRunning || player.CurrentHealth <= 0)
        {
            FinishChoices();
            return;
        }

        if (!gameManager.IsPaused && !gameManager.PauseRun())
        {
            ResetChoices();
            return;
        }

        staffName.text = "Staff LV " + staff.WeaponLevel + " > LV " + (staff.WeaponLevel + 1);
        staffDescription.text = "Damage: " + staff.Damage + " -> " + staff.NextDamage
            + "\nFire rate: " + staff.AttackSpeed.ToString("0.00") + " -> "
            + staff.NextAttackSpeed.ToString("0.00") + " shots/s"
            + "\nExtra pierce: " + staff.PierceCount + " -> " + staff.NextPierceCount;

        upgradePanel.SetActive(true);
        staffButton.interactable = true;
        staffButton.Select();
    }

    public void ChooseStaffUpgrade()
    {
        if (!IsChoosingUpgrade || !gameManager.IsRunning || !gameManager.IsPaused || player.CurrentHealth <= 0)
        {
            return;
        }

        if (!upgradeManager.UpgradeWeapon(staff))
        {
            FinishChoices();
            return;
        }

        pendingUpgrades--;
        ShowUpgradeChoices();
    }

    private void FinishChoices()
    {
        ResetChoices();
        gameManager.ResumeRun();
    }

    private void ResetChoices()
    {
        pendingUpgrades = 0;

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        if (staffButton != null && EventSystem.current != null
            && EventSystem.current.currentSelectedGameObject == staffButton.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
