using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectLoadedHandler : MonoBehaviour
{
    [SerializeField] private ProjectContext context;

    private void Start() {
        PlayerData data = new PlayerData();
        SaveSystem<PlayerData> saveSystem = new SaveSystem<PlayerData>(data);
        data = saveSystem.LoadData();

        if(data != null && data.TutorialCompleted)
            context.ScenesLoader.LoadMenu();
        else
            context.ScenesLoader.LoadTutorial();
    }
}
