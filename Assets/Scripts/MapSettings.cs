using UnityEngine;

[CreateAssetMenu(fileName = "Map Settings", menuName = "Map Settings", order = 0)]
public class MapSettings : ScriptableObject
{
    [Header("Lane settings")]
    public int laneCount;
    public float laneWidth;

    [Header("Front settings")]
    public int frontCount;
    public float firstFrontDistanceFromOrigin;
    public float minDistanceBetweenFronts;
    public float maxDistanceBetweenFronts;
    public int minAsteroidCountPerFront;
    public int maxAsteroidCountPerFront;

    [Header("Asteroid settings")]
    public Vector3 minAsteroidScale;
    public Vector3 maxAsteroidScale;
}
