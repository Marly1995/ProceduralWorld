﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelModule
{
    [Tooltip("The name of the component")]
    public string name;

    [Tooltip("The id of the specific component")]
    public int id;

    [Tooltip("The starting velocity of the bullets")]
    public float shotSpeed;

    [Tooltip("The distance the bullets can travel")]
    public float range;

    [Tooltip("The time delay between when each shot can be fired")]
    public int fireRate;

    [Tooltip("The damage each bullet does")]
    public float damage;

    [Tooltip("The distance the shot direction can be from the players cursor")]
    public float accuracy;

    [Tooltip("The number of shot the gun fires")]
    public int shotCount;

    [Tooltip("The width of the spread of the bullets fired")]
    public float spread;

    public BarrelModule(string type, int id, float shotSpeed, float range, int fireRate, float damage, float accuracy, int shotCount, float spread)
    {
        this.id = id;
        this.shotSpeed = shotSpeed;
        this.range = range;
        this.fireRate = fireRate;
        this.damage = damage;
        this.accuracy = accuracy;
        this.shotCount = shotCount;
        this.spread = spread;
    }
}