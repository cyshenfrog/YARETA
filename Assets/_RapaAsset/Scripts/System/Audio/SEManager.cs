using UnityEngine;

public enum SystemSE
{
    Menu,
    UI確認,
    UI取消,
    UI選擇,
    回收白石,
    大女神出場,
    大鳥起風,
    大鳥飛走,
    女神呢喃,
    女神贈與,
    女神贈與2,
    山崩結束,
    山崩開始,
    強風環境音,
    打破柱子,
    打破柱子2,
    吃水果,
    拔根聲音LOOP,
    搖樹,
    樹上掉落,
    樹上柱子落下,
    樹倒鳥飛,
    機關解謎,
    物品產生,
    精靈飛出,
    精靈飛出2,
    草叢聲短,
    草叢聲長,
    葉子起飛,
    蛋破掉,
    轉動拉霸,
    逃跑花,
    進水1,
    進水2,
    鐘,
    鐘2,
    鵝生氣,
    畫畫,
    小女神登場,
    落地聲,
    石頭拖地聲LOOP,
    跌倒,
    鵝叫聲,
    拔蘿蔔,
    畫圖更新,
    呢喃1,
    呢喃2,
    呢喃3,
    畫好了
}

public enum HitSEType
{
    Soft,
    Hard
}

public class SEManager : UnitySingleton_D<SEManager>
{
    [HideInInspector]
    public AudioSource SESource;

    public AudioClip[] SystemSEs;
    public AudioClip[] HardHitSEs;
    public AudioClip[] SoftHitSEs;
    public AudioClip[] DefaultWalkSE;
    public AudioClip[] CurrentWalkSE;
    private const float DEFAULT_VOL = 1;

    public override void Awake()
    {
        base.Awake();
        CurrentWalkSE = DefaultWalkSE;
        SESource = gameObject.AddComponent<AudioSource>();
        SESource.spatialBlend = 0;
        SESource.volume = DEFAULT_VOL;
    }

    public AudioClip GetSystemClip(SystemSE Name)
    {
        return SystemSEs[(int)Name];
    }

    public AudioClip GetRandomHitSE(HitSEType type)
    {
        switch (type)
        {
            case HitSEType.Soft:
                return SoftHitSEs[Random.Range(0, SoftHitSEs.Length)];

            case HitSEType.Hard:
            default:
                return HardHitSEs[Random.Range(0, HardHitSEs.Length)];
        }
    }

    public void PlayStepSE()
    {
        if (CurrentWalkSE.Length > 0)
            AudioSource.PlayClipAtPoint(CurrentWalkSE[Random.Range(0, CurrentWalkSE.Length)], Player.Instance.transform.position, 0.3f);
    }

    public void ResetWalkSE()
    {
        CurrentWalkSE = DefaultWalkSE;
    }

    public void PlaySystemSE(SystemSE se = SystemSE.UI選擇, float volumn = 1, bool randPitch = false)
    {
        Play(SystemSEs[(int)se], volumn, randPitch);
    }

    public void PlaySystemSERand(SystemSE se = SystemSE.UI選擇, float volumn = 1, float LowPitchRange = .95f, float HighPitchRange = 1.05f)
    {
        PlaySE_RandomPitch(SystemSEs[(int)se], volumn, LowPitchRange, HighPitchRange);
    }

    public void Stop()
    {
        SESource.Stop();
    }

    public void Play(AudioClip clip, float volumn = 1, bool randPitch = false)
    {
        if (randPitch)
            PlaySE_RandomPitch(clip, volumn);
        else
        {
            SESource.PlayOneShot(clip, volumn);
        }
    }

    public void Play(AudioClip[] clips, float volumn = 1, bool randPitch = false)
    {
        int randomIndex = 0;
        if (clips.Length > 1)
            randomIndex = Random.Range(0, clips.Length);
        Play(clips[randomIndex], volumn, randPitch);
    }

    private void PlaySE_RandomPitch(AudioClip clip, float volumn = 1, float LowPitchRange = .95f, float HighPitchRange = 1.05f)
    {
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);
        SESource.pitch = randomPitch;
        //SESource.Stop();
        //SESource.time = 0;
        SESource.PlayOneShot(clip, volumn);
    }
}