using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class AnimatedButton : MonoBehaviour, IPointerClickHandler
{
    public bool Interactable{
       get { return interactable; }
       set { interactable = value; UpdateAlpha(); }    
    }

    [Header("Functionality")]
    [SerializeField] private bool interactable = true;
    [SerializeField] private bool independentOfTimeScale = true;

    [Header("Animation")]
    [SerializeField] private float AnimationDuration = 0.2f;
    [SerializeField] private Ease Ease = Ease.OutCubic;
    [Space]
    [SerializeField] private bool interactableAnimation = true;
    [SerializeField] private float nonInteractableAlpha = 0.8f;

    [Header("Events")]
    public UnityEvent OnClick;

    private CanvasGroup _canvasGroup;

    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();

        UpdateAlpha();
    }

    public void OnPointerClick(PointerEventData eventData){
        if(Interactable){
            OnClick?.Invoke();
            OnClickAnimation();
        }
    }

    private void OnClickAnimation(){
        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(independentOfTimeScale);
        sequence.Append(transform.DOScale(new Vector2(0.9f, 0.9f), AnimationDuration / 2).SetEase(Ease));
        sequence.Append(transform.DOScale(Vector2.one, AnimationDuration / 2).SetEase(Ease));
    }

    private void UpdateAlpha(){
        if(interactableAnimation)
            _canvasGroup.DOFade(interactable ? 1 : nonInteractableAlpha, 0.2f);
    }
}
