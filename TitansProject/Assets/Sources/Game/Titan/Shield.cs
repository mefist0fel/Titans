using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Shield {
    public int Value { get; private set; }
    public int MaxValue { get; private set; }

    public const float reloadTime = 10f;
    private float timer = 0;
    private TitanView titan;

    public Shield(TitanView parentTitan) {
        titan = parentTitan;
    }

    public void Update(float deltaTime) {
        if (Value >= MaxValue)
            return;
        if (!titan.IsAlive)
            return;
        if (timer < 0) {
            Value += 1;
            timer = reloadTime;
            titan.UpdateState();
        }
        timer -= deltaTime;
    }

    public int Hit(int damage) {
        if (Value > damage) {
            Value -= damage;
            return 0;
        }
        int damageAfterShield = damage - Value;
        Value = 0;
        return damageAfterShield;
    }

    public void UpdateModificators(List<IModificator> modificators) {
        MaxValue = 0;
        if (modificators == null)
            return;
        foreach (var modificator in modificators) {
            ApplyModificator(modificator as IShieldModificator);
        }
    }

    private void ApplyModificator(IShieldModificator shieldModificator) {
        if (shieldModificator == null)
            return;
        MaxValue += shieldModificator.GetShieldPoints();
    }

    public interface IShieldModificator : IModificator {
        int GetShieldPoints();
    }
}
