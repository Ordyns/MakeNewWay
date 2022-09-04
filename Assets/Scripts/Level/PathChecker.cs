using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathChecker : MonoBehaviour
{
    public delegate void PathCheckedAction(bool pathCorrect);
    public event PathCheckedAction PathChecked;

    private Island _startIsland;
    private List<Island> _islands;

    public void Init(List<Island> islands){
        _islands = islands;
        _startIsland = _islands.Find(island => island.IslandType == Island.IslandTypes.Start);
        CheckPath();
    }
    
    public void CheckPath(){
        bool isPathCorrect = IsPathCorrect();
        PathChecked?.Invoke(isPathCorrect);
    } 

    public void ChechPathWithoutEvent(){
        IsPathCorrect();
    }

    private bool IsPathCorrect(){
        Island currentIsland = _startIsland;
        List<Island> passedIslands = new List<Island>();

        bool isPathChecked = false;
        while(isPathChecked == false){
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

                isPathChecked = true;
            } 
        }
        return false;
    }
}
