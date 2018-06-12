using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RocketLauncherModule : MonoBehaviour, ITitanModule {
    [SerializeField]
    public int Damage = 5;
    [SerializeField]
    public float Radius = 2f;
    private TitanViewOld titan;

    public int RocketCount = 1;
    public int MaxRocketCount = 2;

    public void Fire(Vector3 fireCoord, PlanetViewOld planet) {
        if (RocketCount <= 0)
            return;
        RocketCount -= 1;
        titan.UpdateState();
        RocketViewOld.Fire(transform.position, fireCoord, planet, Damage, Radius, titan.FactionId);
    }

    public void OnAttach(TitanViewOld titanView) {
        titan = titanView;
    }

    public void OnDetach() {
    }

    public IInterfaceController[] GetInterfaceControllers() {
        return null;
    }
}
