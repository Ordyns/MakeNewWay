using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsContainer : MonoBehaviour
{
    [field:SerializeField] public List<Island> Islands { get; private set; }

    [field:SerializeField] public Transform IslandsParent { get; private set; }
    [field:SerializeField] public Transform WallsParent { get; private set; }
    [Space]
    [SerializeField] private IslandsAnimator islandsAnimator;

    private List<Transform> _walls;

    private void Awake(){
        Islands = IslandsParent.GetAllChildrenWithComponent<Island>(false);

        _walls = new List<Transform>();
        for(int i = 0; i < WallsParent.childCount; i++)
            _walls.Add(WallsParent.GetChild(i));
    }

    private void Start(){
        islandsAnimator.Animate();
    }   

    public void Set(Transform islandsParent, Transform wallsParent){
        IslandsParent = islandsParent;
        WallsParent = wallsParent;
    }
}
