using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class CameraAnimator : MonoBehaviour
{
    public float OriginalSize;
    public float SizeOffset = 5;
    public float Duration => duration;
    [Space]
    [SerializeField] private bool playOnStart;
    [Space]
    [SerializeField] private float inAnimationDelay = 0.3f;
    [Space]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Camera _camera;

    private void Start() {
        _camera = GetComponent<Camera>();

        if(playOnStart){
            _camera.orthographicSize = OriginalSize + SizeOffset;
            PlayInAnimation();
        }
    }

    public void PlayInAnimation() => _camera.DOOrthoSize(OriginalSize, duration).SetEase(ease).SetDelay(inAnimationDelay);
    public void PlayOutAnimation(float delay = 0) => _camera.DOOrthoSize(OriginalSize + SizeOffset, duration).SetEase(ease).SetDelay(delay);
}