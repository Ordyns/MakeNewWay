using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class Transition : MonoBehaviour
{
    public System.Action OnStartTransitionFinished;

    [SerializeField] private float partsAnimationDuration;
    [SerializeField] private Ease partsAnimationEase;
    [Space]
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private Image topPart;
    [SerializeField] private Image bottomPart;

    private float _canvasHeight;
    private Coroutine _loadingTextRoutine;

    private void Start() {
        _canvasHeight = GetComponent<RectTransform>().sizeDelta.y;
        topPart.transform.localPosition = new Vector2(0, _canvasHeight);
        bottomPart.transform.localPosition = new Vector2(0, -_canvasHeight);
        mainText.alpha = 0;

        Animate(targetPosition: 0, targetTextAlpha: 1, () => {
            _loadingTextRoutine = StartCoroutine(LoadingTextCycle());
            OnStartTransitionFinished();
        });
    }

    private IEnumerator LoadingTextCycle(){
        while(true){
            for(int i = 0; i < 4; i++){
                mainText.text = "Loading" + new string('.', i);
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }
    }

    private IEnumerator CloseRoutine(){
        if(_loadingTextRoutine != null) StopCoroutine(_loadingTextRoutine);
        mainText.text = "Done";
        yield return new WaitForSecondsRealtime(0.3f);

        Animate(targetPosition: _canvasHeight, targetTextAlpha: 0, () => {
            Destroy(gameObject);
        });
    }

    public void ChangeColor(Color newColor) => bottomPart.color = topPart.color = newColor;

    public void Close() => StartCoroutine(CloseRoutine());

    private void Animate(float targetPosition, float targetTextAlpha, TweenCallback onComplete){
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0, bottomPart.transform.DOLocalMoveY(-targetPosition, partsAnimationDuration).SetEase(partsAnimationEase));
        sequence.Insert(0, topPart.transform.DOLocalMoveY(targetPosition, partsAnimationDuration).SetEase(partsAnimationEase));
        sequence.Insert(0, mainText.DOFade(targetTextAlpha, partsAnimationDuration * 0.75f));
        sequence.OnComplete(onComplete);
    }
}
