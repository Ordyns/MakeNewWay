using UnityEngine;

public struct FourDirectionalTextureAsset
{
    public Texture UpperLeft;
    public Texture UpperRight;
    public Texture DownLeft;
    public Texture DownRight;

    private readonly string _resourcesFolder;

    public FourDirectionalTextureAsset(string folderName, string resourcesFolder = "Editor/Islands"){
        _resourcesFolder = resourcesFolder;

        UpperLeft = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/upper_left", typeof(Texture));
        UpperRight = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/upper_right", typeof(Texture));
        DownLeft = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/down_left", typeof(Texture));
        DownRight = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/down_right", typeof(Texture));
    }

    public Texture GetTextureByDirection(Direction direction){
        switch(direction){
            case Direction.UpperLeft: return UpperLeft;
            case Direction.UpperRight: return UpperRight;
            case Direction.DownLeft: return DownLeft;
            case Direction.DownRight: return DownRight;
        }

        return null;
    }

    private void LoadTextures(string folderName){
        UpperLeft = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/upper_left", typeof(Texture));
        UpperRight = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/upper_right", typeof(Texture));
        DownLeft = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/down_left", typeof(Texture));
        DownRight = (Texture) Resources.Load($"{_resourcesFolder}/{folderName}/down_right", typeof(Texture));
    }
}
