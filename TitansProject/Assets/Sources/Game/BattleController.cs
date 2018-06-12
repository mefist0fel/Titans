﻿using UnityEngine;
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
    private GameUI gameUI;


    public void Start () {
        gameUI = UILayer.Show<GameUI>();
        battle = new Battle(this);
        playerFactionController = Utils.GetOrCreateComponent<PlayerFactionController>(gameObject);
        playerFactionController.Init(battle, battle.Factions[0]);
        planetView.Init(battle.Planet);
        CameraController.SetPlanetRadius(battle.Planet.Radius);
    }

	private void Update () {
        battle.Update(Time.deltaTime);
    }

    public void OnCreateTitan(Titan titan) {
        var titanView = TitansFactory.CreateTitan(titan, transform);
        titanList.Add(titanView);
        gameUI.AddMarker(titanView, planetView);
    }

    public void OnRemoveTitan(Titan titan) {
    }

    public void OnBattleEnd(Faction winner) {
        Debug.LogError("Battle end, and winner is " + winner.ID);
    }

    public void OnAddInteraction(AbstractInteraction interaction) {
        var laserInteraction = interaction as LaserInteraction;
        if (laserInteraction != null) {
            var titanView = laserInteraction.ParentTitan.View as TitanView;
            var enemyTitanView = laserInteraction.TargetTitan.View as TitanView;
            if (laserInteraction.Damage.Value > 0)
                LaserBeamPool.ShowHit(titanView.GetHitPoint(), enemyTitanView.GetHitPoint(), 0.5f);
            else
                LaserBeamPool.ShowMiss(titanView.GetHitPoint(), enemyTitanView.GetHitPoint(), 0.5f);
        }
        // LaserBeamController
    }
}
