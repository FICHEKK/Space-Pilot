using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private Transform spaceship;
    [SerializeField] private List<GameObject> asteroidPrefabs;

    private GameObject _fronts;
    private float _lastFrontDistanceFromOrigin;

    private void Start()
    {
        _fronts = new GameObject("Fronts");
        _lastFrontDistanceFromOrigin = mapSettings.firstFrontDistanceFromOrigin;

        for (var i = 0; i < mapSettings.frontCount; i++)
        {
            SpawnNextFront();
        }
    }

    private void Update()
    {
        if (spaceship == null || _fronts.transform.childCount == 0) return;

        var firstFront = _fronts.transform.GetChild(0);
        if (spaceship.position.z < firstFront.position.z) return;

        Destroy(firstFront.gameObject);
        SpawnNextFront();
    }

    private void SpawnNextFront()
    {
        var front = new GameObject("Front");
        front.transform.parent = _fronts.transform;
        front.transform.position = new Vector3(0, 0, _lastFrontDistanceFromOrigin);

        foreach (var laneIndex in GetRandomLaneIndices(laneCount: Random.Range(mapSettings.minAsteroidCountPerFront, mapSettings.maxAsteroidCountPerFront + 1)))
        {
            var asteroid = Instantiate(
                original: asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)],
                position: new Vector3(laneIndex * mapSettings.laneWidth, 0, front.transform.position.z),
                rotation: Quaternion.identity,
                parent: front.transform
            );

            asteroid.transform.localScale = new Vector3(
                Random.Range(mapSettings.minAsteroidScale.x, mapSettings.maxAsteroidScale.x),
                Random.Range(mapSettings.minAsteroidScale.y, mapSettings.maxAsteroidScale.y),
                Random.Range(mapSettings.minAsteroidScale.z, mapSettings.maxAsteroidScale.z)
            );
        }

        _lastFrontDistanceFromOrigin += Random.Range(mapSettings.minDistanceBetweenFronts, mapSettings.maxDistanceBetweenFronts);
    }

    private IEnumerable<int> GetRandomLaneIndices(int laneCount)
    {
        var laneIndices = new int[mapSettings.laneCount];

        for (var i = 0; i < mapSettings.laneCount; i++)
        {
            laneIndices[i] = i;
        }

        Shuffle(laneIndices);

        for (var i = 0; i < laneCount; i++)
        {
            yield return laneIndices[i];
        }
    }

    private static void Shuffle<T>(T[] array)
    {
        for (var i = 0; i < array.Length - 1; i++)
        {
            var j = Random.Range(i, array.Length);

            var value = array[j];
            array[j] = array[i];
            array[i] = value;
        }
    }
}
