using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LevelsContainer : MonoBehaviour
{
    [field:SerializeField] public int LevelsCount { get; private set; }
    [SerializeField] private List<int> numbersOfLevelsWithBonus;
    
    public ReadOnlyCollection<int> NumbersOfLevelsWithBonus { get; private set; }

    private void Awake(){
        NumbersOfLevelsWithBonus = numbersOfLevelsWithBonus.AsReadOnly();
    } 
}
