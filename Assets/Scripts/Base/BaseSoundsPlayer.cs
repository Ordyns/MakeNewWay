using UnityEngine;

public class BaseSoundsPlayer : SoundsPlayer
{
    [Header("Sounds")]
    [SerializeField] private AudioClip[] swipeSounds;
    [SerializeField] private AudioClip levelCompletedSound;

    public void PlaySwipeSound(float delay = 0){
        PlayRandomClip(swipeSounds, delay);
    }

    public void PlayLevelCompletedSound(float delay = 0){
        PlayClip(levelCompletedSound, delay);
    }
}
