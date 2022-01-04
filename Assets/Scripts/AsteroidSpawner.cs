using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private Transform spaceship;
    [SerializeField] private float distanceBetweenAsteroidFronts;
    [SerializeField] private int initialAsteroidFrontCount;
    [SerializeField] private int startAsteroidFrontIndex;
    [SerializeField] private List<GameObject> asteroidPrefabs;

    private void Start() => SpawnInitialAsteroidFronts();

    private void SpawnInitialAsteroidFronts()
    {
        for (var i = startAsteroidFrontIndex; i < startAsteroidFrontIndex + initialAsteroidFrontCount; i++)
        {
            var asteroidCount = Random.Range(0, mapSettings.laneCount);

            foreach (var laneIndex in GetRandomLaneIndices(asteroidCount))
            {
                var position = new Vector3(laneIndex * mapSettings.laneWidth, 0, distanceBetweenAsteroidFronts * i);
                var asteroid = Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)], position, Quaternion.identity);
                asteroid.transform.localScale = new Vector3(Random.Range(2, 3f), Random.Range(2, 3f), Random.Range(2, 3f));
            }
        }
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
