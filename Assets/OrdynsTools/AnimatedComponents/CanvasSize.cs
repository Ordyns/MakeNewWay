using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasSize : MonoBehaviour
{
    public static CanvasSize Instance;
    public static Vector2 Size {get; private set;}

    private void Start() {
        Instance = this;
        Size = GetComponent<RectTransform>().rect.size;
    }
}
