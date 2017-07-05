using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockModule
{
    [Tooltip("The name of the component")]
    public string name;

    [Tooltip("The id of the specific component")]
    public int id;
    
    [Tooltip("The distance the bullets can travel")]
    public float range;

    [Tooltip("The damage each bullet does")]
    public float damage;

    [Tooltip("The distance the shot direction can be from the players cursor")]
    public float accuracy;
    
    public StockModule(int id, float range, float damage, float accuracy)
    {
        this.id = id;
        this.range = range;
        this.damage = damage;
        this.accuracy = accuracy;
    }
}
