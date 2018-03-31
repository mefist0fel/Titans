using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RocketComponent : MonoBehaviour, ITitanComponent {
    [SerializeField]
    public int Damage = 5;
    [SerializeField]
    public float Radius = 2f;

    public int RocketCount = 10;
    public int MaxRocketCount = 10;

    public void Fire(Vector3 fireCoord, PlanetView planet) {
        if (RocketCount <= 0)
            return;
        RocketCount -= 1;
        RocketView.Fire(transform.position, fireCoord, planet, Damage, Radius);
    }

    public void Attach(TitanView titan) {
    }

    public void Detach() {
    }

    public IInterfaceController[] GetInterfaceControllers() {
        return null;
    }
}
