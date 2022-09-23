using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraConstantWidth : MonoBehaviour
{
    [SerializeField] private bool setSizeOnStart;
    [Space]
    [SerializeField] private Vector2 defaultResolution = new Vector2(1080, 1920);
    [SerializeField] [Range(0f, 1f)] private float widthOrHeight = 0;

    private Camera _camera;
    
    private float _initialSize;
    private float _targetAspect;

    private bool isInitialized;

    private void OnValidate() {
        _camera = GetComponent<Camera>();
    }

    private void Awake(){
        if(isInitialized)
            return;

        _initialSize = _camera.orthographicSize;
        _targetAspect = defaultResolution.x / defaultResolution.y;
        isInitialized = true;
    }

    private void Start() {
        if(setSizeOnStart)
            _camera.orthographicSize = GetConstSize(_initialSize);
    }

    public float GetConstSize(float initialSize){
        if(isInitialized == false)
            Awake();

        float constantWidthSize = initialSize * (_targetAspect / _camera.aspect);
        return Mathf.Lerp(constantWidthSize, initialSize, widthOrHeight);
    }
}