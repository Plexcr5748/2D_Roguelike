using UnityEngine;
using UnityEngine.Rendering.Universal; // 2D 라이트가 비실험적 상태가 되면 이 네임스페이스를 유지해야 함.
using UnityEngine.Experimental.Rendering.Universal;

[DisallowMultipleComponent]
public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField] private float lightIntensityMin;   // 빛의 최소 강도
    [SerializeField] private float lightIntensityMax;   // 빛의 최대 강도
    [SerializeField] private float lightFlickerTimeMin; // 깜박임 최소 시간
    [SerializeField] private float lightFlickerTimeMax; // 깜박임 최대 시간
    private float lightFlickerTimer;                     // 깜박임 타이머

    private void Awake()
    {
        // 컴포넌트 로드
        light2D = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);
    }

    private void Update()
    {
        if (light2D == null) return;

        lightFlickerTimer -= Time.deltaTime;

        if (lightFlickerTimer < 0f)
        {
            lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);

            RandomiseLightIntensity();
        }
    }

    private void RandomiseLightIntensity()
    {
        light2D.intensity = Random.Range(lightIntensityMin, lightIntensityMax);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 빛의 강도 범위 검증
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightIntensityMin), lightIntensityMin, nameof(lightIntensityMax), lightIntensityMax, false);
        // 깜박임 시간 범위 검증
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightFlickerTimeMin), lightFlickerTimeMin, nameof(lightFlickerTimeMax), lightFlickerTimeMax, false);
    }
#endif
    #endregion
}