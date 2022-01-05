using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    private const float AsteroidShardForceMultiplier = 100;
    private const float AsteroidShardLifeDuration = 3;

    [SerializeField] private GameObject fracturedPrefab;
    private Vector3 _rotationAxis;
    private float _rotationSpeed;
    private float _damageTaken;

    private void Awake()
    {
        _rotationAxis = Random.onUnitSphere;
        _rotationSpeed = 2 * Random.value - 1;
    }

    private void Update()
    {
        transform.Rotate(_rotationAxis, _rotationSpeed);
    }

    public void Damage(float amount)
    {
        _damageTaken += amount;

        var bounds = GetComponent<MeshRenderer>().bounds;
        var dx = Mathf.Abs(bounds.min.x - bounds.max.x);
        var dy = Mathf.Abs(bounds.min.y - bounds.max.y);
        var dz = Mathf.Abs(bounds.min.z - bounds.max.z);
        var volume = dx * dy * dz;

        if (_damageTaken >= volume) Break();
    }

    public void Break()
    {
        var fractured = Instantiate(fracturedPrefab, transform.position, transform.rotation);
        fractured.transform.localScale = transform.localScale;

        foreach (Transform child in fractured.transform)
        {
            child.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * AsteroidShardForceMultiplier, ForceMode.Impulse);
        }

        Destroy(gameObject);
        Destroy(fractured, AsteroidShardLifeDuration);
    }
}
