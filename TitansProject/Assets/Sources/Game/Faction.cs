using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Faction {
    public readonly int ID;
    public Faction EnemyFaction;
    
    public List<TitanView> Units = new List<TitanView>();

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
