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
        
        world = new WorldTile[width, height];
        peasantHoldings = new Godot.Collections.Dictionary<int, int>();

        _GenerateWorld();
    }

    public override void _PhysicsProcess(float delta)
    {

    }

    private void _UpdateSim()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Naive signals (EXPERIMENTAL)
                //EmitSignal("MapUpdateSignal", x, y);
            }
        }
    }

    // Takes in the upper left corner x value of the region to be updated,
    // A remainder (in the case of uneven map division),
    // and the dimensions of the region to be updated
    public void _UpdateRegion(int xloc, int r, Vector2 regionDimensions)
    {

        for (int x = xloc; x < (xloc + regionDimensions.x + r); x++) 
        {
            for (int y = 0; y < regionDimensions.y; y++)
            {
                // Run sim here
            }
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

}
