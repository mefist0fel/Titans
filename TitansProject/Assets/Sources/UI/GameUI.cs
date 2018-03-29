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
        if (selectedTitan == null) {
            statusText.text = "";
            buildPanel.SetActive(false);
            fireRocketPanel.SetActive(false);
            return;
        }
        string status = "Energy: " + selectedTitan.EnergyUnits + "\n" + "Armor: " + selectedTitan.Armor;
        statusText.text = status;
        buildPanel.SetActive(true);
        fireRocketPanel.SetActive(true);
        buildPanel.UpdatePanel(selectedTitan);
    }

    public void OnSelectNextTitanClick() {
        Game.SelectNextTitan();
    } 
}
