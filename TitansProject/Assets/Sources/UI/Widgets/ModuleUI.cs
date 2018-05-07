using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Model;
using System;

namespace UI.Widget {
    public sealed class ModuleUI : MonoBehaviour {
        [SerializeField]
        private Image moduleImage; // Set from editor
        [SerializeField]
        private Text moduleName; // Set from editor
        [SerializeField]
        private Text moduleCost; // Set from editor
        [SerializeField]
        private Text moduleTime; // Set from editor
        [SerializeField]
        private Text moduleDescription; // Set from editor

        public RectTransform RectTransform { get; private set; }
        private ModuleData module;
        private Action<string> onSelectModule;

        private void Awake() {
            RectTransform = GetComponent<RectTransform>();
        }

        public void Init(ModuleData moduleData, Action<string> OnSelectModuleBuild) {
            module = moduleData;
            // moduleImage.sprite = 
            onSelectModule = OnSelectModuleBuild;
            moduleName.text = module.Id;
            moduleDescription.text = module.Description;
            moduleCost.text = module.Cost.ToString();
            moduleTime.text = module.BuildTime.ToString("##.#");
        }

        public void OnSelectModuleClick() {
            if (onSelectModule != null && module != null) {
                onSelectModule(module.Id);
            }
        }
    }
}