using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;  // The correct namespace import

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public Button menuButton;
    public Button respawnButton;
    public DepthMeter depthMeter;
    private GameObject player;

    private void Awake()
    {
        // Hide the game over screen initially
        gameObject.SetActive(false);
    }

    private void Start()
    {
        // Set up button listeners
        menuButton.onClick.AddListener(ReturnToMenu);
        respawnButton.onClick.AddListener(Respawn);
        
        // Find the player in scene (if applicable)
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ShowGameOver()
    {
        // Update the score text
        scoreText.text = $"Score: {depthMeter.depth.ToString("F0")} m";
        
        // Show the game over screen
        gameObject.SetActive(true);
        
        // Pause the game (optional)
        //Time.timeScale = 0f;
    }

    void ReturnToMenu()
    {
        // Resume normal time scale before switching scenes
        Time.timeScale = 1f;
        
        // Load the menu scene
        SceneManager.LoadScene(0);
    }

    void Respawn()
    {
        // Reset the time scale to normal
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}