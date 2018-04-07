using UnityEngine;

public class ShieldView : MonoBehaviour {
    [SerializeField]
    private MeshRenderer shieldDome; // Set from editor
    [SerializeField]
    private Color shieldColor = Color.white; // Set from editor
    [SerializeField]
    private float appearTime = 0.2f; // Set from editor

    private int currentShieldValue = -1;

    public void UpdateState(Shield shield) {
        if (shield.Value != currentShieldValue) {
            shieldDome.gameObject.SetActive(shield.Value > 0);
            if (currentShieldValue == 0) {
                shieldDome.material.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, 0);
                shieldDome.transform.localScale = Vector3.one * (0.7f);
                Timer.Add(appearTime, (anim) => {
                    shieldDome.material.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, shieldColor.a * anim * anim);
                    shieldDome.transform.localScale = Vector3.one * (0.7f + 0.3f * anim * anim);
                }, () => {
                    shieldDome.material.color = shieldColor;
                    shieldDome.transform.localScale = Vector3.one;
                });
            }
            currentShieldValue = shield.Value;
        }
    }
}
