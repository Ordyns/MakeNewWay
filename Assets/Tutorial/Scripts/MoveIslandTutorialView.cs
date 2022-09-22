using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MoveIslandTutorialView : MonoBehaviour
{
    [SerializeField] private LineRenderer swipeDirectionLine;
    [SerializeField] private CanvasGroup hand;
    [Space]
    [SerializeField] private float loopDelay = 0.75f;
    [SerializeField] private float handMoveAnimationDuration = 0.5f;
    [SerializeField] private float handFadeDuration = 0.25f;

    private CanvasGroup _canvasGroup;

    private Coroutine _handAnimationRoutine;
    private Sequence _handAnimationSequence;

    private Vector3 _firstPosition;
    private Vector3 _secondPosition;

    private bool isShowing;
    private bool isActive;

    private void OnValidate() {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Awake() {
        Hide();
    }

    public void SetPositions(Vector3 firstPosition, Vector3 secondPosition){
        _firstPosition = firstPosition;
        _secondPosition = secondPosition;
        swipeDirectionLine.SetPositions(new Vector3[] { firstPosition, secondPosition });

        StopAnimations();
    }

    public void Show(){
        isShowing = true;

        if(isActive == false)
            isActive = true;

        if(_handAnimationSequence == null)
            StartHandAnimation();
        else 
            _handAnimationSequence.Restart();

        swipeDirectionLine.gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 0.25f);
    }

    public void Hide(){
        isShowing = false;

        swipeDirectionLine.gameObject.SetActive(false);
        _canvasGroup.DOFade(0, 0.25f);
    }

    public void StopAnimations(){
        isActive = false;

        Hide();

        if(_handAnimationSequence.IsActive())
            _handAnimationSequence.Kill();

        _handAnimationSequence = null;
    }

    private void StartHandAnimation(){
        hand.transform.localPosition = _firstPosition;

        _handAnimationSequence = DOTween.Sequence().SetLoops(-1);
        _handAnimationSequence.Append(hand.DOFade(1, handFadeDuration).SetEase(Ease.InOutSine));
        _handAnimationSequence.Insert(0, hand.transform.DOLocalMove(_secondPosition, handMoveAnimationDuration).SetEase(Ease.InOutSine));
        _handAnimationSequence.Append(hand.DOFade(0, handFadeDuration));
        _handAnimationSequence.AppendInterval(loopDelay);
    }
}
