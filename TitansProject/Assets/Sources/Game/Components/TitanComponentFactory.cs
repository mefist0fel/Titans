using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanComponentFactory {

    public static WeaponComponent AttachWeapon(TitanView titan) {
        var weaponPrefab = Resources.Load<WeaponComponent>("Prefabs/Components/weapon_component");
        var weapon = GameObject.Instantiate<WeaponComponent>(weaponPrefab, titan.transform);
        weapon.Attach(titan);
        return weapon;
    }

    public static RocketComponent AttachRocketLauncher(TitanView titan) {
        var weaponPrefab = Resources.Load<RocketComponent>("Prefabs/Components/rocket_component");
        var weapon = GameObject.Instantiate<RocketComponent>(weaponPrefab, titan.transform);
        weapon.Attach(titan);
        return weapon;
    }
}
