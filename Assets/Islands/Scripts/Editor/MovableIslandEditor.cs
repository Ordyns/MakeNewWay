using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovableIsland))]
public class MovableIslandEditor : IslandEditor
{
    private Texture _allDirectionsTexture;
    private Texture _upperLeftToDownRightTexture;
    private Texture _downLeftToUpperRightTexture;

    private MovableIsland _movableIsland;
    private SerializedObject _serializedObject;

    public override void OnEnable(){
        base.OnEnable();

        _movableIsland = (MovableIsland) target;
        _serializedObject = new SerializedObject(_movableIsland);
        
        _allDirectionsTexture = LoadEditorTexture($"Editor/Islands/MovableIsland/all_directions");
        _upperLeftToDownRightTexture = LoadEditorTexture($"Editor/Islands/MovableIsland/upper_left-down_right");
        _downLeftToUpperRightTexture = LoadEditorTexture($"Editor/Islands/MovableIsland/down_left-upper_right");

        AdditionalSectionsDrawBegin += OnAdditionalSectionDraw;
    }

    private Texture LoadEditorTexture(string path) => (Texture) Resources.Load(path, typeof(Texture));

    public void OnAdditionalSectionDraw(){
        _serializedObject.Update();

        GUILayout.Label("Move directions", HeadlineStyle);

        EditorGUILayout.BeginHorizontal("box");
        DrawMoveDirectionsButton(_allDirectionsTexture, MovableIsland.MoveDirections.Default);
        DrawMoveDirectionsButton(_upperLeftToDownRightTexture, MovableIsland.MoveDirections.UpperLeftToDownRight);
        DrawMoveDirectionsButton(_downLeftToUpperRightTexture, MovableIsland.MoveDirections.DownLeftToUpperRight);
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(_serializedObject.FindProperty(_movableIsland.MovableIndicatorPropertyName), new GUIContent("Movable indicator"));
        
        DrawLine(1, 15);

        _serializedObject.ApplyModifiedProperties();
    }

    private void DrawMoveDirectionsButton(Texture texture, MovableIsland.MoveDirections moveDirections){
        GUILayout.FlexibleSpace();

        GUIStyle buttonStyle = new GUIStyle(GUIStyle.none);
        if(_movableIsland.IslandMoveDirections == moveDirections)
            buttonStyle.normal.background = (Texture2D) SelectionOutline;

        GUILayout.FlexibleSpace();
        if(GUILayout.Button(texture, buttonStyle, GUILayout.Width(76), GUILayout.Height(76))){
            _serializedObject.FindProperty(nameof(_movableIsland.IslandMoveDirections)).enumValueIndex = (int) moveDirections;
            _serializedObject.ApplyModifiedProperties();

            GameObject movableIndicator = (GameObject) _serializedObject.FindProperty(_movableIsland.MovableIndicatorPropertyName).objectReferenceValue;
            movableIndicator.SetActive(_movableIsland.IslandMoveDirections != MovableIsland.MoveDirections.Default);
            float rotation = _movableIsland.IslandMoveDirections == MovableIsland.MoveDirections.DownLeftToUpperRight ? 90 : 0;
            movableIndicator.transform.eulerAngles = new Vector3(0, rotation, 0);
        }
    }
}
