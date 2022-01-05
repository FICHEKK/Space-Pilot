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
    [SerializeField] private float turboSpeedMultiplier = 3;
    [SerializeField] private float maxHealth = 10000;

    [Header("Laser settings")]
    [SerializeField] private float laserEnergyConsumptionPerSecond = 20;
    [SerializeField] private float laserDamagePerSecond = 500;
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private float timeRequiredToStartRecharging = 2;
    [SerializeField] private LayerMask laserRaycastLayerMask;

    [Header("Animation settings")]
    [SerializeField] private int animationTickCount = 50;
    [SerializeField] private float animationRotation = 30;

    [Header("Sounds")]
    [SerializeField] private AudioSource laserAudioSource;

    private int _currentLaneIndex;
    private bool _isMovingToAnotherLane;
    private float _lastTimeLaserWasShot;

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
        var speedMultiplier = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? turboSpeedMultiplier : 1;
        transform.Translate(0, 0, forwardSpeed * speedMultiplier * Time.deltaTime);
        MoveSidewaysIfNeeded();
    }

    private void LateUpdate()
    {
        laserRenderer.SetPosition(0, laserOrigin.position);

        if (Energy > 0 && Input.GetKey(KeyCode.Space))
        {
            if (!laserAudioSource.isPlaying) laserAudioSource.Play();

            if (Physics.Raycast(laserOrigin.position, Vector3.forward, out var hit, float.PositiveInfinity, laserRaycastLayerMask))
            {
                laserHitParticleSystem.SetActive(true);
                laserHitParticleSystem.transform.position = hit.point;
                laserRenderer.SetPosition(1, hit.point);

                var asteroid = hit.transform.GetComponent<Asteroid>();

                if (asteroid != null)
                {
                    asteroid.Damage(laserDamagePerSecond * Time.deltaTime);
                }
                else
                {
                    Debug.Log($"We hit {hit.transform.name}");
                }
            }
            else
            {
                laserHitParticleSystem.SetActive(false);
                laserRenderer.SetPosition(1, laserOrigin.position + Vector3.forward * 10000);
            }

            Energy = Mathf.Max(Energy - laserEnergyConsumptionPerSecond * Time.deltaTime, 0);
            OnEnergyChanged?.Invoke();

            _lastTimeLaserWasShot = Time.time;
        }
        else
        {
            if (laserAudioSource.isPlaying) laserAudioSource.Stop();

            laserHitParticleSystem.SetActive(false);
            laserRenderer.SetPosition(1, laserOrigin.position);

            if (Time.time - _lastTimeLaserWasShot >= timeRequiredToStartRecharging)
            {
                Energy = Mathf.Min(Energy + laserEnergyConsumptionPerSecond * Time.deltaTime, MaxEnergy);
                OnEnergyChanged?.Invoke();
            }
        }
    }

    private void MoveSidewaysIfNeeded()
    {
        if (_isMovingToAnotherLane) return;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            StartCoroutine(MoveToLane(_currentLaneIndex - 1));
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            StartCoroutine(MoveToLane(_currentLaneIndex + 1));
        }
    }

    private IEnumerator MoveToLane(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= mapSettings.laneCount) yield break;

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
