using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Shield {
    public int Value { get; private set; }
    public int MaxValue { get; private set; }
    public float NormalizedRestoreTime { get { return timer / reloadTime; } }

    private int restoreValue = 5;

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
        timer -= deltaTime;
        if (timer < 0) {
            Value = Mathf.Min(MaxValue, Value + restoreValue);
            timer = reloadTime;
            titan.UpdateState();
        }
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
        timer = reloadTime;
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
