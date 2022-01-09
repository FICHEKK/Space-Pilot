using System;
using System.Collections;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private GameObject laserHitParticleSystem;

    [Header("Spaceship settings")]
    [SerializeField] private float forwardSpeed = 100;
    [SerializeField] private float maxHealth = 10000;
    [SerializeField] private float slowDownSpeedMultiplier = 0.5f;
    [SerializeField] private float slowDownEnergyConsumptionPerSecond = 30;
    [SerializeField] private float timeRequiredToStartRechargingEnergy = 2;
    [SerializeField] private float energyRechargePerSecond = 10;

    [Header("Laser settings")]
    [SerializeField] private float laserEnergyConsumptionPerSecond = 20;
    [SerializeField] private float laserDamagePerSecond = 500;
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private LayerMask laserRaycastLayerMask;

    [Header("Animation settings")]
    [SerializeField] private int animationTickCount = 50;
    [SerializeField] private float animationRotation = 30;

    [Header("Audio settings")]
    [SerializeField] private AudioSource laserAudioSource;
    [SerializeField] private AudioSource laneChangeAudioSource;

    [Header("Key bindings")]
    [SerializeField] private KeyCode laserShootKey = KeyCode.W;
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode slowDownKey = KeyCode.S;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;

    private int _currentLaneIndex;
    private bool _isMovingToAnotherLane;
    private float _lastTimeEnergyWasConsumed;

    public event Action OnEnergyChanged;
    public event Action OnHealthChanged;
    public event Action OnDeath;

    public float Energy { get; private set; }
    public float MaxEnergy => maxEnergy;

    public float Health { get; private set; }
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        Energy = maxEnergy;
        Health = maxHealth;

        _currentLaneIndex = mapSettings.laneCount / 2;
        transform.position = new Vector3(_currentLaneIndex * mapSettings.laneWidth, 0, 0);
    }

    private void Update()
    {
        MoveForward();
        MoveSidewaysIfNeeded();
    }

    private void MoveForward()
    {
        var speedMultiplier = 1f;

        if (Input.GetKey(slowDownKey) && Energy > 0)
        {
            speedMultiplier = slowDownSpeedMultiplier;
            ConsumeEnergy(slowDownEnergyConsumptionPerSecond);
        }

        transform.Translate(0, 0, forwardSpeed * speedMultiplier * Time.deltaTime);
    }

    private void MoveSidewaysIfNeeded()
    {
        if (_isMovingToAnotherLane) return;

        if (Input.GetKey(moveLeftKey))
        {
            StartCoroutine(MoveToLane(_currentLaneIndex - 1));
        }
        else if (Input.GetKey(moveRightKey))
        {
            StartCoroutine(MoveToLane(_currentLaneIndex + 1));
        }
    }

    private IEnumerator MoveToLane(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= mapSettings.laneCount) yield break;

        laneChangeAudioSource.Play();

        _isMovingToAnotherLane = true;
        var startPositionX = transform.position.x;
        var endPositionX = laneIndex * mapSettings.laneWidth;

        var middleRotation = laneIndex < _currentLaneIndex ? animationRotation : -animationRotation;

        for (var i = 0; i < animationTickCount; i++)
        {
            var percentage = (float) i / animationTickCount;

            transform.eulerAngles = percentage < 0.5f
                ? new Vector3(0, 0, Mathf.Lerp(0, middleRotation, EaseIntOutQuad(percentage / 0.5f)))
                : new Vector3(0, 0, Mathf.Lerp(middleRotation, 0, EaseIntOutQuad((percentage - 0.5f) / 0.5f)));

            var currentPosition = transform.position;
            currentPosition.x = Mathf.Lerp(startPositionX, endPositionX, EaseIntOutQuad(percentage));
            transform.position = currentPosition;

            yield return null;
        }

        _currentLaneIndex = laneIndex;
        _isMovingToAnotherLane = false;

        static float EaseIntOutQuad(float t) =>
            t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }

    private void LateUpdate()
    {
        ShootLaserIfRequested();
        RechargeEnergyIfPossible();
    }

    private void ShootLaserIfRequested()
    {
        var isShootingLaser = Input.GetKey(laserShootKey) && Energy > 0;
        laserHitParticleSystem.SetActive(isShootingLaser);
        laserRenderer.SetPosition(0, laserOrigin.position);

        if (!isShootingLaser)
        {
            laserRenderer.SetPosition(1, laserOrigin.position);
            laserAudioSource.Stop();
            return;
        }

        if (!laserAudioSource.isPlaying) laserAudioSource.Play();
        ConsumeEnergy(laserEnergyConsumptionPerSecond);

        var laserDidHit = Physics.Raycast(laserOrigin.position, Vector3.forward, out var hit, float.PositiveInfinity, laserRaycastLayerMask);
        var laserHitPosition = laserDidHit ? hit.point : laserOrigin.position + Vector3.forward * 10000;

        laserHitParticleSystem.transform.position = laserHitPosition;
        laserRenderer.SetPosition(1, laserHitPosition);

        if (!laserDidHit) return;

        var asteroid = hit.transform.GetComponent<Asteroid>();
        if (asteroid != null) asteroid.Damage(laserDamagePerSecond * Time.deltaTime);
    }

    private void RechargeEnergyIfPossible()
    {
        var elapsedTimeSinceLastEnergyConsumption = Time.time - _lastTimeEnergyWasConsumed;
        if (elapsedTimeSinceLastEnergyConsumption < timeRequiredToStartRechargingEnergy) return;

        Energy = Mathf.Min(Energy + energyRechargePerSecond * Time.deltaTime, MaxEnergy);
        OnEnergyChanged?.Invoke();
    }

    private void ConsumeEnergy(float consumptionPerSecond)
    {
        _lastTimeEnergyWasConsumed = Time.time;
        Energy = Mathf.Max(Energy - consumptionPerSecond * Time.deltaTime, 0);
        OnEnergyChanged?.Invoke();
    }

    private void OnCollisionEnter(Collision other)
    {
        var asteroid = other.gameObject.GetComponent<Asteroid>();
        if (asteroid == null) return;

        asteroid.Break();

        Health -= asteroid.Volume - asteroid.DamageTaken;
        OnHealthChanged?.Invoke();

        if (Health <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}
