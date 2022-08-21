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

    private void Awake(){
        _camera = GetComponent<Camera>();
        _initialSize = _camera.orthographicSize;

        _targetAspect = defaultResolution.x / defaultResolution.y;

        if(setSizeOnStart)
            _camera.orthographicSize = GetConstSize(_initialSize);
    }

    public float GetConstSize(float initialSize){
        float constantWidthSize = initialSize * (_targetAspect / _camera.aspect);
        return Mathf.Lerp(constantWidthSize, initialSize, widthOrHeight);
    }
}