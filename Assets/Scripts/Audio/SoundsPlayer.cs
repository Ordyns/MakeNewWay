using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(AudioSource))]
public abstract class SoundsPlayer : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] [MinMaxSlider(-3, 3)] private Vector2 pitchRange = Vector2.one;
    [Space]
    public bool IsEnabled = true;

    private AudioSource _audioSource;

    private void OnValidate() {
        _audioSource = GetComponent<AudioSource>();
    }

    protected void PlayRandomClip(AudioClip[] clips, float delay = 0){
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlayClip(clip, delay);
    }

    protected void PlayClip(AudioClip clip, float delay){
        if(delay == 0){
            PlayClip(clip);
            return;
        }

        Timer.StartNew(this, delay, () => PlayClip(clip));
    }

    protected void PlayClip(AudioClip clip){
        if(IsEnabled == false)
            return;

        _audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        _audioSource.PlayOneShot(clip);
    }
}