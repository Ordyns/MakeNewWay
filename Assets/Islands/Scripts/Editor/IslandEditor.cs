using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Island))]
public class IslandEditor : Editor
{
    protected event Action AdditionalSectionsDrawBegin;

    private Island _island;
    private IslandEnergy _islandEnergy;

    private SerializedObject _serializedObject;

    protected Texture SelectionOutline { get; private set; }

    private MethodInfo _islandRendererUpdatedMethodInfo;

    private IslandEditorsTexturesFactory _texturesFactory;

    private IslandEnergyEditor _islandEnergyEditor;

    private SerializedProperty _defaultRendererProperty;
    private SerializedProperty _cornerRendererProperty;

    public virtual void OnEnable() {
        _island = (Island) target;
        _serializedObject = new SerializedObject(_island);

        InitRenderers();

        _texturesFactory = new IslandEditorsTexturesFactory();
        SelectionOutline = (Texture) Resources.Load("Editor/selected_outline", typeof(Texture));

        InitializeIslandEnergyEditor();

        UpdateRenderer(_island.GetOutputDirection(false));
    }

    private void InitRenderers(){
        _islandRendererUpdatedMethodInfo = typeof(Island).GetMethod("RendererUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
        _defaultRendererProperty = _serializedObject.FindProperty(_island.DefaultRendererFieldName);
        _cornerRendererProperty = _serializedObject.FindProperty(_island.CornerRendererFieldName);
    }

    private void InitializeIslandEnergyEditor(){
        _islandEnergy = (IslandEnergy) typeof(Island).GetField("_islandEnergy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_island);
        _islandEnergyEditor = new IslandEnergyEditor(_islandEnergy);
        _islandEnergyEditor.EnergyUpdated += () => UpdateRenderer(_island.GetOutputDirection());
        _islandEnergyEditor.OnEnable();
    }

    protected GUIStyle HeadlineStyle => new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 15, fontStyle = FontStyle.Bold};

    public override void OnInspectorGUI(){
        _serializedObject.Update();

        DrawIslandTypeSelection();

        DrawLine();

        _islandEnergyEditor.OnInspectorGUI();
        
        DrawLine();

        AdditionalSectionsDrawBegin?.Invoke();

        DrawRenderersFields();

        _serializedObject.ApplyModifiedProperties();
    }

    private void DrawRenderersFields(){
        EditorGUILayout.ObjectField(_defaultRendererProperty);
        EditorGUILayout.ObjectField(_cornerRendererProperty);
    }

    private Vector2 _scenesTabScrollPosition;
    private void DrawIslandTypeSelection(){
        GUILayout.Label("Island type", HeadlineStyle);

        _scenesTabScrollPosition = EditorGUILayout.BeginScrollView(_scenesTabScrollPosition);
        GUILayout.BeginHorizontal("box");

        foreach(Island.IslandType islandType in System.Enum.GetValues(typeof(Island.IslandType)))
            DrawIslandTypeButton(islandType);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }

    private void UpdateRenderer(Direction direction){
        MeshRenderer defaultRenderer = typeof(Island).GetField(_island.DefaultRendererFieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_island) as MeshRenderer;
        MeshRenderer cornerRenderer = typeof(Island).GetField(_island.CornerRendererFieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_island) as MeshRenderer;

        if(defaultRenderer) defaultRenderer.gameObject.SetActive(_island.Type != Island.IslandType.Corner);
        if(cornerRenderer) cornerRenderer.gameObject.SetActive(_island.Type == Island.IslandType.Corner);

        Transform renderer = defaultRenderer.transform;
        if(_island.Type == Island.IslandType.Corner){
            renderer = cornerRenderer.transform;
            
            renderer.transform.localScale = new Vector3(_islandEnergyEditor.IsInputMirrored ? -1 : 1, 1, 1);
        }

        float offset = _island.Type == Island.IslandType.Finish ? 0 : 180;
        renderer.localEulerAngles = new Vector3(0, direction.ToDegrees() + offset, 0);

        _islandRendererUpdatedMethodInfo.Invoke(_island, null);

        _islandEnergyEditor.IslandTypeChanged(_island.Type);
    }

    private void DrawIslandTypeButton(Island.IslandType type){
        GUIStyle buttonStyle = new GUIStyle(GUIStyle.none);
        if(_island.Type == type)
            buttonStyle.normal.background = (Texture2D) SelectionOutline;

        Texture texture = _texturesFactory.GetTextureByIslandType(type).DownRight;

        GUILayout.FlexibleSpace();
        if(GUILayout.Button(texture, buttonStyle, GUILayout.Width(76), GUILayout.Height(76))){
            _serializedObject.FindProperty(nameof(_island.Type)).enumValueIndex = (int) type;
            _serializedObject.ApplyModifiedProperties();
            UpdateRenderer(_island.GetOutputDirection(false));
        }
    }

    public void DrawLine(float height = 1, float space = 15, float widthOffset = 0){
        GUILayout.Space(space);
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        rect.width -= widthOffset;
        rect.x += widthOffset / 2;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        GUILayout.Space(space);
    }
}
