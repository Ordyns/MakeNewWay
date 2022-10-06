using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using NaughtyAttributes;

public class ComplexIsland : MonoBehaviour
{
    [HideInInspector] public ReadOnlyCollection<Island> ChildIslands => _childIslands.AsReadOnly();
    [SerializeField] private bool rotatableIsland;
    [Space]
    [ShowIf(nameof(rotatableIsland))] [SerializeField] private List<RotatableIslandUpdateDirection> rotatableIslandsUpdateDirections;

    private List<Island> _childIslands;

    private void Awake() {
        _childIslands = transform.GetAllChildrenWithComponent<Island>(false);

        foreach (Island island in _childIslands)
            if(island is IUpdatableIsland == false)
                throw new System.NotSupportedException($"{island.transform.name} can't be updated. ComplexIsland supports the following types of islands: (Empty) MovableIsland, (Empty) RotatableIsland");

        _childIslands.ForEach(Island => Island.Parent = this);
    }

    public bool AllChildIslandsCanBeUpdated(Direction direction){
        for(int i = 0; i < _childIslands.Count; i++){
            Direction updateDirection = direction;

            if(rotatableIsland){
                var rotatableIslandUpdateDirection = rotatableIslandsUpdateDirections.Find(direction => direction.Island == _childIslands[i]);

                if(rotatableIslandUpdateDirection.Island != null)
                    updateDirection = DirectionExtensions.GetDirectionFromAngle(updateDirection.ToDegrees() + rotatableIslandUpdateDirection.DirectionOffsetInDegrees);
            }

            IUpdatableIsland updatableIsland = (IUpdatableIsland) _childIslands[i];
            if(updatableIsland.CanBeUpdated(updateDirection) == false)
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected() {
        if(_childIslands == null)
            return;

        for(int i = 0; i < _childIslands.Count; i++){
            Direction updatingDirection = DirectionExtensions.GetDirectionFromAngle(transform.eulerAngles.y + 90);
            var rotatableIslandUpdatingDirection = rotatableIslandsUpdateDirections.Find(direction => direction.Island == _childIslands[i]);

            if(rotatableIsland && rotatableIslandUpdatingDirection.Island != null)
                updatingDirection = DirectionExtensions.GetDirectionFromAngle(updatingDirection.ToDegrees() + rotatableIslandUpdatingDirection.DirectionOffsetInDegrees);

            Gizmos.color = Color.blue;
            Vector3 islandPosition = rotatableIslandsUpdateDirections[i].Island.transform.position;
            Gizmos.DrawRay(islandPosition, new Vector3(Mathf.Sin(updatingDirection.ToDegrees() / Mathf.Rad2Deg), 0, Mathf.Cos(updatingDirection.ToDegrees() / Mathf.Rad2Deg)) * 6.5f);
        }
    }

    [System.Serializable]
    private struct RotatableIslandUpdateDirection{
        public RotatableIsland Island;
        [Space]
        public int DirectionOffsetInDegrees;
    }
}