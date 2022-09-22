using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StepsTextAnimator : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] private float scaleAnimationDuration = 0.2f;
    [SerializeField] private Ease scaleAnimationEase = Ease.OutCubic;

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 1;
    [SerializeField] private Ease shakeEase = Ease.Flash;
    
    [Header("Color")]
    [SerializeField] private float colorAnimationDuration = 0.2f;
    [SerializeField] private Ease colorAnimationEase;

    private TextMeshProUGUI _text;

    private void OnValidate() {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private Tweener _scaleTweener;
    public void PlayScaleAnimation(){
        if(_scaleTweener != null){
            _scaleTweener.Restart();
        }
        else{
            _scaleTweener = transform.DOScale(new Vector2(1.1f, 1.1f), scaleAnimationDuration).SetEase(scaleAnimationEase).SetAutoKill(false);
            _scaleTweener.OnComplete(() => _scaleTweener.PlayBackwards());
        }
    }

    private Tweener _colorTweener;
    public void PlayShakeAnimation(){
        transform.DOShakePosition(shakeDuration, shakeStrength).SetEase(shakeEase);

        if(_colorTweener != null){
            _colorTweener.Restart();
        }
        else{
            _colorTweener = _text.DOColor(Color.red, colorAnimationDuration).SetEase(colorAnimationEase).SetAutoKill(false);
            _colorTweener.OnComplete(() => _colorTweener.PlayBackwards());
        }
    }
}

