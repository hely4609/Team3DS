using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Manager
{
    AudioMixer currentMixer;
    AudioMixerGroup AMGmaster;
    AudioMixerGroup AMGbgm;
    AudioMixerGroup AMGsfx;

    AudioSource[] bgmArray = new AudioSource[2];
    const int sfxMaxNumber = 10;
    Queue<AudioSource> sfxQueue = new();

    Action<float> AudioEffectUpdate;
    public override IEnumerator Initiate()
    {
        currentMixer = ResourceManager.Mixer;
        if (currentMixer == null) Debug.LogWarning("AudioMixer not found!");
        AMGmaster = currentMixer.FindMatchingGroups("Master")[0];
        AMGbgm = currentMixer.FindMatchingGroups("BGM")[0];
        AMGsfx = currentMixer.FindMatchingGroups("SFX")[0];
        if (AMGmaster == null) Debug.LogWarning("AudioMixerGroup Master not found!");
        if (AMGbgm == null) Debug.LogWarning("AudioMixerGroup BGM not found!");
        if (AMGsfx == null) Debug.LogWarning("AudioMixerGroup SFX not found!");

        Transform soundContainer = new GameObject("Sound Container").transform;
        soundContainer.SetParent(GameManager.Instance.transform);
        // BGM 교체할 때 페이드인/아웃 하기위해 AudioSource를 2개 준비
        GameObject bgmCarrier = new("BGM Carrier", typeof(AudioSource), typeof(AudioSource));
        bgmCarrier.transform.SetParent(soundContainer);
        bgmArray = bgmCarrier.GetComponents<AudioSource>();

        for(int i = 0; i < bgmArray.Length; i++)
        {
            bgmArray[i].outputAudioMixerGroup = AMGbgm;
            bgmArray[i].loop = true;
            bgmArray[i].playOnAwake = false;
            // BGM은 거리 상관 없으므로
            bgmArray[i].maxDistance = float.MaxValue;
            bgmArray[i].minDistance = float.MaxValue;
        }

        for(int i=0; i<sfxMaxNumber; i++)
        {
            GameObject sfxCarrier = new("SFX Carrier", typeof(AudioSource));
            sfxCarrier.transform.SetParent(soundContainer);
            AudioSource currentSource = sfxCarrier.GetComponent<AudioSource>();
            currentSource.outputAudioMixerGroup = AMGsfx;
            currentSource.playOnAwake = false;
            currentSource.spatialBlend = 1;
            sfxQueue.Enqueue(currentSource);
        }

        yield return null;
    }

    public override void ManagerUpdate(float deltaTime)
    {
        AudioEffectUpdate?.Invoke(deltaTime);
    }

    public void UpdateBGMMixer(float deltaTime)
    {
        bgmArray[0].volume = Mathf.SmoothStep(bgmArray[0].volume, 1, deltaTime * 5);
        bgmArray[1].volume = Mathf.SmoothStep(bgmArray[1].volume, 0, deltaTime * 5);
        if (bgmArray[0].volume == 1)
        {
            AudioEffectUpdate -= UpdateBGMMixer;
        }
    }

    public static void Play(ResourceEnum.BGM wantBGM)
    {
        // 0 : 플레이 할 브금
        // 1 : 현재 플레이 중인 브금
        // 현재 0번을 1번으로 보내고 플레이 할 브금을 0에 넣기
        SoundManager soundManager = GameManager.Instance.SoundManager;
        soundManager.bgmArray[1].clip = soundManager.bgmArray[0].clip;
        soundManager.bgmArray[1].time = soundManager.bgmArray[0].time;
        soundManager.bgmArray[1].volume = soundManager.bgmArray[0].volume;

        soundManager.bgmArray[0].clip = ResourceManager.Get(wantBGM);
        soundManager.bgmArray[0].time = 0;
        soundManager.bgmArray[0].volume = 0;

        soundManager.AudioEffectUpdate -= soundManager.UpdateBGMMixer;
        soundManager.AudioEffectUpdate += soundManager.UpdateBGMMixer;
    }

    public static void Play(ResourceEnum.SFX wantSFX, Vector3 soundOrigin, bool loop = false)
    {
        SoundManager soundManager = GameManager.Instance.SoundManager;
        AudioClip clip = ResourceManager.Get(wantSFX);
        if(soundManager.sfxQueue.TryDequeue(out AudioSource currentSource))
        {
            currentSource.clip = clip;
            currentSource.loop = loop;
            currentSource.transform.position = soundOrigin;
            currentSource.Play();
            soundManager.sfxQueue.Enqueue(currentSource);
        }
    }

    public static void Play(ResourceEnum.SFX wantSFX, Vector3 soundOrigin, out AudioSource source)
    {
        SoundManager soundManager = GameManager.Instance.SoundManager;
        AudioClip clip = ResourceManager.Get(wantSFX);
        if (soundManager.sfxQueue.TryDequeue(out AudioSource currentSource))
        {
            currentSource.clip = clip;
            currentSource.loop = true;
            currentSource.transform.position = soundOrigin;
            currentSource.Play();
            source = currentSource;
        }
        else source = null;
    }
    public static void StopBGM()
    {
        foreach(var bgm in GameManager.Instance.SoundManager.bgmArray)
        {
            bgm.Stop();
        }
    }

    public static void StopSFX(AudioSource source)
    {
        source.Stop();
        GameManager.Instance.SoundManager.sfxQueue.Enqueue(source);
    }
}
