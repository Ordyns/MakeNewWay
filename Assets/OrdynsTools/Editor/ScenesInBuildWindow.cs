using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ScenesInBuildWindow : EditorWindow
{
    private Vector2 _scenesTabScrollPosition;
    private string[] _guids;

    private bool isHotKeyPressed;

    [MenuItem("Tools/Scenes in build")]
    public static void Init(){
        var window = EditorWindow.GetWindow<ScenesInBuildWindow>("Scenes in build");
        window.Show();
    }

    protected virtual void OnGUI(){
        MainTab();
        
        if(CheckEventKey(KeyCode.LeftAlt, EventType.KeyDown))
            isHotKeyPressed = true;
        else if(CheckEventKey(KeyCode.LeftAlt, EventType.KeyUp))
            isHotKeyPressed = false;
    }

    private bool CheckEventKey(KeyCode key, EventType eventType){
        if(Event.current.type == eventType && Event.current.keyCode == key){
            Event.current.Use();
            return true;
        }

        return false;
    }

    protected virtual void MainTab(){
        var headlineStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 14 };
        EditorGUILayout.LabelField("Scenes in build", headlineStyle, GUILayout.ExpandWidth(true));

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        _guids = AssetDatabase.FindAssets("t:Scene");

        _scenesTabScrollPosition = EditorGUILayout.BeginScrollView(_scenesTabScrollPosition);
        EditorGUILayout.BeginVertical();

        if (_guids.Length == 0)
            GUILayout.Label("No Scenes Found", EditorStyles.centeredGreyMiniLabel);

        for (int i = 0; i < _guids.Length; i++){
            string scenePath = AssetDatabase.GUIDToAssetPath(_guids[i]);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            EditorBuildSettingsScene buildScene = buildScenes.Find((editorBuildScene) => {
                return editorBuildScene.path == scenePath;
            });

            if (buildScene == null)
                continue;

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            bool isOpened = scene.IsValid() && scene.isLoaded;

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label((isOpened ? "• " : "") + sceneAsset.name, isOpened ? EditorStyles.whiteLabel : EditorStyles.wordWrappedLabel);

            if(isOpened){
                if(isHotKeyPressed && GUILayout.Button("Unload", GUILayout.Width(60))){
                    UnloadScene(scenePath);
                }
                else if(isHotKeyPressed == false){
                    GUILayout.Label("Opened", GUILayout.Width(55));
                } 
            }
            else{
                if (GUILayout.Button(isHotKeyPressed ? "Add" : "Open", GUILayout.Width(60))){
                    OpenScene(scenePath, isHotKeyPressed);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
    }

    public virtual void OpenScene(string path, bool additive = false) {
        if(additive){
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            return;
        }

        if (EditorSceneManager.EnsureUntitledSceneHasBeenSaved("You don't have saved the Untitled Scene, Do you want to leave?")){
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()){
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }
        }
    }

    public void UnloadScene(string path){
        Scene sceneToUnload = EditorSceneManager.GetSceneByPath(path);
        if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[]{sceneToUnload})) {
            EditorSceneManager.CloseScene(sceneToUnload, true);
        }
    }
}