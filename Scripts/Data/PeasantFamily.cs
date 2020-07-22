using Godot;
using System;
using System.Collections.Generic;

public class PeasantFamily
{

    public PeasantFamily()
    {
        holdings = new HashSet<Holding>();
    }

    public HashSet<Holding> holdings { get; private set; } // the holdings of the peasant
    public int id  { get; set; } = -1;
    public int size { get; set; } = 0;
    public double foodSupply { get; set; } = 0.0;

    // flags
    public bool dead { get; private set; }= false;

    public void _Sow()
    {

    }

    public void _Harvest()
    {
        foreach (Holding h in holdings)
        {
            foreach (WorldTile t in h.constituentTiles)
            {
                double factor = t.food / 4;
                t.food = t.food - factor;
                foodSupply += factor;          
            }
        }
    }

    public void _Metabolize()
    {
        if (foodSupply < 0) // dead
        {
            dead = true;
        } else {
            foodSupply -= (double) size / 5.0;
        }
    }

}
