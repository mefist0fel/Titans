using UnityEngine;
using Model;
using View;
using System;
using System.Collections.Generic;
using UI;
using Navigation;
using Random = UnityEngine.Random;

public sealed class BattleController : MonoBehaviour, IBattleController {
    [SerializeField]
    private PlanetView planetView; // Set from editor
    [SerializeField]
    private EnviromentBuilder builder; // Set from editor
    [SerializeField]
    private PlayerFactionController playerFactionController; // Set from editor
    [SerializeField]
    private List<TitanView> titanList = new List<TitanView>();

    private Battle battle;
    private GameUI gameUI;


    public void Start () {
        float radius = 10f;
        var excludePoints = new List<ExcludeVolume>();
        if (builder != null) {
            excludePoints = builder.GenerateEnviroment();
            radius = builder.Radius;
        }
        var planet = new Planet(radius, 20, excludePoints);
        battle = new Battle(this, 
            new Battle.IFactionController[] {
                playerFactionController,
                FactionAIController.Create()
            }, planet);
        planetView.Init(battle.Planet);
        CameraController.SetPlanetRadius(battle.Planet.Radius);
    }

    private List<ExcludeVolume> GenerateEnviroment(float radius, int count, float minSize, float maxSize) {
        var volumes = new List<ExcludeVolume>(count);
        for (int i = 0; i < count; i++) {
            volumes.Add(new ExcludeVolume(Random.insideUnitSphere * radius, Random.Range(minSize, maxSize)));
        }
        return volumes;
    }

    private void Update () {
        battle.Update(Time.deltaTime);
    }

    public void OnCreateTitan(Titan titan) {
        var titanView = TitansFactory.CreateTitan(titan, transform);
        titanList.Add(titanView);
        playerFactionController.OnAddTitan(titanView, planetView);
    }

    public void OnRemoveTitan(Titan titan) {
        foreach(var titanView in titanList)
            if (titanView != null && titanView.Titan == titan)
                playerFactionController.OnRemoveTitan(titanView);
        // TODO remove corps
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
        var rocketInteraction = interaction as RocketInteraction;
        if (rocketInteraction != null) {
            RocketsPool.Fire(rocketInteraction);
        }
    }
}
