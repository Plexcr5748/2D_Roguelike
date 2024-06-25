using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// Passed in AmmoHitEffectSO details�κ��� Hit Effect�� �����մϴ�.
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        // Hit Effect�� ���� �׷����Ʈ ����
        SetHitEffectColorGradient(ammoHitEffect.colorGradient);

        // Hit Effect�� ��ƼŬ �ý��� �ʱ� �� ����
        SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed, ammoHitEffect.startLifetime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);

        // Hit Effect�� ��ƼŬ �ý��� ��ƼŬ ����Ʈ �� ����
        SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);

        // Hit Effect�� ��ƼŬ ��������Ʈ ����
        SetHitEffectParticleSprite(ammoHitEffect.sprite);

        // Hit Effect�� ���� �ּ� �� �ִ� �ӵ� ����
        SetHitEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);
    }

    /// Hit Effect�� ��ƼŬ �ý��� ���� �׷����Ʈ ����
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        // ���� �׷����Ʈ ����
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// Hit Effect�� ��ƼŬ �ý��� �ʱ� �� ����
    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

        // ��ƼŬ �ý��� ���� �ð� ����
        mainModule.duration = duration;

        // ��ƼŬ ���� ũ�� ����
        mainModule.startSize = startParticleSize;

        // ��ƼŬ ���� �ӵ� ����
        mainModule.startSpeed = startParticleSpeed;

        // ��ƼŬ ���� ���� ����
        mainModule.startLifetime = startLifetime;

        // ��ƼŬ ���� �߷� ����
        mainModule.gravityModifier = effectGravity;

        // �ִ� ��ƼŬ �� ����
        mainModule.maxParticles = maxParticles;
    }

    /// Hit Effect�� ��ƼŬ �ý��� ��ƼŬ ����Ʈ �� ����
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

        // ��ƼŬ ����Ʈ �� ����
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        // ��ƼŬ ����� ����
        emissionModule.rateOverTime = emissionRate;
    }

    /// Hit Effect�� ��ƼŬ �ý��� ��������Ʈ ����
    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        // ��ƼŬ ��������Ʈ ����
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// Hit Effect�� LifeTime ���� ����
    private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        // �ּ� �ִ� X �ӵ� ����
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        // �ּ� �ִ� Y �ӵ� ����
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        // �ּ� �ִ� Z �ӵ� ����
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
