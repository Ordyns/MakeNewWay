using System.Reflection;
using UnityEditor;
using UnityEngine;
using static DirectionExtensions;

public class IslandEnergyEditor
{
    public event System.Action EnergyUpdated;

    public bool IsInputMirrored => isInputMirrored.Value;
    private ViewModel.ObservableProperty<bool> isInputMirrored = new ViewModel.ObservableProperty<bool>();

    private IslandEnergy _islandEnergy;
    private SerializedObject _serializedObject;

    private FourDirectionalTextureAsset _islandTextureAsset => GetCurrentIslandAsset();

    private FourDirectionalTextureAsset _activeDirectionButtonsAsset;
    private FourDirectionalTextureAsset _inactiveDirectionButtonsAsset;

    private Texture _linkedTexture;
    private Texture _unlinkedTexture;

    private Island.IslandType _currentIslandType;

    private bool isInputAndOutputLinked = true;


    private IslandEditorsTexturesFactory _texturesFactory;

    private GUIStyle _headlineStyle => new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 15, fontStyle = FontStyle.Bold};

    public IslandEnergyEditor(IslandEnergy islandEnergy){
        _islandEnergy = islandEnergy;
    }

    public void OnEnable() {
        _serializedObject = new SerializedObject(_islandEnergy);
        
        var serializedIsInputMirrored = _serializedObject.FindProperty(_islandEnergy.IsInputMirroredFieldName);
        isInputMirrored.Value = serializedIsInputMirrored.boolValue;

        isInputMirrored.Changed += () => {
            serializedIsInputMirrored.boolValue = isInputMirrored.Value;
            DirectionsChanged(true);
        };

        LoadTextures();
    }

    private void LoadTextures(){
        _texturesFactory = new IslandEditorsTexturesFactory();

        _activeDirectionButtonsAsset = new FourDirectionalTextureAsset("DirectionButtons/Active");
        _inactiveDirectionButtonsAsset = new FourDirectionalTextureAsset("DirectionButtons/Inactive");

        _linkedTexture = (Texture) Resources.Load("Editor/linked", typeof(Texture));
        _unlinkedTexture = (Texture) Resources.Load("Editor/unlinked", typeof(Texture));
    }

    public void IslandTypeChanged(Island.IslandType islandType){
        _currentIslandType = islandType;
    }

    public void OnInspectorGUI(){
        _serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Rect inputRect = new Rect();
        Rect outputRect = new Rect();

        if(_islandEnergy.IsInputEnabled){
            Direction direction = _islandEnergy.GetInputDirection(false).GetMirroredDirection();
            if(_currentIslandType == Island.IslandType.Corner){
                direction = GetDirectionFromAngle(direction.ToDegrees() - 270 + (isInputMirrored ? 180 : 0));
            }

            DrawDirectionSelector("Input", direction, true, out inputRect);
            DrawDirectionButtons(inputRect, true, _islandEnergy.InputFieldName);
        }

        if(_islandEnergy.IsInputEnabled && _islandEnergy.IsOutputEnabled){
            GUILayout.Space(15);
    
            EditorGUILayout.BeginVertical();

            GUILayout.Space(76 / 2 + 13);

            if(GUILayout.Button(isInputAndOutputLinked ? _linkedTexture : _unlinkedTexture, GUIStyle.none, GUILayout.Width(25), GUILayout.Height(25))) 
                isInputAndOutputLinked = !isInputAndOutputLinked;

            EditorGUILayout.EndVertical();
    
            GUILayout.Space(15);
        }

        if(_islandEnergy.IsOutputEnabled){
            DrawDirectionSelector("Output", _islandEnergy.GetOutputDirection(false), false, out outputRect);
            DrawDirectionButtons(outputRect, false, _islandEnergy.OutputFieldName);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(6);

        GUIStyle informationLabelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12 };
        informationLabelStyle.normal.textColor = Color.gray;
        GUILayout.Label("The rotation of the island in the inspector \nis not taken into account",  informationLabelStyle);

        _serializedObject.ApplyModifiedProperties();
    }

    private void DrawDirectionSelector(string header, Direction direction, bool mirrorToggle, out Rect selectorRect){
        EditorGUILayout.BeginVertical();
        GUILayout.Label(header, _headlineStyle);
        GUILayout.Label(_islandTextureAsset.GetTextureByDirection(direction), new GUIStyle() { fixedHeight = 75, fixedWidth = 75 });
        selectorRect = GUILayoutUtility.GetLastRect();
        selectorRect.x -= 1;

        if(_currentIslandType == Island.IslandType.Corner && mirrorToggle) 
            isInputMirrored.Value = EditorGUILayout.ToggleLeft("Mirror", isInputMirrored, GUILayout.Width(70));
            
        EditorGUILayout.EndVertical();
    }

    private void DrawDirectionButtons(Rect rect, bool input, string directionPropertyString){
        float width = 38;
        DrawDirectionButton(new Rect(rect.x, rect.y, width, width), _activeDirectionButtonsAsset, _inactiveDirectionButtonsAsset, input, Direction.UpperLeft, directionPropertyString);
        DrawDirectionButton(new Rect(rect.x + width, rect.y, width, width), _activeDirectionButtonsAsset, _inactiveDirectionButtonsAsset, input, Direction.UpperRight, directionPropertyString);
        DrawDirectionButton(new Rect(rect.x + width, rect.y + width, width, width), _activeDirectionButtonsAsset, _inactiveDirectionButtonsAsset, input, Direction.DownRight, directionPropertyString);
        DrawDirectionButton(new Rect(rect.x, rect.y + width, width, width), _activeDirectionButtonsAsset, _inactiveDirectionButtonsAsset, input, Direction.DownLeft, directionPropertyString);
    }

    private void DrawDirectionButton(Rect rect, FourDirectionalTextureAsset activeTexture, FourDirectionalTextureAsset inactiveTexture, bool input, Direction direction, string directionPropertyString){
        Direction propertyAsDirection = (Direction) _serializedObject.FindProperty(directionPropertyString).enumValueIndex;
        GUIContent buttonContent = new GUIContent((propertyAsDirection == direction) ? activeTexture.GetTextureByDirection(direction) : inactiveTexture.GetTextureByDirection(direction));
        if(GUI.Button(rect, buttonContent, new GUIStyle(GUIStyle.none))){
            _serializedObject.FindProperty(directionPropertyString).enumValueIndex = (int) direction;
            DirectionsChanged(input);
        }
    }

    private void DirectionsChanged(bool inputChanged){
        if(isInputAndOutputLinked == false)
            return;

        Direction inputDirection = (Direction) _serializedObject.FindProperty(_islandEnergy.InputFieldName).enumValueIndex;
        Direction outputDirection = (Direction) _serializedObject.FindProperty(_islandEnergy.OutputFieldName).enumValueIndex;
        Direction newDirection = (inputChanged ? inputDirection : outputDirection).GetMirroredDirection();

        if(_currentIslandType == Island.IslandType.Corner){
            newDirection = GetDirectionFromAngle(newDirection.ToDegrees() - (inputChanged ? 270 : 90));

            if(isInputMirrored)
                newDirection = newDirection.GetMirroredDirection();
        }

        if(inputChanged) _serializedObject.FindProperty(_islandEnergy.OutputFieldName).enumValueIndex = (int) newDirection;
        else _serializedObject.FindProperty(_islandEnergy.InputFieldName).enumValueIndex = (int) newDirection;

        _serializedObject.ApplyModifiedProperties();
        Undo.RecordObject(_islandEnergy, $"{(inputChanged ? "input" : "output")} direction changes");

        EnergyUpdated?.Invoke();
    }

    private FourDirectionalTextureAsset GetCurrentIslandAsset(){
        if(isInputMirrored && _currentIslandType == Island.IslandType.Corner)
            return _texturesFactory.GetMirroredCornerTextureAsset();

        return _texturesFactory.GetTextureByIslandType(_currentIslandType);
    }
}
