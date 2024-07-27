using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoSington<MusicManager>
{
    private AudioSource _bgmSource;
    private AudioSource _clipsAudioSource;

    public SOClipsData _clipsData;
    public SOClipsData _bgmsData;


    private void Awake()
    {
        _clipsAudioSource = this.AddComponent<AudioSource>();
        _bgmSource = this.AddComponent<AudioSource>();
        _bgmSource.clip = _bgmsData?.clips[this.SwitchBgmBySceneName()];
        _bgmSource.Play();
        _bgmSource.loop = true;
    }


    int SwitchBgmBySceneName()
    {
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "MainScene":
                return 8;
            case "CardScene":
                return 8;
            case "BattleScene":
                return 5;
            case "IntroductScene":
                return 5;
        }
        return 0;
    }
    public void PlayClipByIndex(int index,int start = 0)
    {
        this._clipsAudioSource.clip = _clipsData?.clips[index];
        //Debug.Log(this._clipsAudioSource.clip.length);
        this._clipsAudioSource.time = start;
        this._clipsAudioSource.Play();
    }


    public void PlayClipByClip(AudioClip clip)
    {
        this._clipsAudioSource.clip = clip;
        this._clipsAudioSource.Play();
    }
    public void PlayBgmByIndex(int index, int start = 0)
    {
        this._bgmSource.clip = _bgmsData?.clips[index];
        Debug.Log(this._bgmSource.clip.length);
        this._bgmSource.time = start;
        this._bgmSource.Play();
    }
}
