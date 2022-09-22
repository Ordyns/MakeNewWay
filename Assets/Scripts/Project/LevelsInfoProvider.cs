using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LevelsInfoProvider : MonoBehaviour
{
    [field:SerializeField] public int LevelsCount { get; private set; }
    [SerializeField] private List<int> numbersOfLevelsWithBonus;
    
    public ReadOnlyCollection<int> NumbersOfLevelsWithBonus => numbersOfLevelsWithBonus.AsReadOnly();
}