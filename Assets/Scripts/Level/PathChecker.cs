using System.Collections.Generic;

public class PathChecker
{
    private Island _startIsland;
    private List<Island> _islands;

    private Zenject.SignalBus _signalBus;

    [Zenject.Inject]
    private void Init(Zenject.SignalBus signalBus, IslandsProvider islandsProvider){
        _signalBus = signalBus;

        _islands = islandsProvider.Islands;
        _startIsland = _islands.Find(island => island.IslandType == Island.IslandTypes.Start);
    }

    private void Start() {
        CheckPath();
    }

    public void CheckPath(){
        Island currentIsland = _startIsland;
        HashSet<Island> islandsWithoutEnergy = new HashSet<Island>(_islands);

        while(currentIsland != null){
            if(currentIsland.TryGetNextIsland(out Island nextIsland)){
                nextIsland.EnergyIsGoing();;
                islandsWithoutEnergy.Remove(nextIsland);

                if(nextIsland.IslandType == Island.IslandTypes.Finish){
                    _signalBus.Fire<LevelCompletedSignal>();
                    break;
                }

                currentIsland = nextIsland;
            }
            else{
                foreach(Island island in islandsWithoutEnergy){
                    if(island.IslandType == Island.IslandTypes.Start)
                        island.EnergyIsGoing();
                    else
                        island.EnergyIsNotGoing();
                }

                break;
            } 
        }
    }
}
