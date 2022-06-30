using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static DirectionExtensions;


[CustomEditor(typeof(Island))]
public class IslandEditor : Editor
{
    private Island _island;
    private SerializedObject _serializedObject;

    private EditorIslandTextureAsset _emptyIslandTextureAsset;
    private EditorIslandTextureAsset _defaultIslandTextureAsset;
    private EditorIslandTextureAsset _cornerIslandTextureAsset;
    private EditorIslandTextureAsset _cornerMirroredIslandTextureAsset;
    private EditorIslandTextureAsset _startIslandTextureAsset;
    private EditorIslandTextureAsset _finishIslandTextureAsset;

    private EditorIslandTextureAsset _islandTextureAsset => GetCurrentIslandAsset();

    private EditorIslandTextureAsset _activeDirectionButtonsAsset;
    private EditorIslandTextureAsset _inactiveDirectionButtonsAsset;

    private bool isInputAndOutputLinked = true;
    private bool isInputMirrored {
        get => _island.isInputMirrored;
        set {
            if(value != _island.isInputMirrored){
                _serializedObject.FindProperty(nameof(_island.isInputMirrored)).boolValue = value;
                DirectionsChanged(true);
            } 
        }
    }

    private Texture _linkedTexture;
    private Texture _unlinkedTexture;

    protected Texture selectedOutline;

    private Vector3 _originalRendererScale;

    private void OnEnable() {
        _island = (Island) target;

        _serializedObject = new SerializedObject(_island);

        _emptyIslandTextureAsset = LoadIslandTextureAsset("Empty");
        _defaultIslandTextureAsset = LoadIslandTextureAsset("Default");
        _cornerIslandTextureAsset = LoadIslandTextureAsset("Corner");
        _cornerMirroredIslandTextureAsset = LoadIslandTextureAsset("MirroredCorner");
        _startIslandTextureAsset = LoadIslandTextureAsset("Start");
        _finishIslandTextureAsset = LoadIslandTextureAsset("Finish");

        _activeDirectionButtonsAsset = LoadIslandTextureAsset("DirectionButtons/Active");
        _inactiveDirectionButtonsAsset = LoadIslandTextureAsset("DirectionButtons/Inactive");

        _linkedTexture = (Texture) Resources.Load("Editor/linked", typeof(Texture));
        _unlinkedTexture = (Texture) Resources.Load("Editor/unlinked", typeof(Texture));

        selectedOutline = (Texture) Resources.Load("Editor/selected_outline", typeof(Texture));

        UpdateRenderer(_island.GetOutputDirection(false));

        AdditionalInit();
    }

    public virtual void AdditionalInit() { }

    private EditorIslandTextureAsset GetCurrentIslandAsset(){
        if(_island.IslandType == Island.IslandTypes.Corner){
            if(isInputMirrored) return _cornerMirroredIslandTextureAsset;
            else return _cornerIslandTextureAsset;
        } 
        else if(_island.IslandType == Island.IslandTypes.Start) return _startIslandTextureAsset;
        else if(_island.IslandType == Island.IslandTypes.Finish) return _finishIslandTextureAsset;
        else if(_island.IslandType == Island.IslandTypes.Empty) return _emptyIslandTextureAsset;
        else return _defaultIslandTextureAsset;
    }

    private EditorIslandTextureAsset LoadIslandTextureAsset(string folderName){
        EditorIslandTextureAsset textureAsset = new EditorIslandTextureAsset();
        textureAsset.UpperLeft = (Texture) Resources.Load($"Editor/Islands/{folderName}/upper_left", typeof(Texture));
        textureAsset.UpperRight = (Texture) Resources.Load($"Editor/Islands/{folderName}/upper_right", typeof(Texture));
        textureAsset.DownLeft = (Texture) Resources.Load($"Editor/Islands/{folderName}/down_left", typeof(Texture));
        textureAsset.DownRight = (Texture) Resources.Load($"Editor/Islands/{folderName}/down_right", typeof(Texture));
        return textureAsset;
    }

