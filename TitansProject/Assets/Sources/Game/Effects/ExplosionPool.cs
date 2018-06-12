using UnityEngine;
using View;

public sealed class ExplosionPool : MonoBehaviour {
    private static ExplosionPool instance;

    private ObjectPool<ExplosionView> cache;

    [SerializeField]
    public ExplosionView prototype; // Set from editor

    private void Awake() {
        instance = this;
    }

    private void Start() {
        cache = new ObjectPool<ExplosionView>(prototype);
    }

    public static void Explode(Vector3 position, float radius) {
        var explosion = instance.cache.Get();
        explosion.Init(position, radius);
    }
}
