using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class OldFaction {
    public readonly int ID;
    public OldFaction EnemyFaction;

    public List<TitanViewOld> Units = new List<TitanViewOld>();

    public int ActiveUnitsCount {
        get {
            int count = 0;
            foreach (var unit in Units) {
                if (unit != null && unit.IsAlive) {
                    count += 1;
                }
            }
            return count;
        }
    }

    public OldFaction(int id) {
        ID = id;
    }

    public void AddUnit(TitanViewOld titanView, Vector3 position) {
        if (titanView == null)
            return;
        Units.Add(titanView);
        titanView.Init(this, position);
    }
}
