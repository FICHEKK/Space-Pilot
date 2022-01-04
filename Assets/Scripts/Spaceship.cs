using System.Collections;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private float forwardSpeed = 100f;
    [SerializeField] private int animationTickCount = 50;
    [SerializeField] private float animationRotation = 30;
    private int _currentLaneIndex;
    private bool _isMovingToAnotherLane;

    private void Update()
    {
        transform.Translate(0, 0, forwardSpeed * Time.deltaTime);
        MoveSidewaysIfNeeded();
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
    }
}
