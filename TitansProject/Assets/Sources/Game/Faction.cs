using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Faction {
    public readonly int ID;
    public Faction EnemyFaction;
    
    public List<TitanView> Units = new List<TitanView>();

    public int ActiveUnitsCount { get {
            int count = 0;
            foreach (var unit in Units) {
                if (unit != null && unit.IsAlive) {
                    count += 1;
                }
            }
            return count;
        }
    }

    public Faction(int id) {
        ID = id;
    }

    public void AddUnit(TitanView titanView, Vector3 position) {
        if (titanView == null)
            return;
        Units.Add(titanView);
        titanView.Init(this, position);
    }
}
