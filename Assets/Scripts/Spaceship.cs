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

    [Header("Animation settings")]
    [SerializeField] private int animationTickCount = 50;
    [SerializeField] private float animationRotation = 30;

    private int _currentLaneIndex;
    private bool _isMovingToAnotherLane;

    public event Action OnHealthChanged;
    public event Action OnDeath;

    public float Health { get; private set; }
    public float MaxHealth => maxHealth;

    private void Awake()
    {
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
        HandleLaser();
    }

    private void HandleLaser()
    {
        laserRenderer.SetPosition(0, laserOrigin.position);

        if (Input.GetKey(KeyCode.Space))
        {
            if (Physics.Raycast(laserOrigin.position, Vector3.forward, out var hit))
            {
                laserHitParticleSystem.SetActive(true);
                laserHitParticleSystem.transform.position = hit.point;
                laserRenderer.SetPosition(1, hit.point);
            }
            else
            {
                laserHitParticleSystem.SetActive(false);
                laserRenderer.SetPosition(1, laserOrigin.position + Vector3.forward * 10000);
            }
        }
        else
        {
            laserHitParticleSystem.SetActive(false);
            laserRenderer.SetPosition(1, laserOrigin.position);
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

        var bounds = other.gameObject.GetComponent<MeshRenderer>().bounds;
        var dx = Mathf.Abs(bounds.min.x - bounds.max.x);
        var dy = Mathf.Abs(bounds.min.y - bounds.max.y);
        var dz = Mathf.Abs(bounds.min.z - bounds.max.z);
        var asteroidVolume = dx * dy * dz;

        Health -= asteroidVolume;
        OnHealthChanged?.Invoke();

        if (Health <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}
