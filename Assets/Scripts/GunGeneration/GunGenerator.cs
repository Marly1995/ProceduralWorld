using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGenerator : MonoBehaviour {

    public GunModule gun;
    public PlayerShotScript weapon;

    public BarrelModule[] barrels = new BarrelModule[]
    {
        new BarrelModule("shotgun", 0, 4.0f, 1.0f, 16, 6, 12.0f, 8, 4.0f),
        new BarrelModule("longBarrelShotgun", 1, 6.0f, 3.0f, 12, 4.0f, 12.0f, 6, 2.0f),
        new BarrelModule("machinegun", 1, 8.0f, 6.0f, 4, 2.0f, 7.0f, 1, 2.0f),
        new BarrelModule("dualMachinegun", 1, 8.0f, 6.0f, 4, 2.0f, 12.0f, 2, 2.0f),
        new BarrelModule("minigun", 1, 8.0f, 6.0f, 1, 2.0f, 24.0f, 1, 6.0f)
    };

    FrameModule[] frames = new FrameModule[]
    {
        new FrameModule("standard", 0, 1.0f, 1.0f, 6, 1.0f, 1.0f),
        new FrameModule("short", 1, 0.8f, 0.6f, 5, 1.0f, 0.6f),
        new FrameModule("long", 2, 1.2f, 1.4f, 10, 1.2f, 1.2f)
    };

    ScopeModule[] scopes = new ScopeModule[]
    {
        new ScopeModule(0, 1.2f, 0.8f, 1.4f),
        new ScopeModule(1, 2.0f, 1.2f, 2.0f),
        new ScopeModule(2, 1.0f, 1.0f, 1.0f)
    };

    StockModule[] stocks = new StockModule[]
    {
        new StockModule(0, 1.0f, 1.4f, 1.0f),
        new StockModule(1, 0.6f, 1.0f, 0.8f),
        new StockModule(2, 1.4f, 1.0f, 1.2f)
    };

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space"))
        {
            GenerateGun();
            weapon.gun = gun;
        }
	}

    void GenerateGun()
    {
        StockModule stock = stocks[Random.Range(0, stocks.Length)];
        FrameModule frame = frames[Random.Range(0, frames.Length)];
        BarrelModule barrel = barrels[Random.Range(0, barrels.Length)];
        ScopeModule scope = scopes[Random.Range(0, scopes.Length)];

        GunModule nGun = new GunModule(barrel, stock, scope, frame);
        gun = nGun;
        Debug.Log("damage: " + gun.damage);
    }
}
