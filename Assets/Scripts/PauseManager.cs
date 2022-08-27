using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : IPauseHandler
{
    private List<IPauseHandler> _pauseHandlers = new List<IPauseHandler>();

    public bool IsPaused { get; private set; }

    public void Subscribe(IPauseHandler handler) => _pauseHandlers.Add(handler);
    public void Unsubscribe(IPauseHandler handler) => _pauseHandlers.Remove(handler);

    public void SetPaused(bool isPaused){
        IsPaused = isPaused;

        for(int i = 0; i < _pauseHandlers.Count; i++)
            _pauseHandlers[i].SetPaused(isPaused);
    }
}

public interface IPauseHandler
{
    void SetPaused(bool isPaused);
}
