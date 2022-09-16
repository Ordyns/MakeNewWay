using UnityEngine;

public class HintRenderer : MonoBehaviour
{    
    public static LayerMask HintLayer { get; private set; }

    [field:SerializeField] public Camera HintCamera { get; private set; }
    [field:SerializeField] public LineRenderer HintLineRenderer { get; private set; }
    [field:SerializeField] public Transform HintIslandsParent { get; private set; }
    [Space]
    [SerializeField] private string layer;

    private Camera _mainCamera;

    public void Init(LevelSettings levelSettings) {
        HintLayer = LayerMask.NameToLayer(layer);

        _mainCamera = Camera.main;
        
        HintCamera.orthographicSize = levelSettings.CameraSize;
        if(levelSettings.CustomCameraPosition) HintCamera.transform.localPosition = levelSettings.CameraPosition;

        HintCamera.gameObject.SetActive(false);
    }

    public void Deactivate(){
        gameObject.SetActive(false);
        HintCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(true);
    }

    public void Activate(){
        gameObject.SetActive(true);
        HintCamera.gameObject.SetActive(true);
        _mainCamera.gameObject.SetActive(false);
    }
}