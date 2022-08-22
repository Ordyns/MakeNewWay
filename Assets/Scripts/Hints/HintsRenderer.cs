using UnityEngine;

public class HintsRenderer : MonoBehaviour
{    
    [field:SerializeField] public Camera HintCamera { get; private set; }
    [field:SerializeField] public LineRenderer HintLineRenderer { get; private set; }
    [field:SerializeField] public Transform HintIslandsParent { get; private set; }
    [Space]
    [SerializeField] private Mesh wallMesh;

    private void Start() {
        LevelSettings levelSettings = LevelContext.Instance.LevelSettings;

        Transform wallsParent = Instantiate(LevelContext.Instance.IslandsContainer.WallsParent, transform);
        foreach(Transform wall in wallsParent)
            wall.GetComponent<MeshFilter>().mesh = wallMesh;

        HintCamera.orthographicSize = levelSettings.CameraSize;
        if(levelSettings.CustomCameraPosition) HintCamera.transform.localPosition = levelSettings.CameraPosition;

        HintCamera.gameObject.SetActive(false);
    }
}
