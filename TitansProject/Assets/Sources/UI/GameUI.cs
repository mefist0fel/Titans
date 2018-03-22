using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    [SerializeField]
    private Text statusText; // Set from editor

    public void SetStatusText(string text) {
        statusText.text = text;
    }

	void Start () {
		
	}
	
	void Update () {
		
	}
}
