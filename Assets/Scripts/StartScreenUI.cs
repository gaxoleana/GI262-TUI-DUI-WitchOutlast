using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class StartScreenUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject startPanel;

    private int readyFrame;
    private bool hasStarted;
    private IDisposable buttonPressSubscription;

    private void OnEnable()
    {
        gameManager.RunStarted += HidePanel;
    }

    private void OnDisable()
    {
        gameManager.RunStarted -= HidePanel;
        StopListening();
    }

    private void Start()
    {
        readyFrame = Time.frameCount;
        startPanel.SetActive(true);
        buttonPressSubscription = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
    }

    private void OnButtonPressed(InputControl control)
    {
        if (hasStarted || Time.frameCount <= readyFrame || !startPanel.activeSelf)
        {
            return;
        }

        StartGame();
    }

    public void StartGame()
    {
        if (hasStarted || gameManager.IsRunning)
        {
            return;
        }

        gameManager.StartRun();
    }

    private void HidePanel()
    {
        hasStarted = true;
        startPanel.SetActive(false);
        StopListening();
    }

    private void StopListening()
    {
        if (buttonPressSubscription != null)
        {
            buttonPressSubscription.Dispose();
            buttonPressSubscription = null;
        }
    }
}
