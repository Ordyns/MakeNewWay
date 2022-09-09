using DG.Tweening;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [field:SerializeField] public RectTransform Canvas { get; private set; }
    [Space]
    [SerializeField] private CanvasGroup background;
    [SerializeField] private PanelAnimator tutorialCompletedPanel;

    private void Awake() {
        background.alpha = 0;
    }

    public void ShowTutorialCompletedPanel(float delay){
        Timer.StartNew(this, delay, () => {
            background.DOFade(1, 0.25f).SetEase(Ease.InOutSine);
            tutorialCompletedPanel.OpenPanel();
        });
    }
}
