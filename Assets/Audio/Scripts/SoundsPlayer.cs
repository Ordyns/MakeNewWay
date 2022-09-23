using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(AudioSource))]
public abstract class SoundsPlayer : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] [MinMaxSlider(-3, 3)] private Vector2 pitchRange = Vector2.one;
    
    public bool IsSoundsEnabled{
        get => isEnabled;
        set{
            isEnabled = value;
            UpdateVolume();
        }
    }

    private bool isEnabled;

    private const float _defaultVolume = 1f;

    private AudioSource _audioSource;

    private Zenject.SignalBus _signalBus;

    private void OnValidate() {
        _audioSource = GetComponent<AudioSource>();
    }

    [Zenject.Inject]
    private void Init(Zenject.SignalBus signalBus){
        _signalBus = signalBus;
        _signalBus.Fire(new ObjectCreatedSignal<SoundsPlayer>(this));
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
        if(IsSoundsEnabled == false)
            return;

        _audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        _audioSource.PlayOneShot(clip);
    }

    private void UpdateVolume(){
        _audioSource.volume = isEnabled ? _defaultVolume : 0;
    }

    private void OnDestroy() {
        _signalBus.Fire(new ObjectDestroyedSignal<SoundsPlayer>(this));
    }
}