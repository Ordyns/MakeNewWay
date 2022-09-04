using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectContext : MonoBehaviour
{
    public static ProjectContext Instance { get; private set; }

    [field:SerializeField] public AdsManager AdsManager { get; private set; }
    [field:SerializeField] public LevelsContainer LevelsContainer { get; private set; }
    [field:SerializeField] public Localization Localization { get; private set; }
    [field:SerializeField] public Settings Settings { get; private set; }
    [field:SerializeField] public ScenesLoader ScenesLoader { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        for(int i = 0; transform.childCount > 0; i++){
            Transform child = transform.GetChild(0);
            child.parent = null;
            DontDestroyOnLoad(child.gameObject);
        }
    }
}
