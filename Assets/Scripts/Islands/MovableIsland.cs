using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[SelectionBase]
public class MovableIsland : Island
{
    //public MovableComplexIsland Parent;
    [Space]
    public MoveDirections IslandMoveDirections;
    [Space]
    [SerializeField] private GameObject movableIndicator;

    #if UNITY_EDITOR
    public string MovableIndicatorPropertyName => nameof(movableIndicator);
    #endif

    private Direction _lastSwipteDirection;

    public override void OnSwipe(Direction direction){
        base.OnSwipe(direction);

        _lastSwipteDirection = direction;

        Vector3 moveDirection = ConvertDirectionToVector(direction);

        if(Parent && Parent.AllChildIslandsCanBeUpdated(direction) == false)
            return;
        else if(CanMoveInDirection(direction) == false)
            return;

        Transform targetTransform = Parent ? Parent.transform : transform;
        targetTransform.DOLocalMove(targetTransform.position + moveDirection * 6.5f, 0.3f).SetEase(Ease.OutCubic).OnComplete(UpdatingFinished);
        PlayerInput.Instance.IslandUpdating();

        AudioPlayer.PlayClip(base.updatingSound);
    }

    public bool CanMoveInDirection(Direction direction){
        if(AllowedToMoveInDirection(direction) == false)
            return false;

        if(TryGetIslandInDirection(direction, out Island island))
            return false;
        else if(TryGetWallInDirection(direction, out Transform wall))
            return false;

        return true;
    }

    public override bool AdditionalUpdatingCondition(Direction direction) => CanMoveInDirection(direction);

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
