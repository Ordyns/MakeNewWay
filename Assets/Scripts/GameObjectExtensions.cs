using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetLayerForChildren(this GameObject parent, LayerMask layer){
        if(parent == null)
            return;

        parent.layer = layer;

        for (int i = 0; i < parent.transform.childCount; i++)
            parent.transform.GetChild(i).gameObject.SetLayerForChildren(layer);
    }
}
