using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public sealed class PauseMenuUI : UILayer {
        protected override void OnShow() {
            Time.timeScale = 0;
        }

        protected override void OnHide() {
            Time.timeScale = 1f;
        }

        public void OnContinueButtonClick() {
            Hide<PauseMenuUI>();
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                Hide<PauseMenuUI>();
            }
        }

        public void OnExitButtonClick() {
            Hide<PauseMenuUI>();
            SceneManager.LoadScene("Main");
        }
    }
}