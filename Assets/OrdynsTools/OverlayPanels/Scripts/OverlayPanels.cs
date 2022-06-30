using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayPanels : MonoBehaviour
{
    [SerializeField] private ConfirmationPanel confirmationPanelPrefab;
    [SerializeField] private ConfirmationPanel informationPanelPrefab;
    [Space]
    [SerializeField] private Color backgroundColor;

    private static OverlayPanels _instance;

    private void Awake() => _instance = this;

    public static void CreateNewConfirmationPanel(string message, Action onCancel, Action onConfirm){
        ConfirmationPanel panel = Instantiate(_instance.confirmationPanelPrefab);
        panel.Init(message, onCancel, onConfirm);
        panel.SetBackgroundColor(_instance.backgroundColor);
    }

    public static ConfirmationPanel CreateNewInformationPanel(string message, Action onCancel, bool cancelButtonActive = true){
        ConfirmationPanel panel = Instantiate(_instance.informationPanelPrefab);
        panel.Init(message, onCancel, null, cancelButtonActive);
        panel.SetBackgroundColor(_instance.backgroundColor);
        return panel;
    }
}
