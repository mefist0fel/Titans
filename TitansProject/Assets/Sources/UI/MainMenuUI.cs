using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MainMenuUI : UILayer {

    public void OnStartGameClick() { // Set from editor
        SceneManager.LoadScene("Game");
    }

    public void OnExitClick() { // Set from editor
        ExitApplication();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ExitApplication();
        }
    }

    private void ExitApplication() {
        Application.Quit();
    }
}
