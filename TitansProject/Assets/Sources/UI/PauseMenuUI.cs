using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PauseMenuUI : UILayer {
    public void Show() {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void Hide() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void OnContinueButtonClick() {
        Hide();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Hide();
        }
    }

    public void OnExitButtonClick() {
        Hide();
        SceneManager.LoadScene("Main");
    }
}