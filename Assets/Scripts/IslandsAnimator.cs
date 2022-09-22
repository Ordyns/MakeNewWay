using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IslandsAnimator : MonoBehaviour
{
    private const float AnimatonDelay = 0.075f;
    private const float AnimationDuration = 0.5f;
    private float _nextIslandAnimationDelay => AnimationDuration / _islands.Count;

    [SerializeField] private bool playOnStart;

    private List<Island> _islands = new List<Island>();
    private List<Vector3> _originalSizes = new List<Vector3>();

    public void Init(List<Island> islands){
        _islands = islands;

        foreach(Island island in _islands){
            _originalSizes.Add(island.transform.localScale);
            island.transform.localScale = Vector3.zero;
        }
    }

    private IEnumerator Start() {
        if(playOnStart == false)
            yield break;

        yield return new WaitForSeconds(AnimatonDelay);
        Animate();
    }

    public void Animate(){
        for (int i = 0; i < _islands.Count; i++)
            _islands[i].transform.DOScale(_originalSizes[i], 0.3f).SetEase(Ease.OutCubic).SetDelay(_nextIslandAnimationDelay * i);
    }
}