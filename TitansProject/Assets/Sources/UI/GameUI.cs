using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameUI : MonoBehaviour {
    [SerializeField]
    private Text statusText; // Set from editor
    [SerializeField]
    private GameObject modulesPanel; // Set from editor
    [SerializeField]
    private FireRocketUIPanel fireRocketPanel; // Set from editor
    [SerializeField]
    private Button BuildTitanButton; // Set from editor
    [SerializeField]
    private ModuleUIPanel[] modules = new ModuleUIPanel[12]; // Set from editor
    [SerializeField]
    private ModuleUIPanel buildTitanModule; // Set from editor
    [SerializeField]
    private Button UpgradeTitanButton; // Set from editor
    [SerializeField]
    private RectTransform BuildContextMenu; // Set from editor
    [SerializeField]
    private Button FullScreenHolder; // Set from editor

    private TitanView selectedTitan;
    private int selectedSlot = 0;

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
        string status = "Energy: " + selectedTitan.ResourceUnits + "\n" + "Armor: " + selectedTitan.Armor;
        statusText.text = status;
        modulesPanel.SetActive(true);
        UpdateModules();
        fireRocketPanel.SetActive(true);
        fireRocketPanel.UpdatePanel(0);
        Game.Instance.MoveController.ShowPathMarkers(selectedTitan, selectedTitan.GetPathPoints());
    }

    private void UpdateModules() {
        for (int i = 0; i < modules.Length; i++) {
            bool needShow = false;
            if (selectedTitan.SlotLevel.Length > i)
                needShow = selectedTitan.SlotLevel[i] <= selectedTitan.Level;
            if (modules[i] != null) {
                modules[i].SetActive(needShow);
            }
            ITitanModule module = null;
            if (selectedTitan.Modules.Length > i)
                module = selectedTitan.Modules[i];
            if (modules[i] != null) {
                modules[i].SetModule(module);
            }
        }
        buildTitanModule.SetModule(selectedTitan.Modules[12]);
        UpgradeTitanButton.gameObject.SetActive(selectedTitan.Level < TitanView.MaxLevel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(modules[0].transform.parent.GetComponent<RectTransform>());
    }

    public void OnSelectNextTitanClick() {
        Game.SelectNextTitan();
    }

    public void OnBuildModuleClick(int moduleId) { // Set from editor
        selectedSlot = moduleId;
        FullScreenHolder.gameObject.SetActive(true);
        BuildContextMenu.gameObject.SetActive(true);
        BuildContextMenu.position = modules[moduleId].transform.position;
    }

    public void OnBuildTitanClick() { // Set from editor
        selectedTitan.BuildTitan(Config.Modules["titan"]);
    }

    public void OnUpgradeTitanClick() { // Set from editor
        Debug.LogError("Build upgrade click ");
    }

    public void OnSelectBuildWeaponModuleClick() { // Set from editor
        var module = Config.Modules["weapon"];
        selectedTitan.BuildModule(module, selectedSlot);
        HideContextMenu();
    }

    public void OnSelectBuildRocketModuleClick() { // Set from editor
        var module = Config.Modules["rocket"];
        selectedTitan.BuildModule(module, selectedSlot);
        HideContextMenu();
    }

    public void OnSelectBuildShieldModuleClick() { // Set from editor
        var module = Config.Modules["shield"];
        selectedTitan.BuildModule(module, selectedSlot);
        HideContextMenu();
    }

    public void OnSelectBuildAntiAirModuleClick() { // Set from editor
        var module = Config.Modules["anti_air"];
        selectedTitan.BuildModule(module, selectedSlot);
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
