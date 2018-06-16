using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class AITitanController {
    public readonly Titan Titan;
    public enum Target {
        Wait,
        CollectResources,
        KillAllEnemy,
        DoNothing
    }

    private const float updateTime = 1f;
    private readonly ReloadTimer timer = new ReloadTimer(updateTime);
    private Target target = Target.CollectResources;

    public AITitanController(Titan titan) {
        Titan = titan;
    }

    public void Update(float deltaTime) {
        timer.Update(deltaTime);
        if (timer.IsReady) {

        }
    }
}
