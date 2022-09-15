using UnityEngine;

public class LevelContext : MonoBehaviour
{
    public static LevelContext Instance { get; private set; }

    [field:SerializeField] public HintSystem HintSystem { get; private set; }
    [field:SerializeField] public PathChecker PathChecker { get; private set; }
    [field:SerializeField] public LevelSettings LevelSettings { get; private set; }
    [field:SerializeField] public IslandsContainer IslandsContainer { get; private set; }
    [field:SerializeField] public IslandsAnimator IslandsAnimator { get; private set; }

    private void Awake() {
        if(Instance != null)
            throw new System.NotSupportedException($"The scene can have only one \"{nameof(LevelContext)}\"");

        Instance = this;
    }
}