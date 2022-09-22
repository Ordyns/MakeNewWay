using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static List<T> GetAllChildrenWithComponent<T>(this Transform parent, bool includingInactive = true){
        List<T> children = new List<T>();
        for(int i = 0; i < parent.childCount; i++){
            var child = parent.GetChild(i);
            
            if(includingInactive == false && child.gameObject.activeSelf == false)
                continue;
            
            if(child.TryGetComponent<T>(out T component))
                children.Add(component);
            
            children.AddRange(child.GetAllChildrenWithComponent<T>());
        }

        return children;
    }
}