using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public sealed class LoseUI : UILayer {

        public void Show() {
            Time.timeScale = 0;
            gameObject.SetActive(true);
        }

        public void Hide() {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Hide();
            }
        }

        public void OnExitButtonClick() {
            SceneManager.LoadScene("Main");
            Hide();
        }
    }

}