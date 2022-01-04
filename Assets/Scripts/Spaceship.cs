using System.Collections;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private float forwardSpeed = 100f;
    [SerializeField] private int animationTickCount = 50;
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

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StartCoroutine(MoveToLane(_currentLaneIndex - 1));
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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

        for (var i = 0; i < animationTickCount; i++)
        {
            var percentage = EaseIntOutQuad((float) i / animationTickCount);

            var currentPosition = transform.position;
            currentPosition.x = Mathf.Lerp(startPositionX, endPositionX, percentage);
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
        Debug.Log("Entered asteroid!");
    }

    private void OnCollisionStay(Collision other)
    {
        Debug.Log("Stayin.");
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log("Out!");
    }
}
