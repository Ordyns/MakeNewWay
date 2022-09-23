using UnityEngine;

public static class ParticleSystemExtensions
{
    public static void ChangeParticleSystemColor(this ParticleSystem particleSystem, Color color){
        ParticleSystem.MainModule main = particleSystem.main;
        main.startColor = color;
    }
}