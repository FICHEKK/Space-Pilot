using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private Transform spaceship;
    [SerializeField] private float distanceBetweenAsteroidFronts;
    [SerializeField] private int initialAsteroidFrontCount;
    [SerializeField] private int startAsteroidFrontIndex;

    private void Start() => SpawnInitialAsteroidFronts();

    private void SpawnInitialAsteroidFronts()
    {
        for (var i = startAsteroidFrontIndex; i < startAsteroidFrontIndex + initialAsteroidFrontCount; i++)
        {
            var laneIndex = Random.Range(0, mapSettings.laneCount);
            var position = new Vector3(laneIndex * mapSettings.laneWidth, 0, distanceBetweenAsteroidFronts * i);
            var asteroid = Instantiate(asteroidPrefab, position, Quaternion.identity);
        }
    }
}
