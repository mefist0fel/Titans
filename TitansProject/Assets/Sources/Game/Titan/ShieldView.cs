using System;
using UnityEngine;

public sealed class ShieldView : MonoBehaviour {
    [SerializeField]
    private MeshRenderer shieldDome; // Set from editor
    [SerializeField]
    private Color shieldColor = Color.white; // Set from editor
    [SerializeField]
    private ShieldViewSettings viewSettings; // Set from editor

    private int currentShieldValue = 0;

    public void Start() {
        shieldDome.material.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, 0);
        shieldDome.gameObject.SetActive(false);
    }

    public void UpdateState(int shieldValue) {
        if (shieldValue == currentShieldValue)
            return;

        if (currentShieldValue == 0) {
            shieldDome.gameObject.SetActive(true);
            ShowShieldAnimation(viewSettings.AppearAnimation);
        } else {
            if (shieldValue == 0) {
                ShowShieldAnimation(viewSettings.DissapearAnimation);
            } else {
                if (shieldValue > currentShieldValue) {
                    ShowShieldAnimation(viewSettings.RestoreAnimation);
                } else {
                    ShowShieldAnimation(viewSettings.HitAnimation);
                }
            }
        }
        currentShieldValue = shieldValue;
    }

    private void ShowShieldAnimation(ShieldViewSettings.ShieldAnimation param) {
        var dome = shieldDome;
        dome.material.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, shieldColor.a * param.ColorAnimation.Evaluate(0));
        dome.transform.localScale = Vector3.one * param.SizeAnimation.Evaluate(0);
        Timer.Add(param.Time, (anim) => {
            dome.material.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, shieldColor.a * param.ColorAnimation.Evaluate(anim));
            dome.transform.localScale = Vector3.one * param.SizeAnimation.Evaluate(anim);
        }, () => {
            dome.material.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, shieldColor.a * param.ColorAnimation.Evaluate(1));
            dome.transform.localScale = Vector3.one * param.SizeAnimation.Evaluate(1);
            dome.gameObject.SetActive(param.ColorAnimation.Evaluate(1) > 0.01f);
        });
    }

#if UNITY_EDITOR
    // for visual debug
    [ContextMenu("Show appear animation")]
    private void ShowAppearAnimation() {
        Timer.Add(0.5f, () => ShowShieldAnimation(viewSettings.AppearAnimation));
    }

    [ContextMenu("Show hit animation")]
    private void ShowHitAnimation() {
        Timer.Add(0.5f, () => ShowShieldAnimation(viewSettings.HitAnimation));
    }

    [ContextMenu("Show restore animation")]
    private void ShowRestoreAnimation() {
        Timer.Add(0.5f, () => ShowShieldAnimation(viewSettings.RestoreAnimation));
    }

    [ContextMenu("Show dissapear animation")]
    private void ShowDisappearAnimation() {
        Timer.Add(0.5f, () => ShowShieldAnimation(viewSettings.DissapearAnimation));
    }
#endif
}
