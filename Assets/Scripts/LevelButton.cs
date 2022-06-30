using UnityEngine;
using TMPro;
using DG.Tweening;

public class LevelButton : MonoBehaviour
{
    private int levelNumber;
    
    [SerializeField] private CanvasGroup graphics;
    [SerializeField] private float graphicsClosedPosition;
    [SerializeField] private float graphicsLockedAlpha = 0.45f;
    [Space]
    [SerializeField] private AnimatedButton button;
    [Space]
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private GameObject completedPanel;
    [Space]
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private GameObject bonusReceivedPanel;

    private Timer _animationTimer;
    private bool isLevelLocked;

    public void LoadLevel(){
        ScenesLoader.Instance.LoadLevel(levelNumber);
    }

    public void SetLevelNumber(int number){
        levelNumber = number;
        levelNumberText.text = number.ToString();
    }

    public void SetBonusActive(bool active) => bonusPanel.SetActive(active);
    public void BonusReceived() => bonusReceivedPanel.SetActive(true);


    private Sequence _graphicsAnimationSequence;
    private const float GRAPHICS_ANIMATION_DURATION = 0.2f;
    public void Animate(float delay){
        if(_graphicsAnimationSequence != null) _graphicsAnimationSequence.Kill();
        _graphicsAnimationSequence = DOTween.Sequence();

        graphics.transform.localPosition = new Vector3(graphics.transform.localPosition.x, graphicsClosedPosition, graphics.transform.localPosition.y);
        graphics.alpha = 0;

        _graphicsAnimationSequence.Insert(0, graphics.transform.DOLocalMoveY(0, GRAPHICS_ANIMATION_DURATION).SetEase(Ease.OutCubic).SetDelay(delay));
        _graphicsAnimationSequence.Insert(0, graphics.DOFade(GetAlphaForGraphics(), GRAPHICS_ANIMATION_DURATION).SetDelay(delay));
    } 

    public void SetLockedState(bool locked){
        isLevelLocked = locked;
        graphics.alpha = GetAlphaForGraphics();
        button.Interactable = !isLevelLocked;
    }

    private float GetAlphaForGraphics() => isLevelLocked ? graphicsLockedAlpha : 1f;

    public void LevelCompleted(){
        SetLockedState(false);
        completedPanel.SetActive(true);
    }
}