using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoSingleton<MusicManager>
{
    private AudioSource _bgmSource;
    private AudioSource _clipsAudioSource;

    public SOClipsData _clipsData;
    public SOClipsData _bgmsData;


    private void Awake()
    {
        _clipsAudioSource = this.AddComponent<AudioSource>();
        _bgmSource = this.AddComponent<AudioSource>();
        _bgmSource.clip = _bgmsData.clips[0];
        _bgmSource.Play();
        _bgmSource.loop = true;
    }

    public void PlayClipByIndex(int index)
    {
        this._clipsAudioSource.clip = _clipsData.clips[index];
        this._clipsAudioSource.Play();
    }


    public void PlayClipByClip(AudioClip clip)
    {
        this._clipsAudioSource.clip = clip;
        this._clipsAudioSource.Play();
    }

}
