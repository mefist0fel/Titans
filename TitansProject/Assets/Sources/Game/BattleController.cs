using UnityEngine;
using Model;
using View;
using System;
using System.Collections.Generic;
using UI;

public sealed class BattleController : MonoBehaviour, IBattleController {
    [SerializeField]
    private PlanetView planetView; // Set from editor
    [SerializeField]
    private PlayerFactionController playerFactionController; // Set from editor
    [SerializeField]
    private List<TitanView> titanList = new List<TitanView>();

    private Battle battle;

	public void Start () {
        battle = new Battle(this);
        playerFactionController = Utils.GetOrCreateComponent<PlayerFactionController>(gameObject);
        playerFactionController.Init(battle, battle.Factions[0]);
        planetView.Init(battle.Planet);
	}

	private void Update () {
        battle.Update(Time.deltaTime);
    }

    public void OnCreateTitan(Titan titan) {
        var titanView = TitansFactory.CreateTitan(titan, transform);
        titanList.Add(titanView);
        GameUI gameUI = UILayer.Get<GameUI>();
        if (gameUI != null) {
            gameUI.AddMarker(titanView, planetView);
        }
    }

    public void OnBattleEnd(Faction winner) {
        Debug.LogError("Battle end, and winner is " + winner.ID);
    }
}
