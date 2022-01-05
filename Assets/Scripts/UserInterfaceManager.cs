using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [SerializeField] private Spaceship spaceship;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas deathCanvas;
    [SerializeField] private TMP_Text deathText;

    private void Awake()
    {
        healthSlider.maxValue = spaceship.MaxHealth;
        healthSlider.value = healthSlider.maxValue;
        deathCanvas.enabled = false;

        spaceship.OnHealthChanged += () => healthSlider.value = spaceship.Health;
        spaceship.OnDeath += () =>
        {
            deathText.text = $"Score: {spaceship.transform.position.z}";
            deathCanvas.enabled = true;
        };
    }
}
