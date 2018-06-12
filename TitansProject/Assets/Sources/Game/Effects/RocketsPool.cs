using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketsPool : MonoBehaviour {
    private static RocketsPool instance;

    [SerializeField]
    public RocketView laserLinePrototype; // Set from editor

    private ObjectPool<RocketView> cache;

    private void Awake () {
        instance = this;
    }

    private void Start() {
        cache = new ObjectPool<RocketView>(laserLinePrototype);
    }
}
