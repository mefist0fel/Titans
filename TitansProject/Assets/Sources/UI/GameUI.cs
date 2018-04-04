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
    private Button UpgradeTitanButton; // Set from editor

    private TitanView selectedTitan;

    public enum ModuleType {}

    private void Start() {
        fireRocketPanel.Init(Game.OnSelectRocketStrike);
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
        }
        UpgradeTitanButton.gameObject.SetActive(selectedTitan.Level < TitanView.MaxLevel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SlotButtons[0].transform.parent.GetComponent<RectTransform>());
    }

    public void OnSelectNextTitanClick() {
        Game.SelectNextTitan();
    }

    public void OnBuildModuleClick(int moduleId) { // Set from editor
        Debug.LogError("Build module " + moduleId);
    }

    public void OnBuildTitanClick() { // Set from editor
    }

    public void OnUpgradeTitanClick() { // Set from editor
    }
}
