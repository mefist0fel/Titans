using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RocketComponent : MonoBehaviour, ITitanComponent {
    [SerializeField]
    public int Damage = 5;
    [SerializeField]
    public float Radius = 2f;
    private TitanView titan;

    public int RocketCount = 10;
    public int MaxRocketCount = 10;

    public void Fire(Vector3 fireCoord, PlanetView planet) {
        if (RocketCount <= 0)
            return;
        RocketCount -= 1;
        titan.UpdateState();
        RocketView.Fire(transform.position, fireCoord, planet, Damage, Radius);
    }

    public void Attach(TitanView titanView) {
        titan = titanView;
    }

    public void Detach() {
    }

    public IInterfaceController[] GetInterfaceControllers() {
        return null;
    }
}
