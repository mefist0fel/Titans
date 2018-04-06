using System;
using UnityEngine;
using UnityEngine.UI;

public class FireRocketUIPanel : MonoBehaviour {
    [SerializeField]
    private Button fireRocketButton; // Set from editor
    [SerializeField]
    private Button cancelRocketButton; // Set from editor
    [SerializeField]
    private Text rocketsCount; // Set from editor

    private Action selectFireAction;
    private Action cancelFireAction;

    public void OnFireRocketClick() { // Set from editor
        if (selectFireAction != null) {
            selectFireAction();
        }
    }

    public void OnCancelRocketClick() { // Set from editor
        if (selectFireAction != null) {
            selectFireAction();
        }
    }

    public void Init(Action onSelectRocketStrike) {
        selectFireAction = onSelectRocketStrike;
    }

    public void UpdatePanel(int count) {
        fireRocketButton.interactable = count > 0;
        rocketsCount.text = count.ToString();
    }
}
