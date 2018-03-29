using UnityEngine;
using UnityEngine.UI;

public sealed class BuildModuleUIPanel : MonoBehaviour {
    [SerializeField]
    public Button buildWeaponButton; // Set from editor
    [SerializeField]
    public Button buildRocketButton; // Set from editor
    [SerializeField]
    public Button buildShieldButton; // Set from editor

    public void UpdatePanel(TitanView selectedTitan) {

    }
}
