using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class Transition : MonoBehaviour
{
    public System.Action InAnimationFinished;

    [SerializeField] private float partsAnimationDuration;
    [SerializeField] private Ease partsAnimationEase;
    [Space]
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private Image topPart;
    [SerializeField] private Image bottomPart;

    private float _canvasHeight;
    private Coroutine _loadingTextRoutine;
    private bool isLoading;

    public void Init(Color transitionColor){
        _canvasHeight = GetComponent<RectTransform>().sizeDelta.y;

        topPart.transform.localPosition = new Vector2(0, _canvasHeight);
        bottomPart.transform.localPosition = new Vector2(0, -_canvasHeight);
        mainText.alpha = 0;

        bottomPart.color = topPart.color = transitionColor;
    }

    public void StartInAnimation() {
        isLoading = true;
        Animate(0, 1);
        Timer.StartNew(this, partsAnimationDuration, () => {
            _loadingTextRoutine = StartCoroutine(LoadingTextAnimation());
            InAnimationFinished();
        });
    }

    public void Close() {
        if(_loadingTextRoutine != null) 
            StopCoroutine(_loadingTextRoutine);

        isLoading = false;
        
        mainText.text = "Done";
        Timer.StartNew(this, 0.3f, () => Animate(_canvasHeight, 0));
        Timer.StartNew(this, 0.3f + partsAnimationDuration, () => gameObject.SetActive(false));
    }

    private IEnumerator LoadingTextAnimation(){
        string[] loadingStates = new string[] {"Loading.", "Loading..", "Loading..."};

        while(isLoading){
            for(int i = 0; i < loadingStates.Length; i++){
                mainText.text = loadingStates[i];
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }
    }

    private void Animate(float targetPosition, float targetTextAlpha){
        bottomPart.transform.DOLocalMoveY(-targetPosition, partsAnimationDuration).SetEase(partsAnimationEase);
        topPart.transform.DOLocalMoveY(targetPosition, partsAnimationDuration).SetEase(partsAnimationEase);
        mainText.DOFade(targetTextAlpha, partsAnimationDuration * 0.75f);
    }
}
