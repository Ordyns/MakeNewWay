using UnityEngine;

public class HintRenderer : MonoBehaviour
{    
    [field:SerializeField] public Camera HintCamera { get; private set; }
    [field:SerializeField] public LineRenderer HintLineRenderer { get; private set; }
    [field:SerializeField] public Transform HintIslandsParent { get; private set; }
    [Space]
    [SerializeField] private string layer;

    public static LayerMask HintLayer { get; private set; }

    public void Init(LevelSettings levelSettings) {
        HintLayer = LayerMask.NameToLayer(layer);
        
        HintCamera.orthographicSize = levelSettings.CameraSize;
        if(levelSettings.CustomCameraPosition) HintCamera.transform.localPosition = levelSettings.CameraPosition;

        HintCamera.gameObject.SetActive(false);
    }
}