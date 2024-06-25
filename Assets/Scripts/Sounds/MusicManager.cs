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

        // 컴포넌트 로드
        musicAudioSource = GetComponent<AudioSource>();

        // 시작 시 음악 꺼짐 상태로 설정
        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);

        // 볼륨 불러오기
        LoadMusicVolume();
    }

    private void LoadMusicVolume()
    {
        // PlayerPrefs에서 볼륨 설정을 불러옴
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }

        // 설정된 볼륨으로 음악 볼륨 설정
        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        // PlayerPrefs에 볼륨 설정 저장
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }

    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
    {
        // 음악 트랙 재생
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        // 만약 fade out 루틴이 실행 중이면 중지
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        // 만약 fade in 루틴이 실행 중이면 중지
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        // 음악 트랙이 변경되었다면 새로운 음악 트랙을 재생
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
        // fade out을 위해 믹서 스냅샷 전환
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        // 클립 설정 및 재생
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        // fade in을 위해 믹서 스냅샷 전환
        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    public void IncreaseMusicVolume()
    {
        // 최대 볼륨 값
        int maxMusicVolume = 20;

        // 최대 볼륨에 도달하면 리턴
        if (musicVolume >= maxMusicVolume) return;

        // 볼륨 증가
        musicVolume += 1;

        // 설정된 볼륨으로 음악 볼륨 설정
        SetMusicVolume(musicVolume);
    }

    public void DecreaseMusicVolume()
    {
        // 볼륨이 0이면 리턴
        if (musicVolume == 0) return;

        // 볼륨 감소
        musicVolume -= 1;

        // 설정된 볼륨으로 음악 볼륨 설정
        SetMusicVolume(musicVolume);
    }

    public void SetMusicVolume(int musicVolume)
    {
        // 음소거 데시벨
        float muteDecibels = -80f;

        // 볼륨이 0이면 음소거
        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            // 선형 값에서 데시벨 값으로 변환하여 설정
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(musicVolume));
        }
    }
}