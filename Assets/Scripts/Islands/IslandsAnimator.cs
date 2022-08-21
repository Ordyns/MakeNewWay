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

    private IEnumerator Start() {
        _islands = IslandsContainer.Instance.Islands;

        if(playOnStart)
            yield return new WaitForSeconds(AnimatonDelay);
        else
            yield break;

        Animate();
    }

    public void Animate(){
        foreach(Island island in _islands){
            _originalSizes.Add(island.transform.localScale);
            island.transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < _islands.Count; i++)
            _islands[i].transform.DOScale(_originalSizes[i], 0.3f).SetEase(Ease.OutCubic).SetDelay(_nextIslandAnimationDelay * i);
    }
}
