using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list){
        int count = list.Count;
        while (count > 1) {
            count--;
            int randomIndex = Random.Range(0, count + 1);
            T element = list[randomIndex];
            list[randomIndex] = list[count];
            list[count] = element;
        }  
    }
}