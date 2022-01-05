using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [SerializeField] private Spaceship spaceship;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas deathCanvas;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private Button flyAgainButton;
    [SerializeField] private string flyAgainSceneToLoad;

    private void Awake()
    {
        healthSlider.maxValue = spaceship.MaxHealth;
        healthSlider.value = healthSlider.maxValue;
        deathCanvas.enabled = false;

        spaceship.OnHealthChanged += () => healthSlider.value = spaceship.Health;

        spaceship.OnDeath += () =>
        {
            deathText.text = $"Score: {spaceship.transform.position.z:F0}";
            deathCanvas.enabled = true;
        };

        flyAgainButton.onClick.AddListener(() => SceneManager.LoadScene(flyAgainSceneToLoad));
    }
}
