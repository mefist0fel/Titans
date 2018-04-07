using UnityEngine;

[CreateAssetMenu(fileName = "LivesViewSettings", menuName = "Settings/LivesViewSettings", order = 1)]
public sealed class LivesViewSettings : ScriptableObject {
    public Color ArmorBackColor = new Color(1f, 1f, 1f, 0.25f); // Set from editor
    public Color ShieldBackColor = new Color(0f, 0f, 1f, 0.25f); // Set from editor
    public Color ArmorColor = Color.white; // Set from editor
    public Color ShieldColor = Color.blue; // Set from editor
    public Vector2 Size = new Vector2(0.08f, 0.08f);
    public Vector2 Distance = new Vector2(0.2f, 0.2f);
}
