using Godot;
using System;
using System.Collections.Generic;

public class Aristocrat : Stratum
{

    // // Reference to the world
    // public WorldTile[,] world { private get; set; }
    // public int width { private get; set; }
    // public int height { private get; set; }

    public Aristocrat()
    {
        holdings = new HashSet<Holding>();
    }

    public HashSet<Holding> holdings;

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

    public void _InitData(int id, WorldTile[,] world, int width, int height)
    {
        this.world = world;
        this.width = width;
        this.height = height;

        this.id = id;
        this.size = (int) (GD.Randi() % 20) + 5;
    }

    public override void _PrintDebug()
    {
        GD.Print("# ARISTOCRAT SUMMARY #");
        GD.Print("ID: " + id);
    }

}
