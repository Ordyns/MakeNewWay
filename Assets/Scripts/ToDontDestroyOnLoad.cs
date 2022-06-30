using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ToDontDestroyOnLoad : MonoBehaviour
{
    private void Start() {
        for(int i = 0; transform.childCount > 0; i++){
            Transform child = transform.GetChild(0);
            child.parent = null;
            DontDestroyOnLoad(child.gameObject);
        }
    }
}
