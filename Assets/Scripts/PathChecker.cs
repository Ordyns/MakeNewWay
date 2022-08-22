using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class PathChecker : MonoBehaviour
{
    public delegate void PathCheckedAction(bool pathCorrect);
    public event PathCheckedAction PathChecked;

    private Island _startIsland;
    private List<Island> _islands;
    
    private void Start() {
        _islands = LevelContext.Instance.IslandsContainer.Islands;
        _startIsland = _islands.Find(island => island.IslandType == Island.IslandTypes.Start);
        CheckPath();
    }

    public void CheckPath() => PathChecked?.Invoke(isPathCorrect());
    public void ChechPathWithoutEvent() => isPathCorrect();

    private bool isPathCorrect(){
        Island currentIsland = _startIsland;
        List<Island> passedIslands = new List<Island>();

        bool tryingToCheckPath = true;
        while(tryingToCheckPath){
            if(currentIsland.TryGetNextIsland(out Island island)){
                island.EnergyIsGoing();
                passedIslands.Add(island);
                currentIsland = island;

                if(island.IslandType == Island.IslandTypes.Finish)
                    return true;
            }
            else{
                List<Island> islandsWithoutEnergy = _islands.Except(passedIslands).ToList();
                for(int j = 0; j < islandsWithoutEnergy.Count; j++){
                    if(islandsWithoutEnergy[j].IslandType == Island.IslandTypes.Start)
                        islandsWithoutEnergy[j].EnergyIsGoing();
                    else
                        islandsWithoutEnergy[j].EnergyIsNotGoing();
                }

                tryingToCheckPath = false;
            } 
        }
        
        return false;
    }
}
