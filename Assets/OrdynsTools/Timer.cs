using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float _duration;
    private System.Action<float> _onTickCallback;
    private System.Action _onCompleteCallback;

    public bool isActive {get; private set;}
    private float _remainingTime;

    public void Init(float duration, System.Action<float> onTickCallback, System.Action onComplete){
        _duration = _remainingTime = duration;
        _onTickCallback = onTickCallback;
        _onCompleteCallback = onComplete;
        isActive = true;
    }

    public void Tick() {
        if(isActive){
            if(_remainingTime > 0){
                _remainingTime -= Time.deltaTime;
                if(_remainingTime < 0) _remainingTime = 0;
                _onTickCallback?.Invoke(_remainingTime);
            }
            else{
                Stop(true);
            }
        }
    }

    public void SetPause(bool pause) => isActive = !pause;

    public void Stop(bool invokeOnCompleteCallback = false){
        isActive = false;
        if(invokeOnCompleteCallback) _onCompleteCallback?.Invoke();
    }
}
