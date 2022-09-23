using System;
using UnityEngine;

public abstract class TutorialStep : MonoBehaviour
{
    public event Action Completed;

    [field:SerializeField] public float StartDelay { get; private set; }
    
    public bool RequiresIslandsUpdater { get; protected set; }
    public bool IsActive { get; protected set; }

    private RectTransform _canvas;

    public virtual void Enter(RectTransform canvas){
        IsActive = true;
        _canvas = canvas;
    }

    public virtual void Exit(){
        IsActive = false;
        Completed = null;
    }

    protected void Complete(){
        Completed?.Invoke();
    }

    protected Vector2 WorldToCanvasPoint(Vector3 position){
        Vector2 adjustedPosition = Camera.main.WorldToScreenPoint(position);
        adjustedPosition.x *= _canvas.rect.width / (float)Camera.main.pixelWidth;
        adjustedPosition.y *= _canvas.rect.height / (float)Camera.main.pixelHeight;

        return adjustedPosition - _canvas.sizeDelta / 2f;
    }
}
