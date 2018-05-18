using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public sealed class TitanCommandButton : MonoBehaviour {
        [SerializeField]
        private Image buttonImage; // Set from editor

        private int id;
        private Action<int> onCommandClick;

        public RectTransform RectTransform { get; private set;}

        public void Awake() {
            RectTransform = GetComponent<RectTransform>();
        }

        public void Init(int commandId, Action<int> onTitanCommandClick, Sprite sprite) {
            id = commandId;
            onCommandClick = onTitanCommandClick;
            buttonImage.sprite = sprite;
        }

        public void OnCommandButtonClick() {
            if (onCommandClick != null)
                onCommandClick(id);
        }
    }
}