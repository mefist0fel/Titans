using Model;
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
    private UISettings settings; // Set from editor
    [SerializeField]
    private UnityEvent slotEvent; // Set from editor

    private ModuleSlot slot;

    public void OnModuleClick() { // Set from editor
        slotEvent.Invoke();
    }

    public void SetModule(ModuleSlot controlSlot) {
        if (slot == controlSlot)
            return;
        slot = controlSlot;
        UpdatePanel();
    }

    private void UpdatePanel() {
        if (image != null)
            image.sprite = GetSprite(slot);
        progressImage.fillAmount = 0;
    }

    private void Update() {
        if (slot == null)
            return;
        if (slot.Module is BuilderModule) {
            var buildModule = slot.Module as BuilderModule;
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

    private Sprite GetSprite(ModuleSlot slot) {
        if (slot == null)
            return settings.NewModuleSprite;
        if (slot.Module == null)
            return settings.NewModuleSprite;
       // if (slot.Module is BuilderModule) {
       //     progressImage.fillClockwise = false;
       //     progressImage.color = settings.BuildColor;
       //     var buildModule = module as BuildModule;
       //     switch (buildModule.ConstructionModule.Id) {
       //         case "weapon":
       //             return settings.WeaponModuleSprite;
       //         case "rocket":
       //             return settings.RocketModuleSprite;
       //         case "shield":
       //             return settings.ShieldModuleSprite;
       //         case "anti_air":
       //             return settings.AntiAirModuleSprite;
       //     }
       // }
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
       //if (module is WeaponModule)
       //    return settings.WeaponModuleSprite;
       //if (module is RocketLauncherModule)
       //    return settings.RocketModuleSprite;
       //if (module is AntiAirLaserModule)
       //    return settings.AntiAirModuleSprite;
       //if (module is ShieldModule) {
       //    progressImage.fillClockwise = true;
       //    progressImage.color = settings.ShieldRestoreColor;
       //    return settings.ShieldModuleSprite;
       //}

        return null;
    }
}
