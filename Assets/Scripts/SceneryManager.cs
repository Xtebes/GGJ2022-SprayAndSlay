using UnityEngine;
using DG.Tweening;
public class SceneryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hellSource, normalSource;
    [SerializeField]
    private AudioClip hellSoundTrack, normalSoundTrack;
    [SerializeField]
    private AudioSource soundTrackSource;
    private float soundTrackAudioVolume;
    private void OnFadeMid(Game.mState state)
    {
        float soundTime = soundTrackSource.time;
        if (state == Game.mState.sane)
        {
            hellSource.gameObject.SetActive(false);
            normalSource.gameObject.SetActive(true);
            soundTrackSource.clip = normalSoundTrack;

        }
        else
        {
            hellSource.gameObject.SetActive(true);
            normalSource.gameObject.SetActive(false);
            soundTrackSource.clip = hellSoundTrack;
        }
        if (soundTime > soundTrackSource.clip.length) soundTime = 0;
        soundTrackSource.time = soundTime;
        soundTrackSource.Play();
    }
    private void OnStateChanged(Game.mState state)
    {
        Tween FirstTween = soundTrackSource.DOFade(0, 0.5f);
        FirstTween.onComplete += () => soundTrackSource.DOFade(soundTrackAudioVolume, 0.5f);
    }
    private void Start()
    {
        soundTrackAudioVolume = soundTrackSource.volume;
        Game.Instance.ui.onFadeMid += OnFadeMid;
        Game.Instance.onStateChanged += OnStateChanged;
    }
}
