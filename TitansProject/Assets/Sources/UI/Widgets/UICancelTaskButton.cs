using System;
using UnityEngine;

namespace UI {
    /// <summary>
    /// class controls button, that cancel command from move/collect/attack queue
    /// </summary>
    public sealed class UICancelTaskButton : MonoBehaviour {
        [SerializeField]
        private Vector2 offset = new Vector2(0, -100); // Set from editor

        private Vector3 position;
        private RectTransform rectTransform; 
        private Canvas canvas;

        private int id;
        private Action<int> onCancelClickAction;

        public void Show(Vector3 taskPosition, int taskId, Action<int> cancelAction) {
            position = taskPosition;
            id = taskId;
            onCancelClickAction = cancelAction;
            gameObject.SetActive(true);
            Update();
        }

        public void Hide() {
            gameObject.SetActive(false);
            onCancelClickAction = null;
        }

        public void OnCancelClick() { // Set from editor
            if (onCancelClickAction != null)
                onCancelClickAction(id);
        }

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
        }

        private void Update() {
            var camera = Camera.main;
            Vector2 border = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 screenPosition = ((Vector2)camera.WorldToScreenPoint(position) - border) * (1f / canvas.scaleFactor);
            rectTransform.anchoredPosition = screenPosition + offset;
        }
    }
}
