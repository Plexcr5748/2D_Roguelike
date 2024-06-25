using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonobehaviour<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        // ������Ʈ �ε�
        musicAudioSource = GetComponent<AudioSource>();

        // ���� �� ���� ���� ���·� ����
        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);

        // ���� �ҷ�����
        LoadMusicVolume();
    }

    private void LoadMusicVolume()
    {
        // PlayerPrefs���� ���� ������ �ҷ���
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }

        // ������ �������� ���� ���� ����
        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        // PlayerPrefs�� ���� ���� ����
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }

    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
    {
        // ���� Ʈ�� ���
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        // ���� fade out ��ƾ�� ���� ���̸� ����
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        // ���� fade in ��ƾ�� ���� ���̸� ����
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        // ���� Ʈ���� ����Ǿ��ٸ� ���ο� ���� Ʈ���� ���
        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }

    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        // fade out�� ���� �ͼ� ������ ��ȯ
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        // Ŭ�� ���� �� ���
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        // fade in�� ���� �ͼ� ������ ��ȯ
        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    public void IncreaseMusicVolume()
    {
        // �ִ� ���� ��
        int maxMusicVolume = 20;

        // �ִ� ������ �����ϸ� ����
        if (musicVolume >= maxMusicVolume) return;

        // ���� ����
        musicVolume += 1;

        // ������ �������� ���� ���� ����
        SetMusicVolume(musicVolume);
    }

    public void DecreaseMusicVolume()
    {
        // ������ 0�̸� ����
        if (musicVolume == 0) return;

        // ���� ����
        musicVolume -= 1;

        // ������ �������� ���� ���� ����
        SetMusicVolume(musicVolume);
    }

    public void SetMusicVolume(int musicVolume)
    {
        // ���Ұ� ���ú�
        float muteDecibels = -80f;

        // ������ 0�̸� ���Ұ�
        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            // ���� ������ ���ú� ������ ��ȯ�Ͽ� ����
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(musicVolume));
        }
    }
}