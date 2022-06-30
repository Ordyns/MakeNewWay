using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Extensions
{
    public static List<T> GetAllChildrenWithComponent<T>(this Transform parent, bool includingInactive = true){
        List<T> children = new List<T>();
        for(int i = 0; i < parent.childCount; i++){
            var child = parent.GetChild(i);
            
            if(includingInactive == false && child.gameObject.activeSelf == false)
                continue;
            
            if(child.TryGetComponent<T>(out T component))
                children.Add(component);
            
            children.AddRange(child.GetAllChildrenWithComponent<T>());
        }

        return children;
    }

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

    public static void ChangeParticleSystemColor(this ParticleSystem particleSystem, Color color){
        ParticleSystem.MainModule main = particleSystem.main;
        main.startColor = color;
    }

    public static Vector2 ComputeTotalImpulse2D(Collision2D collision) {
        Vector2 impulse = Vector2.zero;
        int contactCount = collision.contactCount;

        for(int i = 0; i < contactCount; i++) {
            var contact = collision.GetContact(0);
            impulse += contact.normal * contact.normalImpulse;
            impulse.x += contact.tangentImpulse * contact.normal.y;
            impulse.y -= contact.tangentImpulse * contact.normal.x;
        }

        return impulse;
    }

    public static T DeepCopy<T>(T item) {
        BinaryFormatter formatter = new BinaryFormatter();
        using(MemoryStream stream = new MemoryStream()){
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }
    }
}
