using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOperations : MonoBehaviour
{
    public static TimeOperations Instance;

    public event System.Action OnTimeStopped;
    public event System.Action OnTimeUnstopped;

    private AnimationCurve _curve;

    private bool _changeTimeByCurve;
    private float _curveTimeLeft;

    private const float DEFAULT_TIME_SCALE = 1f;
    private const float DEFAULT_FIXED_DELTA_TIME = 0.02f;

    private List<Timer> _timers;

    private void Awake() {
        Instance = this;
        _timers = new List<Timer>();
    }

    public static void StopTime(){
        Instance._changeTimeByCurve = false;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;

        Instance.OnTimeStopped?.Invoke();
    }

    public static void UnstopTime(){
        Instance._changeTimeByCurve = false;
        Time.timeScale = DEFAULT_TIME_SCALE;
        Time.fixedDeltaTime = DEFAULT_FIXED_DELTA_TIME;
        
        Instance.OnTimeUnstopped?.Invoke();
    }

    public void ChangeTimeByCurve(AnimationCurve curve){
        _curve = curve;
        _changeTimeByCurve = true;
        _curveTimeLeft = 0;
    }

    private void Update() {
        if(_changeTimeByCurve){
            if(_curveTimeLeft < 1){
                _curveTimeLeft += Time.deltaTime;
                float value = _curve.Evaluate(_curveTimeLeft);

                Time.timeScale = Mathf.Lerp(DEFAULT_TIME_SCALE, 0, value);
                Time.fixedDeltaTime = Mathf.Lerp(DEFAULT_FIXED_DELTA_TIME, 0, value);
            }
        }

        for(int i = 0; i < _timers.Count; i++){
            _timers[i].Tick();
            if(!_timers[i].isActive) _timers.RemoveAt(i);
        }
    }

    public static Timer CreateTimer(float duration, System.Action<float> onTickCallback, System.Action onComplete){
        Timer timer = new Timer();
        Instance._timers.Add(timer);
        timer.Init(duration, onTickCallback, onComplete);
        return timer;
    }
}
