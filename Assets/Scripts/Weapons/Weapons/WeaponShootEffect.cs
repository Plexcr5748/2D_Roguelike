using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake()
    {
        // ������Ʈ �ε�
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// WeaponShootEffectSO�� aimAngle�� ������� Shoot Effect�� ����
    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        // ���� �׶��̼� ����
        SetShootEffectColorGradient(shootEffect.colorGradient);

        // ��ƼŬ �ý��� ���� �� ����
        SetShootEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticleSize, shootEffect.startParticleSpeed, shootEffect.startLifetime, shootEffect.effectGravity, shootEffect.maxParticleNumber);

        // ��ƼŬ �ý��� ��ƼŬ �߻� ����
        SetShootEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);

        // ������ ȸ�� ����
        SetEmmitterRotation(aimAngle);

        // ��ƼŬ �ý��� ��������Ʈ ����
        SetShootEffectParticleSprite(shootEffect.sprite);

        // ��ƼŬ �ý��� �ӵ� ����
        SetShootEffectVelocityOverLifeTime(shootEffect.velocityOverLifetimeMin, shootEffect.velocityOverLifetimeMax);

    }

    /// Shoot Effect�� ���� �׶��̼��� ����
    private void SetShootEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// Shoot Effect�� ��ƼŬ �ý��� ���� ������ ����
    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;

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

    /// Shoot Effect�� ��ƼŬ �ý��� ��ƼŬ �߻� ������ ��
    private void SetShootEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        // ��ƼŬ ���� �� ����
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        // ��ƼŬ �߻� �ӵ� ����
        emissionModule.rateOverTime = emissionRate;
    }

    /// Shoot Effect�� ��ƼŬ �ý��� ��������Ʈ�� ����
    private void SetShootEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);

    }

    /// Aim Angle�� ���� Emmitter�� ȸ���� ����
    private void SetEmmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    /// Shoot Effect�� ��ƼŬ �ý��� �ӵ��� ����
    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;

        // �ּ� �� �ִ� X �ӵ� ����
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        // �ּ� �� �ִ� Y �ӵ� ����
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        // �ּ� �� �ִ� Z �ӵ� ����
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;

    }

}
