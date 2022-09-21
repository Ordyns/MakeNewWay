public class LoadLevelSignal
{
    public LoadLevelSignal(int levelNumber) => LevelNumber = levelNumber;

    private int _levelNumber;
    public int LevelNumber { 
        get => _levelNumber;
        set {
            if(value < 1)
                throw new System.Exception("Level number can't be less than one");

            _levelNumber = value;
        }
    }
}
