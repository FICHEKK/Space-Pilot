using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    private const float AsteroidShardForceMultiplier = 100;
    private const float AsteroidShardLifeDuration = 3;

    [SerializeField] private GameObject fracturedPrefab;
    private MeshRenderer _meshRenderer;
    private Vector3 _rotationAxis;
    private float _rotationSpeed;

    public float DamageTaken { get; private set; }

    public float Volume
    {
        get
        {
            var bounds = _meshRenderer.bounds;
            var dx = Mathf.Abs(bounds.min.x - bounds.max.x);
            var dy = Mathf.Abs(bounds.min.y - bounds.max.y);
            var dz = Mathf.Abs(bounds.min.z - bounds.max.z);
            return dx * dy * dz;
        }
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rotationAxis = Random.onUnitSphere;
        _rotationSpeed = 2 * Random.value - 1;
    }

    private void Update()
    {
        transform.Rotate(_rotationAxis, _rotationSpeed);
    }

    public void Damage(float amount)
    {
        DamageTaken += amount;
        if (DamageTaken >= Volume) Break();
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
