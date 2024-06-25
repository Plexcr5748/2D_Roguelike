using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int soundsVolume = 8;

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }

        SetSoundsVolume(soundsVolume);
    }

    private void OnDisable()
    {
        // �÷��̾� ���������� ���� ������ ����
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    /// ���� ����Ʈ�� ���
    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        // ������Ʈ Ǯ���� ���� ������Ʈ�� �����Ͽ� ���带 ���
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }

    /// ���� ����Ʈ ��� �� ���� �ð��� ���� �� ������Ʈ�� ��Ȱ��ȭ�Ͽ� ������Ʈ Ǯ�� ��ȯ
    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    /// �Ҹ� ������ ����
    public void IncreaseSoundsVolume()
    {
        int maxSoundsVolume = 20;

        if (soundsVolume >= maxSoundsVolume) return;

        soundsVolume += 1;

        SetSoundsVolume(soundsVolume); ;
    }

    /// �Ҹ� ������ ����
    public void DecreaseSoundsVolume()
    {
        if (soundsVolume == 0) return;

        soundsVolume -= 1;

        SetSoundsVolume(soundsVolume);
    }

    /// �Ҹ� ������ ����
    private void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        if (soundsVolume == 0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }
}