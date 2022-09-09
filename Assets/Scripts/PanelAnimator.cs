using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class PanelAnimator : MonoBehaviour
{
    [Header("===== Headline =====")]
    [SerializeField] private CanvasGroup headline;
    [SerializeField] private Vector3 headlineStartOffset = new Vector2(0, -64);

    [Header("===== Buttons =====")]
    [SerializeField] private CanvasGroup[] buttons;
    [SerializeField] private Vector3 buttonStartOffset = new Vector2(0, -64);

    [Header("===== Settings =====")]
    [SerializeField] private bool playOnStart = true;
    [Space]
    [SerializeField] private float oneElementAnimationDuration = 0.25f;
    [SerializeField] private float nextElementAnimationDelay = 0.1f;
    [SerializeField] private Ease animatoinEase = Ease.OutCubic;

    private Vector3 _headlineOrignalPosition;
    private List<Vector3> _buttonsOrignalPosition = new List<Vector3>();
    
    private void Awake() {
        _deactivationTimer = new Timer(this, nextElementAnimationDelay * (buttons.Length + 2) / 2);
        _deactivationTimer.Completed += () => gameObject.SetActive(false);

        void PrepareElement(CanvasGroup canvasGroup, Vector3 offset){
            canvasGroup.transform.localPosition += offset;
            canvasGroup.alpha = 0;
        }

        _headlineOrignalPosition = headline.transform.localPosition;
        PrepareElement(headline, (Vector3) headlineStartOffset);

        for(int i = 0; i < buttons.Length; i++){
            _buttonsOrignalPosition.Add(buttons[i].transform.localPosition);
            PrepareElement(buttons[i], (Vector3) buttonStartOffset);
        }
    }

    private void Start() {
        if(playOnStart == false)
            return;

        OpenPanel();
    }

    public void OpenPanel(){
        gameObject.SetActive(true);
        
        if(_deactivationTimer.IsRunning)
            _deactivationTimer.Stop();

        AnimateElement(headline, _headlineOrignalPosition, targetAlpha: 1);
        for(int i = 0; i < buttons.Length; i++)
            AnimateElement(buttons[i], _buttonsOrignalPosition[i], targetAlpha: 1, nextElementAnimationDelay * (i + 1));
    }

    private Timer _deactivationTimer;
    public void ClosePanel(bool deactivateGameObject){
        for(int i = 0; i < buttons.Length; i++)
            AnimateElement(buttons[i], _buttonsOrignalPosition[i] + headlineStartOffset, targetAlpha: 0, nextElementAnimationDelay * (i + 1) / 2);

        AnimateElement(headline, _headlineOrignalPosition + buttonStartOffset, targetAlpha: 0);

        if(deactivateGameObject)
            _deactivationTimer.Start();
    }

    private void AnimateElement(CanvasGroup element, Vector2 targetPosition, float targetAlpha, float delay = 0){
        element.transform.DOLocalMove(targetPosition, oneElementAnimationDuration).SetEase(animatoinEase).SetDelay(delay);
        element.DOFade(targetAlpha, oneElementAnimationDuration).SetDelay(delay);
    }
}