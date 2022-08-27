using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AudioPlayer : MonoBehaviour
{
    private static AudioPlayer _instance;


    [Header("Sounds")]
    [SerializeField] private AudioSource source;
    [SerializeField] [MinMaxSlider(-3, 3)] private Vector2 pitchRange = Vector2.one;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] music;
    [Space]
    [SerializeField] private float nextMusicDelay;

    private bool isMusicEnabled => ProjectContext.Instance.SaveSystem.Data.isMusicEnabled;
    private bool isSoundsEnabled => ProjectContext.Instance.SaveSystem.Data.isSoundsEnabled;

    private List<AudioClip> _playlist = new List<AudioClip>();

    private Coroutine _musicRoutine;

    private void Awake(){
        _instance = this;
    }

    private void Start() {
        _musicRoutine = StartCoroutine(MusicRoutine());
    }

    private IEnumerator MusicRoutine(){
        _playlist = new List<AudioClip>(music);
        _playlist.Shuffle();

        while(isMusicEnabled){
            for(int i = 0; i < _playlist.Count; i++){
                yield return new WaitForSecondsRealtime(nextMusicDelay);
                musicSource.PlayOneShot(_playlist[i]);
                yield return new WaitForSecondsRealtime(_playlist[i].length);
            }

            yield return null;
        }
    }

    public static void PlayRandomClip(AudioClip[] clips){
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlayClip(clip);
    }

    public static void StopMusic(){
        if(_instance._musicRoutine != null){
            _instance.StopCoroutine(_instance._musicRoutine);
        }

        _instance.musicSource.Stop();
    } 

    public static void PlayMusic(){
        if(_instance._musicRoutine != null)
            StopMusic();

        _instance._musicRoutine = _instance.StartCoroutine(_instance.MusicRoutine());
    } 

    public static void PlayClip(AudioClip clip){
        if(AudioPlayer._instance.isSoundsEnabled == false)
            return;

        _instance.source.pitch = Random.Range(_instance.pitchRange.x, _instance.pitchRange.y);
        _instance.source.PlayOneShot(clip);
    }

    public static void PlayClip(AudioClip clip, float delay){
        TimeOperations.CreateTimer(delay, null, () => PlayClip(clip));
    }
}