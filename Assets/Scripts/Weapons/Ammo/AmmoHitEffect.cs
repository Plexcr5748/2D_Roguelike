using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// Passed in AmmoHitEffectSO details로부터 Hit Effect를 설정합니다.
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        // Hit Effect의 색상 그래디언트 설정
        SetHitEffectColorGradient(ammoHitEffect.colorGradient);

        // Hit Effect의 파티클 시스템 초기 값 설정
        SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed, ammoHitEffect.startLifetime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);

        // Hit Effect의 파티클 시스템 파티클 버스트 수 설정
        SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);

        // Hit Effect의 파티클 스프라이트 설정
        SetHitEffectParticleSprite(ammoHitEffect.sprite);

        // Hit Effect의 수명 최소 및 최대 속도 설정
        SetHitEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);
    }

    /// Hit Effect의 파티클 시스템 색상 그래디언트 설정
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        // 색상 그래디언트 설정
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// Hit Effect의 파티클 시스템 초기 값 설정
    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

        // 파티클 시스템 지속 시간 설정
        mainModule.duration = duration;

        // 파티클 시작 크기 설정
        mainModule.startSize = startParticleSize;

        // 파티클 시작 속도 설정
        mainModule.startSpeed = startParticleSpeed;

        // 파티클 시작 수명 설정
        mainModule.startLifetime = startLifetime;

        // 파티클 시작 중력 설정
        mainModule.gravityModifier = effectGravity;

        // 최대 파티클 수 설정
        mainModule.maxParticles = maxParticles;
    }

    /// Hit Effect의 파티클 시스템 파티클 버스트 수 설정
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

        // 파티클 버스트 수 설정
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        // 파티클 배출률 설정
        emissionModule.rateOverTime = emissionRate;
    }

    /// Hit Effect의 파티클 시스템 스프라이트 설정
    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        // 파티클 스프라이트 설정
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// Hit Effect의 LifeTime 동안 설정
    private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        // 최소 최대 X 속도 정의
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        // 최소 최대 Y 속도 정의
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        // 최소 최대 Z 속도 정의
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
