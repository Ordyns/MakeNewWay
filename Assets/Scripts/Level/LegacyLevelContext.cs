using UnityEngine;

public class LegacyLevelContext : MonoBehaviour
{
    public static LegacyLevelContext Instance { get; private set; }

    [field:SerializeField] public HintSystem HintSystem { get; private set; }
    [field:SerializeField] public LevelSettings LevelSettings { get; private set; }
    [field:SerializeField] public IslandsProvider IslandsContainer { get; private set; }
    [field:SerializeField] public IslandsAnimator IslandsAnimator { get; private set; }

    private void Awake() {
        if(Instance != null)
            throw new System.NotSupportedException($"The scene can have only one \"{nameof(LegacyLevelContext)}\"");

        Instance = this;
    }
}