using System.Collections.Generic;
using UnityEngine;

public class HintIslandFactory
{
    private HintRenderer _hintRenderer;

    public HintIslandFactory(HintRenderer hintRenderer){
        _hintRenderer = hintRenderer;
    }

    public List<Transform> GetHintIslands(List<Transform> originalIslands){
        List<Transform> hintIslands = new List<Transform>();

        foreach (Transform island in originalIslands)
            hintIslands.Add(GetIsland(island));

        return hintIslands;
    }

    public Transform GetIsland(Transform originalIsland){
        Transform hintIsland = MonoBehaviour.Instantiate(originalIsland.gameObject, _hintRenderer.HintIslandsParent).transform;
        hintIsland.gameObject.SetLayerForChildren(HintRenderer.HintLayer);
        DestoryComponents(hintIsland);
        
        return hintIsland;
    }

    private void DestoryComponents(Transform island){
        if(island.TryGetComponent<Island>(out Island islandComponent))
            MonoBehaviour.Destroy(islandComponent);
        else if(island.TryGetComponent<ComplexIsland>(out ComplexIsland complexIslandComponent))
            MonoBehaviour.Destroy(complexIslandComponent);
    }
}
