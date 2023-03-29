using UnityEngine;

public class UIController : MonoBehaviour
{

    public static UIController Instance;
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }

    [SerializeField] GameObject pauseMenu;
    public bool isPaused;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            togglePause();
        }
    }

    public void togglePause() {
        if(isPaused) {
            // Resume Game
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;

        }
    }

    public void NewGame() {
        togglePause();
        GameController.Instance.SetupGame();
    }

    public void QuitGame() {
        Application.Quit();
    }


    


}
