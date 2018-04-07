using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ShieldModule : MonoBehaviour, ITitanModule, Shield.IShieldModificator {
    [SerializeField]
    private int shield = 10;

    private TitanView titan;

    public int GetShieldPoints() {
        return shield;
    }

    public void OnAttach(TitanView parentTitan) {
        if (parentTitan == null)
            return;
        titan = parentTitan;
        titan.AddModificator(this);
    }

    public void OnDetach() {
        if (titan == null)
            return;
        titan.RemoveModificator(this);
        titan = null;
    }

    public void OnDestroy() {
        OnDetach();
    }
}
