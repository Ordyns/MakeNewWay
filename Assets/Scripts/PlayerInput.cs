using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;

    [HideInInspector] public bool isIslandUpdating;
    [HideInInspector] public bool isInputAllowed = true;
    
    [HideInInspector] public int StepsLeft;

    public event System.Action<Island> OnIslandUpdating;
    public event System.Action OnStepsCountChanged;

    private bool isGameEnded;
    private bool isDraging;

    private Vector2 _startTouch;
    private Vector2 _swipeDelta;
    private Island _currentIsland;
    
    private Camera _mainCamera;

    private void Awake() => Instance = this;

    private IEnumerator Start(){
        PathChecker.Instance.OnPathChecked += CheckEnded;

        if(BaseCamera.Instance)
            _mainCamera = BaseCamera.Instance.Camera;
        else
            _mainCamera = Camera.main;

        while(LevelSettings.Instance == null)
            yield return null;

        StepsLeft = LevelSettings.Instance.Steps;
    }

    private void Update() {
        if(GuideSystem.Instance && GuideSystem.Instance.isGuideShowing)
            return;

        if(isInputAllowed == false || isIslandUpdating || isGameEnded)
            return;

        if(_mainCamera.gameObject.activeSelf == false)
            return;

        CheckMobileInput();
        CheckStandaloneInput();

        if(Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)){
            if(hit.transform.TryGetComponent<Island>(out Island island))
                _currentIsland = island;
            else 
                return;
        }

        if(_currentIsland == null)
            return;

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

                _currentIsland.OnSwipe(swipeDirection);

                Reset();
            }
        }
        else{
            if(_currentIsland != null && Input.GetMouseButtonUp(0)){
                _currentIsland.OnClick();
            }

            _currentIsland = null;
        }
    }

    public void AddStep(){
        StepsLeft++;
        OnStepsCountChanged?.Invoke();
    } 
    public void SubtractStep(){
        StepsLeft--;
        OnStepsCountChanged?.Invoke();
    } 

    public void IslandUpdating(){
        SubtractStep();

        isIslandUpdating = true;
        OnIslandUpdating?.Invoke(_currentIsland);
    }

    public static void IslandUpdatingFinished() => Instance.isIslandUpdating = false;

    public void CheckEnded(bool pathCorrect){
        isIslandUpdating = false;

        if(isGameEnded)
            return;

        if(pathCorrect || StepsLeft == 0)
            isGameEnded = true;
    }

    void CheckMobileInput(){
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

    void CheckStandaloneInput(){
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
