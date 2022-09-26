using System;
using UnityEngine;
using DG.Tweening;

[SelectionBase]
public class RotatableIsland : Island, IClickHandler, IUpdatableIsland
{
    private const float AnimationDuration = 0.275f;

    public bool OnClick(Action onUpdated){
        if(Parent){
            Direction direction = DirectionExtensions.GetDirectionFromAngle(Parent.transform.eulerAngles.y + 90);
            if(Parent.AllChildIslandsCanBeUpdated(direction) == false)
                return false;
        }
            
        Transform targetTransform = Parent ? Parent.transform : transform;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        targetTransform.DOLocalRotateQuaternion(targetRotation, AnimationDuration).SetEase(Ease.OutCubic).OnComplete(() => onUpdated?.Invoke());

        return true;
    }

    public bool CanBeUpdated(Direction direction){
        if(TryGetIslandInDirection(direction, out Island island))
            return false;
        else if(TryGetWallInDirection(direction, out Transform wall))
            return false;
        
        return true;
    }
}