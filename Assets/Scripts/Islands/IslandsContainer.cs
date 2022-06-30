using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsContainer : MonoBehaviour
{
    public static IslandsContainer Instance;

    public List<Island> Islands { get; private set; }

    public Transform IslandsParent;
    public Transform WallsParent;
    [Space]
    [SerializeField] private IslandsAnimator islandsAnimator;

    private List<Transform> _walls;

    private void Awake(){
        Instance = this;

        Islands = IslandsParent.GetAllChildrenWithComponent<Island>(false);

        _walls = new List<Transform>();
        for(int i = 0; i < WallsParent.childCount; i++)
            _walls.Add(WallsParent.GetChild(i));
    }

    private void Start(){
        HintSystem.Instance.OnInitializationFinished += () => islandsAnimator.Animate();
    } 
}
