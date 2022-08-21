using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(CameraConstantWidth), typeof(CameraAnimator))]
public class BaseCamera : MonoBehaviour
{
    public static BaseCamera Instance;

    [HideInInspector] public Camera Camera;
    private CameraAnimator _cameraAnimator;
    private CameraConstantWidth _cameraConstantWidth;

    private void Awake() {
        Instance = this;

        Camera = GetComponent<Camera>();
        _cameraAnimator = GetComponent<CameraAnimator>();
        _cameraConstantWidth = GetComponent<CameraConstantWidth>();
    }

    private void Start() {
        LevelSettings levelSettings = LevelSettings.Instance;

        float cameraOriginalSize = levelSettings.CameraSize;
        Camera.orthographicSize = _cameraConstantWidth.GetConstSize(cameraOriginalSize + _cameraAnimator.SizeOffset);
        _cameraAnimator.SetOriginalSize(_cameraConstantWidth.GetConstSize(cameraOriginalSize));

        if(levelSettings.CustomCameraPosition)
            Camera.transform.position = levelSettings.CameraPosition;

        if(GuideSystem.Instance.isGuideShowing)
            GuideSystem.Instance.GuideFinished += _cameraAnimator.PlayInAnimation;
        else
            _cameraAnimator.PlayInAnimation();
    }
}
