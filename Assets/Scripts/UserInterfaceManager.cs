using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [SerializeField] private Spaceship spaceship;

    [Header("Gameplay canvas")]
    [SerializeField] private Canvas gameplayCanvas;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider healthSlider;

    [Header("Death canvas")]
    [SerializeField] private Canvas deathCanvas;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private Button flyAgainButton;
    [SerializeField] private Button exitButton;
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
        exitButton.onClick.AddListener(Application.Quit);

        spaceship.OnDeath += () =>
        {
            gameplayCanvas.enabled = false;
            deathCanvas.enabled = true;
            deathText.text = $"You flew {spaceship.transform.position.z:F0} meters in deep space!";
        };
    }

    private void Update()
    {
        if (spaceship == null) return;
        scoreText.text = $"{spaceship.transform.position.z:F0}m";
    }
}
