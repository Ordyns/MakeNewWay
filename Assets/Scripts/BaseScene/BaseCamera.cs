using UnityEngine;

[RequireComponent(typeof(Camera), typeof(CameraConstantWidth), typeof(CameraAnimator))]
public class BaseCamera : MonoBehaviour
{
    public Camera Camera { get; private set; }
    public float AnimationDuration => _cameraAnimator.Duration;

    private CameraAnimator _cameraAnimator;
    private CameraConstantWidth _cameraConstantWidth;

    private void OnValidate(){
        Camera = GetComponent<Camera>();
        _cameraAnimator = GetComponent<CameraAnimator>();
        _cameraConstantWidth = GetComponent<CameraConstantWidth>();
    }

    [Zenject.Inject]
    private void Init(LevelSettings levelSettings){
        float cameraOriginalSize = levelSettings.CameraSize;
        float constantWidthSize = _cameraConstantWidth.GetConstSize(cameraOriginalSize + _cameraAnimator.SizeOffset);;
        Camera.orthographicSize = constantWidthSize;
        _cameraAnimator.SetOriginalSize(_cameraConstantWidth.GetConstSize(cameraOriginalSize));

        if(levelSettings.CustomCameraPosition)
            Camera.transform.position = levelSettings.CameraPosition;
    }

    public void PlayOutAnimation() => _cameraAnimator.PlayOutAnimation();
    public void PlayInAnimation() => _cameraAnimator.PlayInAnimation();
}
