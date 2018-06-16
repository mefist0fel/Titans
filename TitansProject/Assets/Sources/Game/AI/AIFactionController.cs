using Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Base class for control AI faction and coordinate actions of individual titans
/// </summary>
public sealed class AIFactionController : MonoBehaviour, Faction.Listener {
    private Faction faction;
    private Battle battle;
    private List<AITitanController> titans = new List<AITitanController>();

  //  private 

    public static AIFactionController Create(Faction faction, Battle battle) {
        var factionName = string.Format("Faction_{0}_AI_controller", faction.ID);
        var gameObject = new GameObject(factionName);
        var controller = gameObject.AddComponent<AIFactionController>();
        controller.Init(faction, battle);
        return controller;
    }

    public void OnAddTitan(Titan titan) {
        Assert.IsNotNull(titan);
        titans.Add(new AITitanController(titan));
    }

    public void OnRemoveTitan(Titan titan) {
        Assert.IsNotNull(titan);
        titans.RemoveAll(ai => ai == null || ai.Titan == titan);
    }

    private void Init (Faction controlFaction, Battle controlBattle) {
        battle = controlBattle;
        faction = controlFaction;
        faction.AddListener(this);
    }

    private void OnDestroy() {
        if (faction != null)
            faction.RemoveListener(this);
    }

    private void Update () {
        foreach (var ai in titans) {
            ai.Update(Time.deltaTime);
        }
	}
}
