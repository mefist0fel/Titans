using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public sealed class TitanCommandQueue : MonoBehaviour {
        [SerializeField]
        private Sprite moveIcon; // Set from editor
        [SerializeField]
        private Sprite attackIcon; // Set from editor
        [SerializeField]
        private TitanCommandButton buttonPrefab; // Set from editor
        [SerializeField]
        private float size = 100;
        [SerializeField]
        private List<TitanCommandButton> queueButtons = new List<TitanCommandButton>();


        void Start() {
            return;
            for (int i = 0; i < 10; i++) {
                queueButtons.Add(CreateButton(i));
            }
        }

        private TitanCommandButton CreateButton(int id) {
            var button = Instantiate(buttonPrefab, transform);
            button.Init(id, OnCommandClick, (id % 2 == 0) ? moveIcon : attackIcon);
            button.RectTransform.localPosition = new Vector3(size * (id + 0.5f), 0);
            return button;
        }

        private void OnCommandClick(int obj) {
            throw new NotImplementedException();
        }

        void Update() {

        }
    }
}