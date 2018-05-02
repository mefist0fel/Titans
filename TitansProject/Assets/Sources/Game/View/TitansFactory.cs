using Model;
using UnityEngine;
using View;

public sealed class TitansFactory {
    public static TitanView CreateTitan(Titan titan, Transform parent) {
        var titanPrefabName = "Prefabs/Titans/titan";
        if (titan.Faction.ID == 1)
            titanPrefabName = "Prefabs/Titans/titan_enemy";
        var titanView = GameObject.Instantiate(Resources.Load<TitanView>(titanPrefabName), parent);
        titanView.Init(titan);
        return titanView;
    }

}
