using System;
using System.Collections.Generic;
using Model;
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
        private ModuleSlot slot;

        public void Init(Vector2 position, ModuleSlot controlSlot) {
            listPanel.anchoredPosition = new Vector2(position.x + 50, 0);
            cancelButtonRect.anchoredPosition = position;
            slot = controlSlot;
        }

        public void OnCancelClick() { // Set from editor
            Hide<SelectModuleUI>();
        }

        protected override void OnShow() {
            if (modules == null)
                CreateAvailableModules();
        }

        protected override void OnHide() { }

        private void CreateAvailableModules() {
            int count = Config.Modules.Modules.Count;
            modules = new List<ModuleUI>(count);
            for (int i = 0; i < count; i++) {
                var module = CreateModule(Config.Modules.Modules[i]);
                module.RectTransform.anchoredPosition = new Vector2(0, moduleSize * -i);
                modules.Add(module);
            }
            contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, moduleSize * count);
        }

        private ModuleUI CreateModule(ModuleData moduleData) {
            modulePrefab.gameObject.SetActive(false);
            var module = Instantiate(modulePrefab, modulePrefab.transform.parent);
            module.gameObject.SetActive(true);
            module.Init(moduleData, OnSelectModuleClick);
            return module;
        }

        private void OnSelectModuleClick(string moduleId) {
            var module = Config.Modules[moduleId];
            if (slot.CanBuild(module)) {
                slot.Build(module);
                UILayer.Hide<SelectModuleUI>();
                UILayer.UpdateInterface();
            } else {
                UILayer.Hide<SelectModuleUI>();
            }
        }
    }
}