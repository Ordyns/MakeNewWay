using UnityEngine;

public class HintsRenderer : MonoBehaviour
{
    public static HintsRenderer Instance;
    
    public Camera HintCamera;
    public LineRenderer HintLineRenderer;
    public Transform HintIslandsParent;
    [Space]
    [SerializeField] private Mesh wallMesh;

    private void Awake() => Instance = this;

    private void Start() {
        LevelSettings levelSettings = LevelSettings.Instance;

        Transform wallsParent = Instantiate(IslandsContainer.Instance.WallsParent, transform);
        foreach(Transform wall in wallsParent)
            wall.GetComponent<MeshFilter>().mesh = wallMesh;

        HintCamera.orthographicSize = levelSettings.CameraSize;
        if(levelSettings.CustomCameraPosition) HintCamera.transform.localPosition = levelSettings.CameraPosition;

        HintCamera.gameObject.SetActive(false);
    }
}