    public GUIStyle HeadlineStyle => new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 15, fontStyle = FontStyle.Bold};

    private Vector2 scenesTabScrollPosition;
    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        GUILayout.Label("Island type", HeadlineStyle);

        scenesTabScrollPosition = EditorGUILayout.BeginScrollView(scenesTabScrollPosition);
        GUILayout.BeginHorizontal("box");
        DrawIslandTypeButton(_emptyIslandTextureAsset.DownRight, Island.IslandTypes.Empty);
        DrawIslandTypeButton(_defaultIslandTextureAsset.DownRight, Island.IslandTypes.Default);
        DrawIslandTypeButton(_cornerIslandTextureAsset.DownRight, Island.IslandTypes.Corner);
        DrawIslandTypeButton(_startIslandTextureAsset.DownRight, Island.IslandTypes.Start);
        DrawIslandTypeButton(_finishIslandTextureAsset.DownRight, Island.IslandTypes.Finish);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        if(_island.IslandType != Island.IslandTypes.Empty){
            DrawLine(1, 10);
    
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
    
            Rect inputRect = new Rect();
            Rect outputRect = new Rect();
    
            if(_island.IslandType != Island.IslandTypes.Start){
                Direction direction = _island.GetInputDirection(false).GetMirroredDirection();
                if(_island.IslandType == Island.IslandTypes.Corner){
                    direction = GetDirectionFromAngle(direction.ToDegrees() - 270 + (isInputMirrored ? 180 : 0));
                }
    
                DrawDirectionSelector("Input", direction, true, out inputRect);
                DrawDirectionButtons(inputRect, true, _island.InputPropertyName);
            }
    
            if(_island.IslandType != Island.IslandTypes.Start && _island.IslandType != Island.IslandTypes.Finish){
                GUILayout.Space(15);
        
                EditorGUILayout.BeginVertical();
                GUILayout.Space(76 / 2 + 13);
                if(GUILayout.Button(isInputAndOutputLinked ? _linkedTexture : _unlinkedTexture, GUIStyle.none, GUILayout.Width(25), GUILayout.Height(25))) 
                    isInputAndOutputLinked = !isInputAndOutputLinked;
                EditorGUILayout.EndVertical();
        
                GUILayout.Space(15);
            }
    
            if(_island.IslandType != Island.IslandTypes.Finish){
                DrawDirectionSelector("Output", _island.GetOutputDirection(false), false, out outputRect);
                DrawDirectionButtons(outputRect, false, _island.OutputPropertyName);
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(6);
    
            GUIStyle informationLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12 };
            informationLabelStyle.normal.textColor = Color.gray;
            GUILayout.Label("The rotation of the island in the inspector \nis not taken into account",  informationLabelStyle);
        }

        DrawLine(1, 15);

        DrawAdditionalSection();

        _island.defaultRenderer = (MeshRenderer)EditorGUILayout.ObjectField("Default renderer", _island.defaultRenderer, typeof(MeshRenderer), true);
        _island.cornerRenderer = (MeshRenderer)EditorGUILayout.ObjectField("Corner renderer", _island.cornerRenderer, typeof(MeshRenderer), true);

        DrawLine(1, 15);

        EditorGUILayout.PropertyField(_serializedObject.FindProperty(_island.UpdatingSoundPropertyName), new GUIContent("Updating sound"));

        _serializedObject.ApplyModifiedProperties();
    }

    public virtual void DrawAdditionalSection() { }
    
    private void DirectionsChanged(bool input){
        if(isInputAndOutputLinked == false)
            return;

        Direction inputDirection = (Direction) _serializedObject.FindProperty(_island.InputPropertyName).enumValueIndex;
        Direction outputDirection = (Direction) _serializedObject.FindProperty(_island.OutputPropertyName).enumValueIndex;
        Direction newDirection = (input ? inputDirection : outputDirection).GetMirroredDirection();

        if(_island.IslandType == Island.IslandTypes.Corner){
            newDirection = GetDirectionFromAngle(newDirection.ToDegrees() - (input ? 270 : 90));

            if(isInputMirrored)
                newDirection = newDirection.GetMirroredDirection();
        }

        if(input) _serializedObject.FindProperty(_island.OutputPropertyName).enumValueIndex = (int) newDirection;
        else _serializedObject.FindProperty(_island.InputPropertyName).enumValueIndex = (int) newDirection;

        _serializedObject.ApplyModifiedProperties();
        Undo.RecordObject(_island, $"{(input ? "input" : "output")} direction changes");
        UpdateRenderer(_island.GetOutputDirection(false));
    }

    private void UpdateRenderer(Direction direction){
        if(_island.defaultRenderer) _island.defaultRenderer.gameObject.SetActive(_island.IslandType != Island.IslandTypes.Corner);
        if(_island.cornerRenderer) _island.cornerRenderer.gameObject.SetActive(_island.IslandType == Island.IslandTypes.Corner);

        Transform renderer = _island.defaultRenderer.transform;
        if(_island.IslandType == Island.IslandTypes.Corner){
            renderer = _island.cornerRenderer.transform;
            
            renderer.transform.localScale = Vector3.Scale(Vector3.one, new Vector3(isInputMirrored ? -1 : 1, 1, 1));
        }

        float offset = _island.IslandType == Island.IslandTypes.Finish ? 0 : 180;
        renderer.localEulerAngles = new Vector3(0, direction.ToDegrees() + offset, 0);
    }

    private void DrawDirectionSelector(string header, Direction direction, bool mirrorToggle, out Rect selectorRect){
        EditorGUILayout.BeginVertical();
        GUILayout.Label(header, HeadlineStyle);
        GUILayout.Label(GetTextureFromAssetByDirection(_islandTextureAsset, direction), new GUIStyle() { fixedHeight = 75, fixedWidth = 75 });
        selectorRect = GUILayoutUtility.GetLastRect();
        selectorRect.x -= 1;

        if(_island.IslandType == Island.IslandTypes.Corner && mirrorToggle) 
            isInputMirrored = EditorGUILayout.ToggleLeft("Mirror", isInputMirrored, GUILayout.Width(70));
        EditorGUILayout.EndVertical();
    }

    private void DrawDirectionButtons(Rect rect, bool input, string directionPropertyString){
        float width = 38;
        DrawDirectionButton(new Rect(rect.x, rect.y, width, width), _activeDirectionButtonsAsset.UpperLeft, _inactiveDirectionButtonsAsset.UpperLeft, input, Direction.UpperLeft, directionPropertyString);
        DrawDirectionButton(new Rect(rect.x + width, rect.y, width, width), _activeDirectionButtonsAsset.UpperRight, _inactiveDirectionButtonsAsset.UpperRight, input, Direction.UpperRight, directionPropertyString);
        DrawDirectionButton(new Rect(rect.x + width, rect.y + width, width, width), _activeDirectionButtonsAsset.DownRight, _inactiveDirectionButtonsAsset.DownRight, input, Direction.DownRight, directionPropertyString);
        DrawDirectionButton(new Rect(rect.x, rect.y + width, width, width), _activeDirectionButtonsAsset.DownLeft, _inactiveDirectionButtonsAsset.DownLeft, input, Direction.DownLeft, directionPropertyString);
    }

    private void DrawDirectionButton(Rect rect, Texture activeTexture, Texture inactiveTexture, bool input, Direction direction, string directionPropertyString){
        Direction propertyAsDirection = (Direction) _serializedObject.FindProperty(directionPropertyString).enumValueIndex;
        if(GUI.Button(rect, new GUIContent((propertyAsDirection == direction) ? activeTexture : inactiveTexture), new GUIStyle(GUIStyle.none))){
            _serializedObject.FindProperty(directionPropertyString).enumValueIndex = (int) direction;
            DirectionsChanged(input);
        }
    }

    private void DrawIslandTypeButton(Texture texture, Island.IslandTypes type){
        GUIStyle buttonStyle = new GUIStyle(GUIStyle.none);
        if(_island.IslandType == type)
            buttonStyle.normal.background = (Texture2D) selectedOutline;

        GUILayout.FlexibleSpace();
        if(GUILayout.Button(texture, buttonStyle, GUILayout.Width(76), GUILayout.Height(76))){
            _serializedObject.FindProperty(nameof(_island.IslandType)).enumValueIndex = (int) type;
            _serializedObject.ApplyModifiedProperties();
            DirectionsChanged(true);
        }
    }

    private Texture GetTextureFromAssetByDirection(EditorIslandTextureAsset asset, Direction direction){
        switch(direction){
            case Direction.UpperLeft: return asset.UpperLeft;
            case Direction.UpperRight: return asset.UpperRight;
            case Direction.DownLeft: return asset.DownLeft;
            case Direction.DownRight: return asset.DownRight;
        }

        return null;
    }

    public void DrawLine(float height, float space, float widthOffset = 0){
        GUILayout.Space(space);
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        rect.width -= widthOffset;
        rect.x += widthOffset / 2;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        GUILayout.Space(space);
    }

    public struct EditorIslandTextureAsset{
        public Texture UpperLeft;
        public Texture UpperRight;
        public Texture DownLeft;
        public Texture DownRight;
    }
}
