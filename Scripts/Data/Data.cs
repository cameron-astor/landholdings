using Godot;
using System;
using System.Collections.Generic;

/* TODO 
# Consider how to logically split this up
  Separate simulation logistics from the game data?

# Getting data from the world onto the map efficiently is a problem...
*/

/* 
    Contains the data that makes up the simulation and exposes
    functions to update it (i.e. the functions that make up
    the activity of the world)
*/
public class Data : Node
{

    // World dimensions
    [Export] 
    public int width { get; set; } = 100;
    [Export]
    public int height { get; set; } = 100;

    // Perlin noise parameters
    [Export]
    private int octaves = 4;
    [Export]
    private int period = 20;
    [Export]
    private float lacunarity = 1.5f;
    [Export]
    private float persistence = 0.75f;

    // Simulation parameters (should be factored into another class at some point idk)
    [Export]
    private int tickRate = 50;
    private int timer = 0;
    [Export]
    private int batchRegions = 5; // number of map subdivisions for updates
    private int currentRegion = 0; // Tracks the current region being processed
    private Vector2 regionDimensions; // the dimensions of each region (calculated at start of sim)
    private int remainder = 0; // In the case of uneven division of width into batch regions, a remainder to append
    private int xlocation = 0; // the current upper left corner of the region


    // World attributes
    private Date date;

    // Data containers
    public WorldTile[,] world { get; set; }
    private List<PeasantFamily> peasants;
    public Godot.Collections.Dictionary<int, int> peasantHoldings { get; set; }

    // Signals
    [Signal]
    public delegate void MapUpdateSignal(int x, int y);

    public override void _Ready()
    {
        // init containers, etc
        date = new Date();
        world = new WorldTile[width, height];
        peasantHoldings = new Godot.Collections.Dictionary<int, int>();

        _CalculateRegionDimensions();
        _GenerateWorld();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (currentRegion < batchRegions) 
        {
            bool addRemainder = false;
            int xloc = ((int)regionDimensions.x * currentRegion);
            if ((currentRegion == batchRegions - 1) && remainder > 0)
                addRemainder = true;
            GD.Print("Update region at " + xloc + " with remainder " + addRemainder);
            _UpdateRegion(xloc, addRemainder);
            currentRegion++;
        }

        if (timer == tickRate)
        {
            date._UpdateDate();
            currentRegion = 0;
            // _UpdateSim();
            timer = 0;
        }
        timer++;
    }

    private void _UpdateSim()
    {
        date._UpdateDate();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Naive signals (EXPERIMENTAL)
                //EmitSignal("MapUpdateSignal", x, y);

            }
        }
    }

    // Takes in the upper left corner x value of the region to be updated
    private void _UpdateRegion(int xloc, bool addRemainder)
    {
        int r = 0;
        if (addRemainder)
            r = remainder;

        for (int x = xloc; x < (xloc + regionDimensions.x + r); x++) 
        {
            for (int y = 0; y < regionDimensions.y; y++)
            {
                // GD.Print(x);
            }
        }

    }

    private void _CalculateRegionDimensions()
    {
        regionDimensions = new Vector2(width / batchRegions, height);
        if (width % batchRegions != 0)
        {
            remainder = width % batchRegions; // in the case of uneven division
        } 
    }

    public void _GenerateWorld()
    {
        _GenerateTerrain();
        _GenerateHoldings();
        _GeneratePeasants();
    }

    // Uses Open Simplex Noise to generate terrain
    public void _GenerateTerrain()
    {
        GD.Randomize(); // reseed rng
        OpenSimplexNoise noise = new OpenSimplexNoise();
        noise.Seed = (int) GD.Randi();
        noise.Octaves = octaves;
        noise.Period = period;
        noise.Lacunarity = lacunarity;
        noise.Persistence = persistence;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WorldTile tile = new WorldTile();
                WorldTile.AGRICULTURE_TYPE type = _GetNoiseTile(noise.GetNoise2d((float) x, (float) y));

                tile.AgType = type;

                world[x, y] = tile;
            }
        }
    }

    private WorldTile.AGRICULTURE_TYPE _GetNoiseTile(float noise)
    {
        if (noise < 0.0)
            return WorldTile.AGRICULTURE_TYPE.Arable;
        if (noise < 0.3)
            return WorldTile.AGRICULTURE_TYPE.Pasture;
        return WorldTile.AGRICULTURE_TYPE.Waste;
    }

    public void _GenerateHoldings()
    {
        int holdingID = 0;
        WorldTile t;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                t = world[x, y];
                if (t.AgType == WorldTile.AGRICULTURE_TYPE.Arable) 
                {
                    t.holdingID = holdingID;
                    holdingID++;
                }
            }
        }
    }

    public void _GeneratePeasants()
    {
        WorldTile current;
        PeasantFamily peasant;
        int id = 0;
        // Possible optimization: on world generation
        // store individual lists of different types
        // of tiles?
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++)
            {
                current = world[x, y];
                if (current.holdingID != -1)
                {
                    peasant = new PeasantFamily();
                    peasant.size = ((int) GD.Randi() % 10) + 1;
                    peasant.id = id;
                    peasantHoldings[current.holdingID] = peasant.id;
                    id++;
                }
            }
        }
    }

    public Date _GetSimDateTime()
    {
        return date;
    }
}
