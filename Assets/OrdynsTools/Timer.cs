using System;
using System.Collections;
using UnityEngine;

public class Timer : IDisposable
{
    public float Duration { get; }
    public bool IsRunning { get; private set; }

    public event Action Completed;

    private MonoBehaviour _targetMonoBehaviour;
    private Coroutine _timerRoutine;

    public Timer(MonoBehaviour monoBehaviour, float duration){
        Duration = duration;
        _targetMonoBehaviour = monoBehaviour;
    }

    public void Stop(){
        IsRunning = false;
        _targetMonoBehaviour.StopCoroutine(_timerRoutine);
    } 

    public void Start(){
        IsRunning = true;
        _timerRoutine = _targetMonoBehaviour.StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine(){
        yield return new WaitForSecondsRealtime(Duration);
        IsRunning = false;
        Completed?.Invoke();
    }

    public static Timer StartNew(MonoBehaviour monoBehaviour, float duration, System.Action onCompleted){
        Timer timer = new Timer(monoBehaviour, duration);
        timer.Completed += onCompleted;
        timer.Start();
        return timer;
    }

    public void Dispose(){
        if(_timerRoutine != null){
            _targetMonoBehaviour.StopCoroutine(_timerRoutine);
            _timerRoutine = null;
        }

        _targetMonoBehaviour = null;
        Completed = null;
    }
}