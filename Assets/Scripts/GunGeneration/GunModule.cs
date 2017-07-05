using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModule
{
    FrameModule frame;
    StockModule stock;
    ScopeModule scope;
    BarrelModule barrel;

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
	
    public GunModule(BarrelModule barrel, StockModule stock, ScopeModule scope, FrameModule frame)
    {
        this.barrel = barrel;
        this.scope = scope;
        this.stock = stock;
        this.frame = frame;
        Initialise();
    }

    void Initialise()
    {
        shotSpeed = barrel.shotSpeed * frame.shotSpeed;
        range = barrel.range * frame.range * scope.range * stock.range;
        fireRate = barrel.fireRate + frame.fireRate;
        damage = barrel.damage * frame.damage * stock.damage * scope.damage;
        accuracy = barrel.accuracy * frame.accuracy * stock.accuracy * scope.accuracy;
        shotCount = barrel.shotCount;
        spread = barrel.spread;
    }
}
