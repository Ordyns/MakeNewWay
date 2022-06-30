using System.Collections.Generic;
using UnityEngine;

public class LevelsContainer : MonoBehaviour
{
    public static LevelsContainer Instance;

    public int LevelsCount;
    public List<int> NumbersOfLevelsWithBonus;

    private void Awake() => Instance = this;
}
