using Godot;
using System;
using System.Collections.Generic;

public class PeasantFamily
{

    // Reference to the world
    public WorldTile[,] world { private get; set; }
    public int width { private get; set; }
    public int height { private get; set; }

    public PeasantFamily()
    {
        holdings = new HashSet<Holding>();
        adjacencies = new HashSet<WorldTile>();
    }

    public HashSet<Holding> holdings { get; private set; } // the holdings of the peasant
    public HashSet<WorldTile> adjacencies { get; private set; } // coords of tiles adjacent to peasant's land

    public int id  { get; set; } = -1;
    public int size { get; set; } = 0; // all members of household
    public int laborPower { get; set; } = 0; // members of household able to work in fields?
    public double foodSupply { get; set; } = 5.0; // default starting supply

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
                foodSupply += (t.food * 5); 
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

    public void _OccupyEmptyLand()
    {
        HashSet<WorldTile> newTiles = new HashSet<WorldTile>();
        foreach (WorldTile t in adjacencies)
        {
            if (t.holding.owner == null && t.AgType == WorldTile.AGRICULTURE_TYPE.Arable)
            {
                Holding h = t.holding;
                holdings.Add(h);
                h.owner = this;

                newTiles.Add(t);
            }
        }
        foreach (WorldTile w in newTiles)
        {
            _CalculateAdjacencies((int) w.coords.x, (int) w.coords.y);
        }
    }

    public void _InitData(int id, WorldTile[,] world, int width, int height)
    {
        this.world = world;
        this.width = width;
        this.height = height;

        this.id = id;
        this.size = (int) (GD.Randi() % 10) + 1;
    }

    // Given a single tile coordinates, adds adjacencies to set 
    public void _CalculateAdjacencies(int x, int y)
    {
        if (x - 1 >= 0) // keep within world boundaries
        {
            if (world[x - 1, y].holding.owner != this)
                adjacencies.Add(world[x - 1, y]);
        }
        if (x + 1 < width)
        {
            if (world[x + 1, y].holding.owner != this)
                adjacencies.Add(world[x + 1, y]);
        }
        if (y - 1 >= 0)
        {
            if (world[x, y - 1].holding.owner != this)
                adjacencies.Add(world[x, y - 1]);
        }
        if (y + 1 < height)
        {
            if (world[x, y + 1].holding.owner != this)
                adjacencies.Add(world[x, y + 1]);
        }       
    }

    public void _PrintDebug()
    {
        GD.Print("\n# PEASANT SUMMARY #");
        GD.Print("Adjacencies:");
        foreach (WorldTile w in adjacencies)
        {
            GD.Print(w.coords.x + ", " + w.coords.y);
        }
        GD.Print("\nPeasant ID: " + id);
        GD.Print("Food Supply: " + foodSupply);
        GD.Print("Size: " + size);
    }

}
