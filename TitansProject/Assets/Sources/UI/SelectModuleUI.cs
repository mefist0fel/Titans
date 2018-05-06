using System;
using System.Collections.Generic;
using UI.Widget;
using UnityEngine;

namespace UI {
    public sealed class SelectModuleUI : UILayer {
        [SerializeField]
        private RectTransform listPanel; // Set from editor
        [SerializeField]
        private RectTransform contentPanel; // Set from editor
        [SerializeField]
        private RectTransform cancelButtonRect; // Set from editor
        [SerializeField]
        private ModuleUI modulePrefab; // Set from editor
        [SerializeField]
        private float moduleSize = 200f; // Set from editor

        private List<ModuleUI> modules;

        public void SetCancelPosition(Vector2 position) {
            listPanel.anchoredPosition = new Vector2(position.x + 50, 0);
            cancelButtonRect.anchoredPosition = position;
        }

        public void OnCancelClick() { // Set from editor
            Hide<SelectModuleUI>();
        }

        protected override void OnShow() {
            if (modules == null)
                CreateAvailableModules();
        }

        protected override void OnHide() {
        }

        private void CreateAvailableModules() {
            int count = 12;
            modules = new List<ModuleUI>(count);
            for (int i = 0; i < count; i++) {
                var module = CreateModule();
                module.RectTransform.anchoredPosition = new Vector2(0, moduleSize * -i);
                modules.Add(module);
            }
            contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, moduleSize * count);
        }

        private ModuleUI CreateModule() {
            modulePrefab.gameObject.SetActive(false);
            var module = Instantiate(modulePrefab, modulePrefab.transform.parent);
            module.gameObject.SetActive(true);
            return module;
        }
    }
}