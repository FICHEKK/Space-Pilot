using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [SerializeField] private Spaceship spaceship;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas deathCanvas;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private Button flyAgainButton;
    [SerializeField] private string flyAgainSceneToLoad;

    private void Awake()
    {
        InitEnergySlider();
        InitHealthSlider();
        InitDeathCanvas();
    }

    private void InitEnergySlider()
    {
        energySlider.maxValue = spaceship.MaxEnergy;
        energySlider.value = energySlider.maxValue;
        spaceship.OnEnergyChanged += () => energySlider.value = spaceship.Energy;
    }

    private void InitHealthSlider()
    {
        healthSlider.maxValue = spaceship.MaxHealth;
        healthSlider.value = healthSlider.maxValue;
        spaceship.OnHealthChanged += () => healthSlider.value = spaceship.Health;
    }

    private void InitDeathCanvas()
    {
        deathCanvas.enabled = false;
        flyAgainButton.onClick.AddListener(() => SceneManager.LoadScene(flyAgainSceneToLoad));

        spaceship.OnDeath += () =>
        {
            deathText.text = $"Score: {spaceship.transform.position.z:F0}";
            deathCanvas.enabled = true;
        };
    }
}
