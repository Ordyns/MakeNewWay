using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AnimatedPanel))]
public class HintView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private BindableAnimatedButton previousStepButton;
    [SerializeField] private BindableAnimatedButton nextStepButton;

    [Header("Other")]
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private CanvasGroup overlay;

    private HintViewModel _hintViewModel;

    private AnimatedPanel _animatedPanel;

    private string _originalContentOfStepsText = string.Empty;

    private void OnValidate() {
        _animatedPanel = GetComponent<AnimatedPanel>();
    }

    public void Init(HintViewModel hintViewModel, System.Func<string, string> getLocalizedValueFunc){
        _hintViewModel = hintViewModel;

        previousStepButton.Bind(hintViewModel.MoveToPreviousStepCommand);
        nextStepButton.Bind(hintViewModel.MoveToNextStepCommand);

        string stepsTextLocalizationKey = stepsText.GetComponent<LocalizedText>().LocalizationKey;
        _originalContentOfStepsText = getLocalizedValueFunc(stepsTextLocalizationKey);

        _hintViewModel.StepChanged += UpdateStepsText;
        UpdateStepsText();
    }

    public void Show(){
        _animatedPanel.Open();
    }

    public void Hide(){
        _animatedPanel.Close();
    }

    public void UpdateStepsText(){
        int currentStep = _hintViewModel.CurrentStep == 0 ? 1 : _hintViewModel.CurrentStep;
        stepsText.text = string.Format(_originalContentOfStepsText, currentStep, _hintViewModel.StepsCount);
    }

    private void AnimateOverlay(System.Action onFadeInFinished){
        overlay.DOFade(1, 0.1f).SetEase(Ease.InOutSine).OnComplete(() => {
            onFadeInFinished?.Invoke();
            overlay.DOFade(0, 0.2f).SetEase(Ease.InOutSine);
        });
    }
}
