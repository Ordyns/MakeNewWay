using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] music;
    [Space]
    [SerializeField] private float nextMusicDelay;

    private bool isMusicEnabled = true;

    private List<AudioClip> _playlist = new List<AudioClip>();

    private Coroutine _musicRoutine;

    private void Start() {
        _musicRoutine = StartCoroutine(PlayMusicCoroutine());
    }

    private IEnumerator PlayMusicCoroutine(){
        _playlist = new List<AudioClip>(music);
        _playlist.Shuffle();
        yield return new WaitForSecondsRealtime(nextMusicDelay);

        while(isMusicEnabled){
            for(int i = 0; i < _playlist.Count; i++){
                musicSource.PlayOneShot(_playlist[i]);
                yield return new WaitForSecondsRealtime(_playlist[i].length + nextMusicDelay);
            }

            yield return null;
        }
    }

    public void PlayMusic(){
        if(_musicRoutine != null)
            StopMusic();

        isMusicEnabled = true;

        if(gameObject.activeInHierarchy)
            _musicRoutine = StartCoroutine(PlayMusicCoroutine());
    }

    public void StopMusic(){
        if(_musicRoutine != null)
            StopCoroutine(_musicRoutine);

        isMusicEnabled = false;
        musicSource.Stop();
    }
}
