using UnityEngine;

[CreateAssetMenu(fileName = "UISettings", menuName = "Settings/UISettings", order = 1)]
public sealed class UISettings : ScriptableObject {
    public Sprite NewModuleSprite; // Set from editor
    public ModuleImage[] modules; // Set from editor
    public Sprite Empty; // Set from editor
    public Color BuildColor = Color.gray; // Set from editor
    public Color ShieldRestoreColor = Color.blue; // Set from editor

    [System.Serializable]
    public sealed class ModuleImage {
        public string Name;
        public Sprite Sprite;
    }

    public Sprite this[string name] {
        get {
            if (modules == null)
                return null;
            foreach (var module in modules)
                if (module.Name == name)
                    return module.Sprite;
            return Empty;
        }
    }
}
