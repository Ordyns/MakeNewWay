using System.Collections.Generic;
using UnityEngine;

public class HintIslandFactory
{
    private HintRenderer _hintRenderer;

    private List<Transform> _hintIslands;
    private List<Transform> _originalIslands;
    private IslandsProvider _islandsProvider;

    public HintIslandFactory(HintRenderer hintRenderer, IslandsProvider islandsProvider){
        _hintRenderer = hintRenderer;
        _islandsProvider = islandsProvider;
    }

    public List<Transform> GetHintIslands(){
        if(_hintIslands != null)
            return _hintIslands;

        _hintIslands = new List<Transform>();
        _originalIslands = IslandsProvider.GetIslandsTransforms(_islandsProvider.Islands);

        foreach (Transform island in _originalIslands)
            _hintIslands.Add(GetIsland(island));

        return _hintIslands;
    }

    public List<Transform> GetOriginalIslands() => new List<Transform>(_originalIslands);

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
