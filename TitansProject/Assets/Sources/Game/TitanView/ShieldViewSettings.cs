using UnityEngine;

[CreateAssetMenu(fileName = "ShieldViewSettings", menuName = "Settings/ShieldViewSettings", order = 1)]
public sealed class ShieldViewSettings : ScriptableObject {
    public ShieldAnimation AppearAnimation = new ShieldAnimation(); // Set from editor
    public ShieldAnimation DissapearAnimation = new ShieldAnimation(); // Set from editor
    public ShieldAnimation HitAnimation = new ShieldAnimation(); // Set from editor
    public ShieldAnimation RestoreAnimation = new ShieldAnimation(); // Set from editor

    [System.Serializable]
    public sealed class ShieldAnimation {
        public float Time;
        public AnimationCurve SizeAnimation = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // Set from editor
        public AnimationCurve ColorAnimation = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // Set from editor
    }
}
