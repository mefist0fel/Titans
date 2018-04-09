using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameUI : MonoBehaviour {
    [SerializeField]
    private Text statusText; // Set from editor
    [SerializeField]
    private GameObject modulesPanel; // Set from editor
    [SerializeField]
    private ModuleUIPanel[] modules = new ModuleUIPanel[12]; // Set from editor
    [SerializeField]
    private ModuleUIPanel buildTitanModule; // Set from editor
    [SerializeField]
    private ModuleUIPanel upgradeTitanModule; // Set from editor
    [SerializeField]
    private RectTransform BuildContextMenu; // Set from editor
    [SerializeField]
    private Button FullScreenHolder; // Set from editor

    // Rockets panel
    [SerializeField]
    private GameObject skillPanel; // Set from editor
    [SerializeField]
    private ModuleUIPanel OnBuildRocketButton; // Set from editor
    [SerializeField]
    private Button OnFireRocketButton; // Set from editor
    [SerializeField]
    private Button OnCancelRocketButton; // Set from editor
    [SerializeField]
    private Text RocketsCount; // Set from editor

    private TitanView selectedTitan;
    private int selectedSlot = 0;

    public enum ModuleType {}

    private void Start() {
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
            skillPanel.SetActive(false);
            Game.Instance.MoveController.HideSelection();
            HideContextMenu();
            return;
        }
        string status = "Energy: " + selectedTitan.ResourceUnits;
        statusText.text = status;
        modulesPanel.SetActive(true);
        UpdateModules();
        var maxRocket = selectedTitan.GetComponentMaxRocketsCount();
        skillPanel.SetActive(maxRocket > 0);
        if (maxRocket > 0) {
            OnFireRocketButton.gameObject.SetActive(!RocketAimView.IsActive());
            OnCancelRocketButton.gameObject.SetActive(RocketAimView.IsActive());
            var rocketCount = selectedTitan.GetComponentRocketsCount();
            RocketsCount.text = rocketCount + "/" + maxRocket;
            OnFireRocketButton.interactable = rocketCount > 0;
        }
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
        upgradeTitanModule.SetModule(selectedTitan.Modules[13]);
        upgradeTitanModule.gameObject.SetActive(selectedTitan.Level < TitanView.MaxLevel);
        OnBuildRocketButton.SetModule(selectedTitan.Modules[14]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(modules[0].transform.parent.GetComponent<RectTransform>());
    }

    public void OnSelectNextTitanClick() {
        Game.SelectNextTitan();
    }

    public void OnBuildModuleClick(int moduleId) { // Set from editor
        if (selectedTitan.Modules[moduleId] != null)
            return;
        selectedSlot = moduleId;
        FullScreenHolder.gameObject.SetActive(true);
        BuildContextMenu.gameObject.SetActive(true);
        BuildContextMenu.position = modules[moduleId].transform.position;
    }

    public void OnBuildTitanClick() { // Set from editor
        if (selectedTitan.Modules[12] != null)
            return;
        selectedTitan.BuildTitan(Config.Modules["titan"]);
    }

    public void OnUpgradeTitanClick() { // Set from editor
        if (selectedTitan.Modules[13] != null)
            return;
        selectedTitan.BuildUpgrade(Config.Modules["titan_upgrade"]);
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

    public void OnSelectRocketFireButton() {
        Game.OnSelectRocketStrike();
        OnFireRocketButton.gameObject.SetActive(false);
        OnCancelRocketButton.gameObject.SetActive(true);
    }
    public void OnCancelRocketFireButton() {
        RocketAimView.Hide();
        OnFireRocketButton.gameObject.SetActive(true);
        OnCancelRocketButton.gameObject.SetActive(false);
    }
    public void OnAddRocketButton() {
        if (selectedTitan.Modules[14] != null)
            return;
        var rocketCount = selectedTitan.GetComponentRocketsCount();
        var maxRocketCount = selectedTitan.GetComponentMaxRocketsCount();
        if (rocketCount >= maxRocketCount)
            return;
        selectedTitan.BuildRocket(Config.Modules["add_rocket"]);
    }

    public void CancelContextMenu() { // Set from editor
        HideContextMenu();
    }

    private void HideContextMenu() {
        FullScreenHolder.gameObject.SetActive(false);
        BuildContextMenu.gameObject.SetActive(false);
    }
}
