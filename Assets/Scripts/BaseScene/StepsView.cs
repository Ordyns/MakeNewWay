using UnityEngine;
using TMPro;

public class StepsView : MonoBehaviour
{
    [Header("Steps")]
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private StepsTextAnimator stepsTextAnimator;
    
    [Header("Bonus")]
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private GameObject bonusFilledStar;
    [SerializeField] private TextMeshProUGUI bonusRecievedText;
    [SerializeField] private TextMeshProUGUI stepsForBonusText;
    
    private StepsViewModel _stepsViewModel;

    public void Init(Zenject.SignalBus signalBus, StepsViewModel viewModel){
        _stepsViewModel = viewModel;

        signalBus.Subscribe<CantUpdateIslandSignal>(OnCantUpdateIsland);
    }

    private void Start() {
        int stepsForBonus = _stepsViewModel.StepsForBonus;
        bonusPanel.SetActive(stepsForBonus > 0);
        bonusFilledStar.SetActive(_stepsViewModel.IsBonusReceived());

        bonusRecievedText.text = string.Format(bonusRecievedText.text, stepsForBonus);
        stepsForBonusText.text = string.Format(stepsForBonusText.text, stepsForBonus);

        stepsText.text = _stepsViewModel.StepsLeft.ToString();
        _stepsViewModel.StepsLeft.Changed += OnStepsCountChanged;
    }

    private void OnCantUpdateIsland(){
        stepsTextAnimator.PlayShakeAnimation();
    }

    private void OnStepsCountChanged(){
        stepsText.text = _stepsViewModel.StepsLeft.ToString();
        stepsTextAnimator.PlayScaleAnimation();
    }
}
