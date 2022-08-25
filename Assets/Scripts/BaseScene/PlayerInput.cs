using System;
using System.Collections;
using UnityEngine;

public class PlayerInput<T> where T: class
{
    public bool IsInputAllowed = true;

    public event System.Action<T, Direction> Swipe;
    public event System.Action<T> Click;

    private T _currentObject;
    private Camera _camera;

    private bool isDraging;

    private Vector2 _startTouch;
    private Vector2 _swipeDelta;

    public PlayerInput(Camera camera){
        _camera = camera;
    }

    public void Update() {
        if(_camera.gameObject.activeSelf == false)
            return;

        if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)){
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)){
                hit.transform.TryGetComponent<T>(out _currentObject);
            }
        }

        if(_currentObject == null)
            return;

        CheckMobileDraging();
        CheckStandaloneDraging();

        if (isDraging){
            _swipeDelta = Vector2.zero;

            if(Input.touchCount > 0) _swipeDelta = Input.touches[0].position - _startTouch;
            else if(Input.GetMouseButton(0)) _swipeDelta = (Vector2)Input.mousePosition - _startTouch;

            if(_swipeDelta.magnitude > 35){
                float x = _swipeDelta.x;
                float y = _swipeDelta.y;

                Direction swipeDirection = new Direction();

                if(x > 0){
                    if(y < 0) swipeDirection = Direction.DownRight;
                    else swipeDirection = Direction.UpperRight;
                }
                else{
                    if(y < 0) swipeDirection = Direction.DownLeft;
                    else swipeDirection = Direction.UpperLeft;
                }
                
                Swipe?.Invoke(_currentObject, swipeDirection);

                Reset();
            }
        }
        else{
            if(_currentObject != null && Input.GetMouseButtonUp(0))
                Click?.Invoke(_currentObject);

            _currentObject = null;
        }
    }

    private void CheckMobileDraging(){
        if(Input.touchCount == 0)
            return;

        if(Input.touches[0].phase == TouchPhase.Began){
            isDraging = true;
            _startTouch = Input.touches[0].position;
        }
        else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled){
            isDraging = false;
            Reset();
        }
    }

    private void CheckStandaloneDraging(){
        if(Input.GetMouseButtonDown(0)){
            isDraging = true;
            _startTouch = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0)){
            isDraging = false;
            Reset();
        }
    }

    private void Reset() {
        _startTouch = _swipeDelta = Vector2.zero;
        isDraging = false;
    }
}
