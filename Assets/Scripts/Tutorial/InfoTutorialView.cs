using DG.Tweening;
using UnityEngine;

public class InfoTutorialView : MonoBehaviour
{
    private Tweener _scaleTweener;

    private void Awake() {
        transform.localScale = Vector3.zero;
    }

    public void Init(Vector3 position){
        transform.localPosition = position;
    }

    public void Show(){
        _scaleTweener = transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutCubic).SetAutoKill(false);
    }
    
    public void Hide(){
        _scaleTweener.PlayBackwards();
        _scaleTweener.OnComplete(() => gameObject.SetActive(false));
    }
}
