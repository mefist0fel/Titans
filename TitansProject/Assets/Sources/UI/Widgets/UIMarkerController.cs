using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIMarkerController : MonoBehaviour {
    private TitanView controlledTitan;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Init (TitanView titan) {
        controlledTitan = titan;
        var image = GetComponentInChildren<Image>();
        if (image != null) {
            image.color = titan.SelfFaction.ID == 0 ? Color.blue : Color.red;
        }
	}
	
	private void Update () {
        if (controlledTitan == null)
            return;
        if (rectTransform == null)
            return;
        if (canvas == null)
            return;
        Vector2 border = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 position = ((Vector2)Camera.main.WorldToScreenPoint(controlledTitan.GetMarkerPosition()) - border) * (1f / canvas.scaleFactor);
        float screenBorderPixels = 64f;
        float aspect = Screen.width / (float)Screen.height;
        if (position.x > 512f * aspect - screenBorderPixels) {
            position *= (512f * aspect - screenBorderPixels) / position.x;
        }
        if (position.x < -(512f * aspect - screenBorderPixels)) {
            position *= -(512f * aspect - screenBorderPixels) / position.x;
        }
        if (position.y > 512f - screenBorderPixels) {
            position *= (512f - screenBorderPixels) / position.y;
        }
        if (position.y < -(512f - screenBorderPixels)) {
            position *= -(512f - screenBorderPixels) / position.y;
        }
        rectTransform.anchoredPosition = position;
	}
}
