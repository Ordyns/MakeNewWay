using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class AnimatedPanel : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private AwakeAction awakeAction = AwakeAction.CloseInstantly;
    [SerializeField] private AwakeAction startAction = AwakeAction.NoAction;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [Space]
    [SerializeField] private bool independentOfTimeScale = true;
    [Space]
    [SerializeField] private Axis axis;

    [Header("Position")]
    [SerializeField] private AutoPosition autoOpenedPosition;
    [SerializeField] private bool invertOpenedPosition;
    [SerializeField] private float openedPosition;
    [Space]
    [SerializeField] private AutoPosition autoClosedPosition;
    [SerializeField] private bool invertClosedPosition;
    [SerializeField] private float closedPosition;

    [Header("Optional")]
    [SerializeField] private float autoOpenDelay;

    private CanvasGroup _canvasGroup;

    private bool isOpened;
    private bool isInitialized;

    private Vector2 _awakePosition;
    private Timer _autoOpenTimer;

    private void Awake() {
        _awakePosition = transform.localPosition;
        _canvasGroup = GetComponent<CanvasGroup>();

        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization(){
        while(CanvasSize.Instance == false)
            yield return null;

        openedPosition = GetPositionUsingAutoPosition(autoOpenedPosition, openedPosition) * (invertOpenedPosition ? -1 : 1);
        closedPosition = GetPositionUsingAutoPosition(autoClosedPosition, closedPosition) * (invertClosedPosition ? -1 : 1);

        InvokeAction(awakeAction);
        isInitialized = true;
    }

    private void Start() {
        InvokeAction(startAction);
    }

    private void InvokeAction(AwakeAction action){
        switch(action){
            case AwakeAction.Close: Close(); break;
            case AwakeAction.Open: Open(); break;
            case AwakeAction.CloseInstantly: CloseInstantly(); break;
            case AwakeAction.OpenInstantly: OpenInstantly(); break;
            case AwakeAction.AutoOpenAfterDelay: _autoOpenTimer = Timer.StartNew(this, autoOpenDelay, () => Open()); break;
        }
    }

    private float GetPositionUsingAutoPosition(AutoPosition autoPosition, float manualValue){
        switch (autoPosition){
            case AutoPosition.PositionEqualsCanvasWidth: return CanvasSize.Size.x;
            case AutoPosition.PositionEqualsCanvasHeight: return CanvasSize.Size.y;
            case AutoPosition.GetPositionOnAwake: return axis == Axis.X ? _awakePosition.x : _awakePosition.y;
            default: return manualValue;
        }
    }

    public void Open() => Open(null);

    public void Open(System.Action onComplete = null){
        isOpened = true;
        AnimatePanel(GetPositionGivenTheAxis(openedPosition), 1, onComplete);
    }

    public void OpenInstantly(){
        isOpened = true;
        transform.localPosition = GetPositionGivenTheAxis(openedPosition);
        _canvasGroup.alpha = 1;
    }

    public void Close() => Close(null);

    public void Close(System.Action onComplete = null){
        isOpened = false;
        AnimatePanel(GetPositionGivenTheAxis(closedPosition), 0, onComplete);
    }

    public void CloseInstantly(){
        isOpened = false;
        transform.localPosition = GetPositionGivenTheAxis(closedPosition);
        _canvasGroup.alpha = 0;
    }

    private void AnimatePanel(Vector2 position, float alpha, System.Action onComplete = null){
        transform.DOLocalMove(position, animationDuration).SetEase(ease).SetUpdate(independentOfTimeScale);
        _canvasGroup.DOFade(alpha, animationDuration).SetEase(Ease.InOutSine).SetUpdate(independentOfTimeScale).OnComplete(() => onComplete?.Invoke());
    }

    private Vector2 GetPositionGivenTheAxis(float position){
        if (axis == Axis.X) return new Vector2(position, transform.localPosition.y);
        else return new Vector2(transform.localPosition.x, position);
    }

    private void OnEnable() {
        if(!isInitialized){
            StartCoroutine(Initialization());
        }
    }

    private void OnDestroy() {
        if(_autoOpenTimer != null)
            _autoOpenTimer.Stop();
    }

    private enum Axis{
        X,
        Y
    }

    private enum AutoPosition{
        Disabled,
        PositionEqualsCanvasWidth,
        PositionEqualsCanvasHeight,
        GetPositionOnAwake
    }


    private enum AwakeAction{
        NoAction,
        Close,
        CloseInstantly,
        Open,
        OpenInstantly,
        AutoOpenAfterDelay
    }
}