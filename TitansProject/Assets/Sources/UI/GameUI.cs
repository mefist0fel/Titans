using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    [SerializeField]
    private Text statusText; // Set from editor
    [SerializeField]
    private BuildModuleUIPanel buildPanel; // Set from editor
    [SerializeField]
    private FireRocketUIPanel fireRocketPanel; // Set from editor

    private TitanView selectedTitan;

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
            buildPanel.SetActive(false);
            fireRocketPanel.SetActive(false);
            Game.Instance.MoveController.HideSelection();
            return;
        }
        string status = "Energy: " + selectedTitan.EnergyUnits + "\n" + "Armor: " + selectedTitan.Armor;
        statusText.text = status;
        buildPanel.SetActive(true);
        fireRocketPanel.SetActive(true);
        buildPanel.UpdatePanel(selectedTitan);
        fireRocketPanel.UpdatePanel(selectedTitan.RocketLauncher);
        Game.Instance.MoveController.ShowPathMarkers(selectedTitan, selectedTitan.GetPathPoints());
    }

    public void OnSelectNextTitanClick() {
        Game.SelectNextTitan();
    } 
}
