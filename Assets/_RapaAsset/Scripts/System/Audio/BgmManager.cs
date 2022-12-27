using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BgmChannels
{
    Defult,
    Second,
    All,
}

public class BgmManager : UnitySingleton_DR<BgmManager>
{
    public AudioSource[] BgmSources = new AudioSource[2];
    public float DefultVolumn = .7f;

    public void SetVolumn(float volumn, BgmChannels SoundChannel)
    {
        if (SoundChannel == BgmChannels.All)
        {
            foreach (var item in BgmSources)
            {
                item.DOKill();
                item.volume = volumn;
            }
        }
        else
        {
            BgmSources[(int)SoundChannel].DOKill();
            BgmSources[(int)SoundChannel].volume = volumn;
        }
    }

    public void VolumeFade(float FadeTime, float volume, BgmChannels SoundChannel)
    {
        VolumeFade(FadeTime, volume, SoundChannel, null);
    }

    /// <summary>
    ///  Fade volum of Target BGMChannel
    /// </summary>
    /// <param name="FadeTime"></param>
    /// <param name="to"></param>
    /// <param name="SoundChannel"></param>
    /// <param name="OnCompelete"></param>
    public void VolumeFade(float FadeTime, float volume, BgmChannels SoundChannel, Action OnCompelete = null)
    {
        if (SoundChannel == BgmChannels.All)
        {
            foreach (var item in BgmSources)
            {
                item.DOKill();
                item.DOFade(volume, FadeTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { OnCompelete(); });
            }
        }
        else
        {
            BgmSources[(int)SoundChannel].DOKill();
            BgmSources[(int)SoundChannel]
                .DOFade(volume, FadeTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => { OnCompelete(); });
        }
    }

    /// <summary>
    /// Play BGM Music
    /// </summary>
    /// <param name="clip">The bgm clip</param>
    /// <param name="SoundChannel">Target Channel, All = Default </param>
    public void PlayBgm(AudioClip clip, BgmChannels SoundChannel)
    {
#if NO_BGM
        return;
#endif
        int _index = (int)SoundChannel;
        if (_index >= BgmSources.Length)
            _index = (int)BgmChannels.Defult;

        if (BgmSources[_index].clip != null)
        {
            if (BgmSources[_index].clip.name != clip.name || !BgmSources[_index].isPlaying)
            {
                BgmSources[_index].clip = clip;
                BgmSources[_index].Play();
            }
        }
        else
        {
            BgmSources[_index].clip = clip;
            BgmSources[_index].Play();
        }
    }

    public void Stop()
    {
        foreach (var track in BgmSources)
        {
            track.Stop();
        }
    }
}