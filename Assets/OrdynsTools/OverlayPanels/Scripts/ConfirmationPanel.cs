using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image background;
    [SerializeField] private GameObject cancelButton;

    private Action _onCancelAction;
    private Action _onConfirmAction;

    public AnimatedPanel AnimatedPanel;

    public void Init(string message, Action onCancel, Action onConfirm, bool cancelButtonActive = true){
        messageText.text = message;
        cancelButton.SetActive(cancelButtonActive);

        _onCancelAction = onCancel;
        _onConfirmAction = onConfirm;
    }

    public void Cancel(){
        _onCancelAction?.Invoke();
        ClosePanel();
    }

    public void Confirm(){
        _onConfirmAction?.Invoke();
        ClosePanel();
    }

    public void SetBackgroundColor(Color color) => background.color = color;

    private void ClosePanel() => AnimatedPanel.Close(() => Destroy(gameObject));
}
