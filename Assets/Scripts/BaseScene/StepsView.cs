using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class StepsView : MonoBehaviour
{
    [Header("ViewModel")]
    [SerializeField] private StepsViewModel stepsViewModel;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI stepsText;
    [Space]
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private GameObject bonusFilledStar;
    [SerializeField] private TextMeshProUGUI bonusRecievedText;
    [SerializeField] private TextMeshProUGUI stepsForBonusText;

    private void Start() {
        int stepsForBonus = stepsViewModel.StepsForBonus;

        bonusPanel.SetActive(stepsForBonus > 0);
        bonusFilledStar.SetActive(stepsViewModel.IsBonusReceived());

        bonusRecievedText.text = string.Format(bonusRecievedText.text, stepsForBonus);
        stepsForBonusText.text = string.Format(stepsForBonusText.text, stepsForBonus);

        stepsText.text = stepsViewModel.StepsLeft.ToString();
        stepsViewModel.StepsLeft.Changed += OnStepsCountChanded;
    }

    private void OnStepsCountChanded(){
        stepsText.text = stepsViewModel.StepsLeft.ToString();
        stepsText.transform.DOScale(new Vector2(1.1f, 1.1f), 0.2f).SetEase(Ease.OutCubic).OnComplete(() => {
            stepsText.transform.DOScale(Vector2.one, 0.15f).SetEase(Ease.OutCubic);
        });
    }
}
