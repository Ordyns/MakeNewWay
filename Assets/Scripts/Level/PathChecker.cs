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
        _startIsland = _islands.Find(island => island.Type == Island.IslandType.Start);
    }

    public void CheckPath(){
        Island currentIsland = _startIsland;
        HashSet<Island> islandsWithoutEnergy = new HashSet<Island>(_islands);

        while(currentIsland != null){
            if(currentIsland.TryGetNextIsland(out Island nextIsland)){
                if(nextIsland.IsEnergyIsland == false)
                    break;
                
                if(Island.IsInputAndOutputCorrespond(nextIsland.GetInputDirection(), currentIsland.GetOutputDirection()) == false)
                    break;

                islandsWithoutEnergy.Remove(nextIsland);
                nextIsland.AcivateEnergy();

                if(nextIsland.Type == Island.IslandType.Finish){
                    _signalBus.Fire<LevelCompletedSignal>();
                    break;
                }

                currentIsland = nextIsland;
            }
            else 
                break;
        }

        foreach(Island island in islandsWithoutEnergy)
            island.DeactivateEnergy();
    }
}
