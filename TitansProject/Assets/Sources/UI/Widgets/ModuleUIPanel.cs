using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModuleUIPanel : MonoBehaviour {
    [SerializeField]
    private Button button; // Set from editor
    [SerializeField]
    private Image image; // Set from editor
    [SerializeField]
    private Image progressImage; // Set from editor
    [SerializeField]
    private UnityEvent slotEvent; // Set from editor
    [SerializeField]
    private UISettings settings; // Set from editor

    private ITitanModule module;

    public void OnModuleClick() {
        slotEvent.Invoke();
    }

    public void SetModule(ITitanModule controlModule) {
        if (module == controlModule)
            return;
        module = controlModule;
        UpdatePanel();
    }

    private void UpdatePanel() {
        image.sprite = GetSprite(module);
        progressImage.fillAmount = 0;
    }

    private void Update() {
        if (module is BuildModule) {
            var buildModule = module as BuildModule;
            progressImage.fillAmount = buildModule.NormalizedTime;
        }
        if (module is ShieldModule) {
            var shieldModule = module as ShieldModule;
            progressImage.fillAmount = 1f - shieldModule.NormalizedTime;
        }
    }

    private Sprite GetSprite(ITitanModule module) {
        if (module == null)
            return settings.NewModuleSprite;
        if (module is BuildModule) {
            progressImage.fillClockwise = false;
            progressImage.color = settings.BuildColor;
            var buildModule = module as BuildModule;
            switch (buildModule.ConstructionModule.Id) {
                case "weapon":
                    return settings.WeaponModuleSprite;
                case "rocket":
                    return settings.RocketModuleSprite;
                case "shield":
                    return settings.ShieldModuleSprite;
                case "anti_air":
                    return settings.AntiAirModuleSprite;
            }
        }
        if (module is WeaponModule)
            return settings.WeaponModuleSprite;
        if (module is RocketLauncherModule)
            return settings.RocketModuleSprite;
        if (module is AntiAirLaserModule)
            return settings.AntiAirModuleSprite;
        if (module is ShieldModule) {
            progressImage.fillClockwise = true;
            progressImage.color = settings.ShieldRestoreColor;
            return settings.ShieldModuleSprite;
        }

        return null;
    }
}
