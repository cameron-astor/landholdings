using Godot;
using System;
using System.Collections.Generic;

public class Aristocrat : Stratum
{
    public Aristocrat()
    {
        holdings = new HashSet<Holding>();
        tenants = new HashSet<PeasantFamily>();
    }

    public HashSet<Holding> holdings;
    public HashSet<PeasantFamily> tenants;

    // in-kind tax rate for tenants of this arisrocrat
    public double taxRate { get; set; } = 0.0;

    public new double foodSupply { get; set; } = 50.0;

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
    
    public void AddTenant(PeasantFamily p)
    {
        tenants.Add(p);
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
        GD.Print("Size: " + size);
        GD.Print("Food supply: " + foodSupply);
        GD.Print("Tax rate: " + taxRate);
        GD.Print("Number of tenants: " + tenants.Count);
    }

}
