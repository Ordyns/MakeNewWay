using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(CameraConstantWidth), typeof(CameraAnimator))]
public class BaseCamera : MonoBehaviour
{
    [HideInInspector] public Camera Camera;

    [SerializeField] private GuideSystem guideSystem;

    private CameraAnimator _cameraAnimator;
    private CameraConstantWidth _cameraConstantWidth;

    private void Awake() {
        Camera = GetComponent<Camera>();
        _cameraAnimator = GetComponent<CameraAnimator>();
        _cameraConstantWidth = GetComponent<CameraConstantWidth>();

        guideSystem.GuideFinished += () => StartCoroutine(Init());
        LevelContext.Instance.PathChecker.PathChecked += (isPathCorrect) => {
            if(isPathCorrect){
                _cameraAnimator.PlayOutAnimation();
            }
        };
    }

    private IEnumerator Init(){
        yield return SetupCameraComponents();

        _cameraAnimator.PlayInAnimation();
    }

    private IEnumerator SetupCameraComponents() {
        while(LevelContext.Instance == null)
            yield return null;

        LevelSettings levelSettings = LevelContext.Instance.LevelSettings;

        float cameraOriginalSize = levelSettings.CameraSize;
        Camera.orthographicSize = _cameraConstantWidth.GetConstSize(cameraOriginalSize + _cameraAnimator.SizeOffset);
        _cameraAnimator.SetOriginalSize(_cameraConstantWidth.GetConstSize(cameraOriginalSize));

        if(levelSettings.CustomCameraPosition)
            Camera.transform.position = levelSettings.CameraPosition;
    }
}
