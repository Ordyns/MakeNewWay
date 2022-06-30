using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class ComplexIsland : MonoBehaviour
{
    [HideInInspector] public ReadOnlyCollection<Island> ChildIslands => _childIslands.AsReadOnly();
    [SerializeField] private bool rotatableIsland;
    [Space]
    [ShowIf(nameof(rotatableIsland))] [SerializeField] private List<RotatableIslandUpdatingDirection> rotatableIslandsUpdatingDirections = new List<RotatableIslandUpdatingDirection>();

    private List<Island> _childIslands;

    private void Awake() {
        _childIslands = transform.GetAllChildrenWithComponent<Island>(false);
        _childIslands.ForEach(Island => Island.Parent = this);
    }

    public bool AllChildIslandsCanBeUpdated(Direction direction){
        for(int i = 0; i < _childIslands.Count; i++){
            Direction updatingDirection = direction;
            var rotatableIslandUpdatingDirection = rotatableIslandsUpdatingDirections.Find(direction => direction.Island == _childIslands[i]);

            if(rotatableIsland && rotatableIslandUpdatingDirection.Island != null)
                updatingDirection = DirectionExtensions.GetDirectionFromAngle(updatingDirection.ToDegrees() + rotatableIslandUpdatingDirection.DirectionOffsetInDegrees);

            if(_childIslands[i].TryGetIslandInDirection(updatingDirection, out Island island))
                return false;
            else if(_childIslands[i].TryGetWallInDirection(updatingDirection, out Transform wall))
                return false;
            else if(_childIslands[i].AdditionalUpdatingCondition(updatingDirection) == false)
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected() {
        if(_childIslands == null)
            return;

        for(int i = 0; i < _childIslands.Count; i++){
            Direction updatingDirection = DirectionExtensions.GetDirectionFromAngle(transform.eulerAngles.y + 90);
            var rotatableIslandUpdatingDirection = rotatableIslandsUpdatingDirections.Find(direction => direction.Island == _childIslands[i]);

            if(rotatableIsland && rotatableIslandUpdatingDirection.Island != null)
                updatingDirection = DirectionExtensions.GetDirectionFromAngle(updatingDirection.ToDegrees() + rotatableIslandUpdatingDirection.DirectionOffsetInDegrees);

            Gizmos.color = Color.blue;
            Vector3 islandPosition = rotatableIslandsUpdatingDirections[i].Island.transform.position;
            Gizmos.DrawRay(islandPosition, new Vector3(Mathf.Sin(updatingDirection.ToDegrees() / Mathf.Rad2Deg), 0, Mathf.Cos(updatingDirection.ToDegrees() / Mathf.Rad2Deg)) * 6.5f);
        }
    }

    [System.Serializable]
    private struct RotatableIslandUpdatingDirection{
        public RotatableIsland Island;
        [Space]
        public int DirectionOffsetInDegrees;
    }
}