using UnityEngine;

public class BaseSceneContext : MonoBehaviour
{
    public static BaseSceneContext Instance { get; private set; }

    public PauseManager PauseManager { get; private set; }
    [field:SerializeField] public GuideSystem GuideSystem { get; private set; }
    [field:SerializeField] public IslandsUpdater IslandsUpdater { get; private set; }
    [field:SerializeField] public BaseCamera BaseCamera { get; private set; }
    [field:SerializeField] public HintRenderer HintRenderer { get; private set; }
    [field:SerializeField] public BaseUI BaseUI { get; private set; }

    private void Awake() {
        PauseManager = new PauseManager();
        
        if(Instance != null)
            throw new System.NotSupportedException($"The scene can have only one \"{nameof(BaseSceneContext)}\"");

        Instance = this;
    }
}