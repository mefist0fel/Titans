using Model;
using Model.AI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Base class for control AI faction and coordinate actions of individual titans
/// </summary>
public sealed class FactionAIController : MonoBehaviour, Battle.IFactionController {
    private Faction faction;
    private Battle battle;
    private List<TitanAI> titans = new List<TitanAI>();
    private List<ResourcePoint> occupiedResources; // Shared list of resourcePoints for all titans of this faction

    public static FactionAIController Create() {
        var gameObject = new GameObject("AI faction controller");
        var controller = gameObject.AddComponent<FactionAIController>();
        return controller;
    }

    public void Init(Battle controlBattle) {
        battle = controlBattle;
    }

    public void Init(Faction controlFaction) {
        faction = controlFaction;
        name = string.Format("Faction_{0}_AI_controller", faction.ID);
        occupiedResources = new List<ResourcePoint>();
    }

    public void OnAddTitan(Titan titan) {
        Assert.IsNotNull(titan);
        titans.Add(new TitanAI(titan, battle, occupiedResources, Config.DefaultAiCommands));
    }

    public void OnRemoveTitan(Titan titan) {
        Assert.IsNotNull(titan);
        titans.RemoveAll(ai => ai == null || ai.Titan == titan);
    }

    private void OnDestroy() {
        if (faction != null)
            faction.RemoveListener(this);
        faction = null;
    }

    private void Update () {
        foreach (var ai in titans) {
            ai.Update(Time.deltaTime);
        }
	}
}
