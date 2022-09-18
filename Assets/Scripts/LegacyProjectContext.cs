using UnityEngine;

public class LegacyProjectContext : MonoBehaviour
{
    public static LegacyProjectContext Instance { get; private set; }

    [field:SerializeField] public AdsManager AdsManager { get; private set; }
    [field:SerializeField] public LevelsInfoProvider LevelsContainer { get; private set; }
    [field:SerializeField] public Localization Localization { get; private set; }
    [field:SerializeField] public Settings Settings { get; private set; }
    [field:SerializeField] public ScenesLoader ScenesLoader { get; private set; }
    [field:SerializeField] private MusicPlayer musicPlayer;

    private void Awake() {
        Instance = this;

        // for(int i = 0; transform.childCount > 0; i++){
        //     Transform child = transform.GetChild(0);
        //     child.parent = null;
        //     DontDestroyOnLoad(child.gameObject);
        // }
    }
}
