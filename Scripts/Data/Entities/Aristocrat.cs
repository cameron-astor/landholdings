using Godot;
using System;
using System.Collections.Generic;

public class Aristocrat : Stratum
{

    // Reference to the world
    public WorldTile[,] world { private get; set; }
    public int width { private get; set; }
    public int height { private get; set; }

    public Aristocrat()
    {
        holdings = new HashSet<Holding>();
    }

    public HashSet<Holding> holdings;

    public int id { get; set; } = -1;
    public int size { get; set; } = 0;
    public double foodSupply { get; set; } = 0.0;

    // flags
    public bool dead;

    public void Metabolize()
    {
        if (foodSupply < 0) // dead
        {
            dead = true;
        } else {
            foodSupply -= (double) size / 5.0;
        }        
    }


}
