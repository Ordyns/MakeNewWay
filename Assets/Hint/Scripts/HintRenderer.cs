using UnityEngine;

public class HintRenderer : MonoBehaviour
{    
    public static LayerMask HintLayer { get; private set; }

    [field:SerializeField] public Transform HintIslandsParent { get; private set; }
    [SerializeField] private Camera hintCamera;
    [SerializeField] private LineRenderer hintLineRenderer;
    [Space]
    [SerializeField] private string layer;

    private Camera _mainCamera;

    [Zenject.Inject]
    private void Init(Camera camera, LevelSettings levelSettings) {
        HintLayer = LayerMask.NameToLayer(layer);

        _mainCamera = camera;
        
        hintCamera.orthographicSize = levelSettings.CameraSize;
        if(levelSettings.CustomCameraPosition) hintCamera.transform.localPosition = levelSettings.CameraPosition;

        hintCamera.gameObject.SetActive(false);
    }

    public void UpdateLineRenderer(bool isActive, Vector3 firstPosition, Vector3 secondPosition){
        hintLineRenderer.gameObject.SetActive(isActive);
        hintLineRenderer.SetPositions(new Vector3[] { firstPosition, secondPosition });
    }

    public void Deactivate(){
        gameObject.SetActive(false);
        hintCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(true);
    }

    public void Activate(){
        gameObject.SetActive(true);
        hintCamera.gameObject.SetActive(true);
        _mainCamera.gameObject.SetActive(false);
    }
}