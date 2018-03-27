using UnityEngine;
using UnityEngine.UI;

public class FireRocketUIPanel : MonoBehaviour {
    [SerializeField]
    private Button fireRocketButton; // Set from editor
    [SerializeField]
    private Text rocketsCount; // Set from editor

    public void OnFireRocketClick() { // Set from editor
        Debug.LogError("OnFireRocket");
    }

	// Use this for initialization
	private void Start () {
        fireRocketButton.onClick.RemoveAllListeners();
        fireRocketButton.onClick.AddListener(OnFireRocketClick);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
