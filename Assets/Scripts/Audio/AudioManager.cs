using System.Collections.Generic;

public class AudioManager
{
    private MusicPlayer _musicPlayer;
    private bool isMusicEnabled;

    private LinkedList<SoundsPlayer> _soundsPlayers;
    private bool isSoundsEnabled;

    private Zenject.SignalBus _signalBus;

    public AudioManager(Zenject.SignalBus signalBus, MusicPlayer musicPlayer){
        _signalBus = signalBus;

        _musicPlayer = musicPlayer;
        _soundsPlayers = new LinkedList<SoundsPlayer>();

        InitSignals();
    }

    private void InitSignals(){
        _signalBus.Subscribe<Settings.IsMusicEnabledChangedSignal>(OnIsMusicEnabledChanged);
        _signalBus.Subscribe<Settings.IsSoundsEnabledChangedSignal>(OnIsSoundsEnabledChanged);

        _signalBus.DeclareSignal<ObjectCreatedSignal<SoundsPlayer>>();
        _signalBus.Subscribe<ObjectCreatedSignal<SoundsPlayer>>(AddSoundsPlayer);

        _signalBus.DeclareSignal<ObjectDestroyedSignal<SoundsPlayer>>();
        _signalBus.Subscribe<ObjectDestroyedSignal<SoundsPlayer>>(RemoveSoundsPlayer);
    }
    
    public void OnIsMusicEnabledChanged(Settings.IsMusicEnabledChangedSignal signal){
        isMusicEnabled = signal.IsEnabled;

        if(signal.IsEnabled)
            _musicPlayer.PlayMusic();
        else
            _musicPlayer.StopMusic();
    }

    public void OnIsSoundsEnabledChanged(Settings.IsSoundsEnabledChangedSignal signal){
        isSoundsEnabled = signal.IsEnabled;
        UpdateSoundsPlayers();
    }

    private void AddSoundsPlayer(ObjectCreatedSignal<SoundsPlayer> signal){
        signal.Object.IsSoundsEnabled = isSoundsEnabled;
        _soundsPlayers.AddLast(signal.Object);
    }

    private void RemoveSoundsPlayer(ObjectDestroyedSignal<SoundsPlayer> signal){
        _soundsPlayers.Remove(signal.Object);
    }

    private void UpdateSoundsPlayers(){
        var current = _soundsPlayers.First;
        while(current != null){
            if(current == null)
                _soundsPlayers.Remove(current);

            current.Value.IsSoundsEnabled = isSoundsEnabled;

            current = current.Next;
        }
    }
}