using Model;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI {
    public sealed class ModuleUIPanel : MonoBehaviour {
        [SerializeField]
        private Button button; // Set from editor
        [SerializeField]
        private Image image; // Set from editor
        [SerializeField]
        private Image progressImage; // Set from editor
        [SerializeField]
        private UISettings settings; // Set from editor
        [SerializeField]
        private UnityEvent slotEvent; // Set from editor

        public RectTransform RectTransform { get; private set; }
        private IModule module;

        public void OnModuleClick() { // Set from editor
            slotEvent.Invoke();
        }

        public void SetModule(ModuleSlot controlSlot) {
            if (module == controlSlot.Module)
                return;
            module = controlSlot.Module;
            UpdatePanel();
        }

        private void Start() {
            RectTransform = GetComponent<RectTransform>();
        } 

        private void UpdatePanel() {
            if (image != null)
                image.sprite = GetSprite(module);
            progressImage.fillAmount = 0;
        }

        private void Update() {
            if (module == null)
                return;
            if (module is BuilderModule) {
                var buildModule = module as BuilderModule;
                progressImage.fillAmount = buildModule.NormalizedTime;
            }
            //   if (slot is BuildTitanModule) {
            //       var buildModule = slot as BuildTitanModule;
            //       progressImage.fillAmount = buildModule.NormalizedTime;
            //   }
            //   if (slot is BuildUpgradeModule) {
            //       var buildModule = slot as BuildUpgradeModule;
            //       progressImage.fillAmount = buildModule.NormalizedTime;
            //   }
            //   if (slot is BuildRocketModule) {
            //       var buildModule = slot as BuildRocketModule;
            //       progressImage.fillAmount = buildModule.NormalizedTime;
            //   }
            //   if (slot is ShieldModule) {
            //       var shieldModule = slot as ShieldModule;
            //       progressImage.fillAmount = 1f - shieldModule.NormalizedTime;
            //   }
        }

        private Sprite GetSprite(IModule module) {
            if (module == null)
                return settings.NewModuleSprite;
            if (module is BuilderModule) {
                progressImage.fillClockwise = false;
                progressImage.color = settings.BuildColor;
                var buildModule = module as BuilderModule;
                return settings[buildModule.Id];
            }
            //if (module is BuildTitanModule) {
            //    progressImage.fillClockwise = false;
            //    progressImage.color = settings.BuildColor;
            //}
            //if (module is BuildUpgradeModule) {
            //    progressImage.fillClockwise = false;
            //    progressImage.color = settings.BuildColor;
            //}
            //if (module is BuildRocketModule) {
            //    progressImage.fillClockwise = false;
            //    progressImage.color = settings.BuildColor;
            //}
            //if (module is RocketLauncherModule)
            //    return settings.RocketModuleSprite;
            //if (module is AntiAirLaserModule)
            //    return settings.AntiAirModuleSprite;
           // if (module is ShieldCapacitorModule) {
           //     progressImage.fillClockwise = true;
           //     progressImage.color = settings.ShieldRestoreColor;
           //     return settings[module.Id];
           // }
           // if (module is LaserModule)
           //     return settings[module.Id];
            return null;
        }
    }

}