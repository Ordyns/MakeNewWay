using UnityEngine;
using DG.Tweening;
using System;

[SelectionBase]
public class MovableIsland : Island, ISwipeHandler, IUpdatableIsland
{
    public MoveDirections IslandMoveDirections;
    [Space]
    [SerializeField] private GameObject movableIndicator;

    #if UNITY_EDITOR
    public string MovableIndicatorPropertyName => nameof(movableIndicator);
    #endif

    private const float AnimationDuration = 0.3f;

    public bool OnSwipe(Direction direction, Action onUpdated){
        Vector3 moveDirection = ConvertDirectionToVector(direction);

        if((Parent && Parent.AllChildIslandsCanBeUpdated(direction) == false) || CanBeUpdated(direction) == false)
            return false;

        Transform targetTransform = Parent ? Parent.transform : transform;
        targetTransform.DOLocalMove(targetTransform.position + moveDirection * 6.5f, AnimationDuration).SetEase(Ease.OutCubic).OnComplete(() => onUpdated?.Invoke());
        
        return true;
    }

    public bool CanBeUpdated(Direction direction){
        if(AllowedToMoveInDirection(direction) == false)
            return false;

        if(TryGetIslandInDirection(direction, out Island island))
            return false;
        else if(TryGetWallInDirection(direction, out Transform wall))
            return false;

        return true;
    }

    private bool AllowedToMoveInDirection(Direction direction){
        direction = DirectionExtensions.GetDirectionFromAngle(direction.ToDegrees() + transform.rotation.eulerAngles.y);
        if(IslandMoveDirections == MoveDirections.UpperLeftToDownRight)
            return direction == Direction.UpperLeft || direction == Direction.DownRight;
        else if(IslandMoveDirections == MoveDirections.DownLeftToUpperRight)
            return direction == Direction.DownLeft || direction == Direction.UpperRight;
        else
            return true;
    }

    public enum MoveDirections{
        Default,
        UpperLeftToDownRight,
        DownLeftToUpperRight
    }
}
