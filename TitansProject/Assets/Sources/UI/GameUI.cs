using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    [SerializeField]
    private Text statusText; // Set from editor
    [SerializeField]
    private GameObject modulesPanel; // Set from editor
    [SerializeField]
    private FireRocketUIPanel fireRocketPanel; // Set from editor
    [SerializeField]
    private Button BuildTitanButton; // Set from editor
    [SerializeField]
    private Button[] SlotButtons; // Set from editor
    [SerializeField]
    private Image[] SlotImages; // Set from editor
    [SerializeField]
    private Button UpgradeTitanButton; // Set from editor
    [SerializeField]
    private ImageSettings Settings; // Set from editor
    [SerializeField]
    private RectTransform BuildContextMenu; // Set from editor
    [SerializeField]
    private Button FullScreenHolder; // Set from editor

    [Serializable]
    public sealed class ImageSettings {
        public Sprite NewModuleSprite; // Set from editor
        public Sprite WeaponModuleSprite; // Set from editor
        public Sprite ShieldModuleSprite; // Set from editor
        public Sprite RocketModuleSprite; // Set from editor
        public Sprite AntiAirModuleSprite; // Set from editor
    }

    private TitanView selectedTitan;

    public enum ModuleType {}

    private void Start() {
        fireRocketPanel.Init(Game.OnSelectRocketStrike);
        HideContextMenu();
    }

    public void SelectTitan(TitanView titan = null) {
        if (selectedTitan != null) {
            selectedTitan.UnSubscribe(UpdateTitanStatus);
        }
        selectedTitan = titan;
        if (selectedTitan != null) {
            selectedTitan.Subscribe(UpdateTitanStatus);
        }
        UpdateTitanStatus();
    }

    public void UpdateTitanStatus() {
        if (selectedTitan == null || !selectedTitan.IsAlive) {
            selectedTitan = null;
            statusText.text = "";
            modulesPanel.SetActive(false);
            fireRocketPanel.SetActive(false);
            Game.Instance.MoveController.HideSelection();
            HideContextMenu();
            return;
        }
        string status = "Energy: " + selectedTitan.EnergyUnits + "\n" + "Armor: " + selectedTitan.Armor;
        statusText.text = status;
        modulesPanel.SetActive(true);
        UpdateModules();
        fireRocketPanel.SetActive(true);
        fireRocketPanel.UpdatePanel(selectedTitan.RocketLauncher);
        Game.Instance.MoveController.ShowPathMarkers(selectedTitan, selectedTitan.GetPathPoints());
    }

    private void UpdateModules() {
        for (int i = 0; i < SlotButtons.Length; i++) {
            bool needShow = false;
            if (selectedTitan.SlotLevel.Length > i)
                needShow = selectedTitan.SlotLevel[i] <= selectedTitan.Level;
            if (SlotButtons[i] != null) {
                SlotButtons[i].gameObject.SetActive(needShow);
            }
            ITitanModule module = null;
            if (selectedTitan.Modules.Length > i)
                module = selectedTitan.Modules[i];
            if (SlotImages[i] != null) {
                SlotImages[i].sprite = SetSprite(module);
            }
        }
        UpgradeTitanButton.gameObject.SetActive(selectedTitan.Level < TitanView.MaxLevel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SlotButtons[0].transform.parent.GetComponent<RectTransform>());
    }

    private Sprite SetSprite(ITitanModule module) {
        if (module == null)
            return Settings.NewModuleSprite;
        if (module is WeaponModule)
            return Settings.WeaponModuleSprite;
        if (module is RocketLauncherModule)
            return Settings.RocketModuleSprite;
        if (module is AntiAirLaserModule)
            return Settings.AntiAirModuleSprite;
        // if (module is ShieldModule)
        //     return Settings.ShieldModuleSprite;

        return null;
    }

    public void OnSelectNextTitanClick() {
        Game.SelectNextTitan();
    }

    public void OnBuildModuleClick(int moduleId) { // Set from editor
        Debug.LogError("Build module " + moduleId);
        FullScreenHolder.gameObject.SetActive(true);
        BuildContextMenu.gameObject.SetActive(true);
        BuildContextMenu.position = SlotButtons[moduleId].transform.position;
    }

    public void OnBuildTitanClick() { // Set from editor
        Debug.LogError("Build titan click ");
    }

    public void OnUpgradeTitanClick() { // Set from editor
        Debug.LogError("Build upgrade click ");
    }

    public void OnSelectBuildWeaponModuleClick() { // Set from editor
        Debug.LogError("OnSelectBuildWeaponModuleClick click ");
        HideContextMenu();
    }

    public void OnSelectBuildRocketModuleClick() { // Set from editor
        Debug.LogError("OnSelectBuildRocketModuleClick click ");
        HideContextMenu();
    }

    public void OnSelectBuildShieldModuleClick() { // Set from editor
        Debug.LogError("OnSelectBuildShieldModuleClick click ");
        HideContextMenu();
    }

    public void OnSelectBuildAntiAirModuleClick() { // Set from editor
        Debug.LogError("OnSelectBuildAntiAirModuleClick click ");
        HideContextMenu();
    }

    public void CancelContextMenu() { // Set from editor
        HideContextMenu();
    }

    private void HideContextMenu() {
        FullScreenHolder.gameObject.SetActive(false);
        BuildContextMenu.gameObject.SetActive(false);
    }
}
