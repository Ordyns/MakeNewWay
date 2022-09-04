using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(CameraConstantWidth), typeof(CameraAnimator))]
public class BaseCamera : MonoBehaviour
{
    [HideInInspector] public Camera Camera;

    private CameraAnimator _cameraAnimator;
    private CameraConstantWidth _cameraConstantWidth;

    private void OnValidate(){
        Camera = GetComponent<Camera>();
        _cameraAnimator = GetComponent<CameraAnimator>();
        _cameraConstantWidth = GetComponent<CameraConstantWidth>();
    }

    public void Init(){
        LevelSettings levelSettings = LevelContext.Instance.LevelSettings;

        float cameraOriginalSize = levelSettings.CameraSize;
        Camera.orthographicSize = _cameraConstantWidth.GetConstSize(cameraOriginalSize + _cameraAnimator.SizeOffset);
        _cameraAnimator.SetOriginalSize(_cameraConstantWidth.GetConstSize(cameraOriginalSize));

        if(levelSettings.CustomCameraPosition)
            Camera.transform.position = levelSettings.CameraPosition;
    }

    public void PlayOutAnimation() => _cameraAnimator.PlayOutAnimation();
    public void PlayInAnimation() => _cameraAnimator.PlayInAnimation();
}
