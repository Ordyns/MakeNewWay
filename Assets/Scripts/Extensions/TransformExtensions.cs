using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class TransformExtensions
{
    public static List<T> GetAllChildrenWithComponent<T>(this Transform parent, bool includingInactive = true){
        LinkedList<T> children = new LinkedList<T>();
        for(int i = 0; i < parent.childCount; i++){
            var child = parent.GetChild(i);
            
            if(includingInactive == false && child.gameObject.activeSelf == false)
                continue;
            
            if(child.TryGetComponent<T>(out T component))
                children.AddLast(component);
            
            foreach(T island in child.GetAllChildrenWithComponent<T>(includingInactive))
                children.AddLast(island);
        }

        return children.ToList();
    }
}