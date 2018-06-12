using Model;
using UnityEngine;
using View;

public class RocketsPool : MonoBehaviour {
    private static RocketsPool instance;

    [SerializeField]
    public RocketView rocketPrototype; // Set from editor

    private ObjectPool<RocketView> cache;

    private void Awake () {
        instance = this;
    }

    private void Start() {
        cache = new ObjectPool<RocketView>(rocketPrototype, transform);
    }

    public static void Fire(RocketInteraction rocketInteraction) {
        var rocketView = instance.cache.Get();
        rocketView.Init(rocketInteraction);
    }
}
