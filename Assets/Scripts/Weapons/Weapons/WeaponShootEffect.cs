using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake()
    {
        // 컴포넌트 로드
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// WeaponShootEffectSO와 aimAngle을 기반으로 Shoot Effect를 설정
    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        // 색상 그라데이션 설정
        SetShootEffectColorGradient(shootEffect.colorGradient);

        // 파티클 시스템 시작 값 설정
        SetShootEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticleSize, shootEffect.startParticleSpeed, shootEffect.startLifetime, shootEffect.effectGravity, shootEffect.maxParticleNumber);

        // 파티클 시스템 파티클 발생 설정
        SetShootEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);

        // 에미터 회전 설정
        SetEmmitterRotation(aimAngle);

        // 파티클 시스템 스프라이트 설정
        SetShootEffectParticleSprite(shootEffect.sprite);

        // 파티클 시스템 속도 설정
        SetShootEffectVelocityOverLifeTime(shootEffect.velocityOverLifetimeMin, shootEffect.velocityOverLifetimeMax);

    }

    /// Shoot Effect의 색상 그라데이션을 설정
    private void SetShootEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// Shoot Effect의 파티클 시스템 시작 값들을 설정
    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;

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

    /// Shoot Effect의 파티클 시스템 파티클 발생 설정을 함
    private void SetShootEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        // 파티클 폭발 수 설정
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        // 파티클 발생 속도 설정
        emissionModule.rateOverTime = emissionRate;
    }

    /// Shoot Effect의 파티클 시스템 스프라이트를 설정
    private void SetShootEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);

    }

    /// Aim Angle에 따라 Emmitter의 회전을 설정
    private void SetEmmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    /// Shoot Effect의 파티클 시스템 속도를 설정
    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;

        // 최소 및 최대 X 속도 정의
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        // 최소 및 최대 Y 속도 정의
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        // 최소 및 최대 Z 속도 정의
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;

    }

}
