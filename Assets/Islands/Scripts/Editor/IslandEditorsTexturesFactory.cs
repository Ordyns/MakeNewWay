using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandEditorsTexturesFactory
{
    private FourDirectionalTextureAsset _cornerMirroredIslandTextureAsset;

    Dictionary<Island.IslandType, FourDirectionalTextureAsset> _assetsDictionary = new Dictionary<Island.IslandType, FourDirectionalTextureAsset>();

    public IslandEditorsTexturesFactory(){
        _cornerMirroredIslandTextureAsset = new FourDirectionalTextureAsset("MirroredCorner");

        foreach(Island.IslandType islandType in System.Enum.GetValues(typeof(Island.IslandType))){
            _assetsDictionary.Add(islandType, new FourDirectionalTextureAsset(islandType.ToString()));
        }
    }

    public FourDirectionalTextureAsset GetTextureByIslandType(Island.IslandType islandType) => _assetsDictionary[islandType];

    public FourDirectionalTextureAsset GetMirroredCornerTextureAsset() => _cornerMirroredIslandTextureAsset;
}
