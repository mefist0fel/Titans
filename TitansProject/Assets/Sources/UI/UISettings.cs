using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UISettings", menuName = "Settings/UISettings", order = 1)]
public sealed class UISettings : ScriptableObject {
    public Sprite NewModuleSprite; // Set from editor
    public Sprite WeaponModuleSprite; // Set from editor
    public Sprite ShieldModuleSprite; // Set from editor
    public Sprite RocketModuleSprite; // Set from editor
    public Sprite AntiAirModuleSprite; // Set from editor
    public Color BuildColor = Color.gray; // Set from editor
    public Color ShieldRestoreColor = Color.blue; // Set from editor
}
