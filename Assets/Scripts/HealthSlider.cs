using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Spaceship spaceship;

    private void Awake()
    {
        slider.maxValue = spaceship.MaxHealth;
        slider.value = slider.maxValue;

        spaceship.OnHealthChanged += () => slider.value = spaceship.Health;
        spaceship.OnDeath += () => Debug.Log("You died.");
    }
}
