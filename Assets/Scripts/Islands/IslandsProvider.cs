using System.Collections.Generic;
using UnityEngine;

public class IslandsProvider : MonoBehaviour
{
    public List<Island> Islands { 
        get{
            if(_islands == null)
                _islands = GetIslands();
            
            return _islands;
        }
    }

    private List<Island> _islands;

    [field:SerializeField] public Transform IslandsParent { get; private set; }

    private List<Island> GetIslands(){
        return IslandsParent.GetAllChildrenWithComponent<Island>(false);
    }

    public static List<Transform> GetIslandsTransforms(List<Island> islands){
        HashSet<ComplexIsland> parents = new HashSet<ComplexIsland>();
        List<Transform> islandsTransforms = new List<Transform>();

        foreach (Island island in islands){
            if(island.gameObject.activeSelf == false)
                continue;

            if(island.Parent != null && parents.Contains(island.Parent) == false){
                parents.Add(island.Parent);
                islandsTransforms.Add(island.Parent.transform);
            }
            else if(island.Parent == null){
                islandsTransforms.Add(island.transform);
            }
        }

        return islandsTransforms;
    }
}
