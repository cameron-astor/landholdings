using Godot;
using System;
using System.Collections.Generic;

/* TODO 
    - Possible optimization: on world generation
      store individual lists of different types
      of tiles?

    - Refactor into multiple files (too much sim update logistics in here still)
      In particular, we're going to need a quick way to iterate and swap out simulation functions, etc.
      So those should definitely be abstracted from the perspective of this file (so we can plug in a single
      function call to swap out a sim, rather than rewrite it all here (and thus not get to save them)).
*/

/* 
    Contains the data that makes up the state of the world.
    Generates world at start.
    Exposes functions to update the whole state (see Sim.cs for individual
    simulation functions).
*/
public class Data : Node
{

    // World dimensions
    [Export] 
    public int width { get; private set; } = 100;
    [Export]
    public int height { get; private set; } = 100;

    // Perlin noise parameters
    [Export]
    private int octaves = 4;
    [Export]
    private int period = 20;
    [Export]
    private float lacunarity = 1.5f;
    [Export]
    private float persistence = 0.75f;

    // Other params
    [Export]
    private int aristocratDensity = 10;

    // Data containers
    public WorldTile[,] world { get; private set; }

    public override void _Ready()
    {
        // init containers, etc
        world = new WorldTile[width, height];
        updatedPeasants = new HashSet<PeasantFamily>();
        updatedAristocrats = new HashSet<Aristocrat>();

        _GenerateWorld();
    }

    // Sets which keep track of what higher level entities have been updated in the 
    // current update so far. (move into another file pls)
    private HashSet<PeasantFamily> updatedPeasants;
    private HashSet<Aristocrat> updatedAristocrats;

    // Intended to be called when each full update is complete.
    // Resets update logistics (like sets of entities)
    public void _FinishUpdate()
    {
        updatedPeasants.Clear();
    }

    // Takes in the upper left corner x value of the region to be updated,
    // A remainder (in the case of uneven map division),
    // and the dimensions of the region to be updated
    public void _UpdateRegion(int xloc, int r, Vector2 regionDimensions, Date date)
    {
        WorldTile current;
        for (int x = xloc; x < (xloc + regionDimensions.x + r); x++) 
        {
            for (int y = 0; y < regionDimensions.y; y++)
            {
                current = world[x, y];
                if (current.holding.owner is PeasantFamily)
                {
                    Sim._SimPeasants(current, updatedPeasants, date);
                }
                else if (current.holding.owner is Aristocrat)
                {
                    Sim._UpdateEcology(current, date);
                } 
                else // no owner 
                {
                    Sim._UpdateEcology(current, date);
                }  
                
            }
        }

    }

    // Runs all generation functions
    private void _GenerateWorld()
    {
        _GenerateTerrain();
        _GenerateHoldings();
        _GenerateAristocrats();
        _GeneratePeasants();
    }

    // Uses Open Simplex Noise to generate terrain
    private void _GenerateTerrain()
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
                WorldTile tile = new WorldTile(x, y);
                WorldTile.AGRICULTURE_TYPE type = _GetNoiseTile(noise.GetNoise2d((float) x, (float) y));

                tile.AgType = type;

                world[x, y] = tile;
            }
        }
    }

    // Returns a terrain type based on a perlin noise value
    private WorldTile.AGRICULTURE_TYPE _GetNoiseTile(float noise)
    {
        if (noise < 0.0)
            return WorldTile.AGRICULTURE_TYPE.Arable;
        if (noise < 0.3)
            return WorldTile.AGRICULTURE_TYPE.Pasture;
        return WorldTile.AGRICULTURE_TYPE.Waste;
    }

    // Generates land holdings, one per tile
    private void _GenerateHoldings()
    {
        int id = 0;
        WorldTile t;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                t = world[x, y];
                t.holding = new Holding(); // register holding with tile
                t.holding.constituentTiles.Add(t); // register tile with holding

                if (t.AgType == WorldTile.AGRICULTURE_TYPE.Arable) 
                {
                    t.holding.type = Holding.HOLDING_TYPE.Freehold;
                    t.holding.holdingID = id;
                } else {
                    t.holding.type = Holding.HOLDING_TYPE.None;
                    t.holding.holdingID = id;
                }

                id++;
            }
        }
    }

    int peasantID = 0;

    // Generates peasant families. Puts one peasant family in each single-tile holding
    private void _GeneratePeasants()
    {
        WorldTile current;
        PeasantFamily peasant;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++)
            {
                current = world[x, y];
                if (current.holding.type == Holding.HOLDING_TYPE.Freehold)
                {
                    // init peasant
                    peasant = new PeasantFamily();
                    peasant._InitData(peasantID, world, width, height);
                    peasant._CalculateAdjacencies(x, y);

                    peasant.holdings.Add(current.holding); // register holding with peasant
                    current.holding.owner = peasant; // register peasant with holding
                    peasantID++;
                }
            }
        }
    }

    private void _GenerateAristocrats()
    {
        WorldTile current;
        Aristocrat a;
        int id = 0;

        GD.Randomize();
        int rand = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                current = world[x, y];
                if (current.AgType == WorldTile.AGRICULTURE_TYPE.Arable && current.holding.owner == null)
                {
                    rand = (int) (GD.Randi() % aristocratDensity);
                    if (rand == (aristocratDensity / 2))
                    {
                        a = new Aristocrat();
                        a._InitData(id, world, width, height);
                        id++;

                        current.holding.owner = a;
                        current.holding.occupier = a;
                        current.holding.type = Holding.HOLDING_TYPE.Manor;
                        _CreateAristocratHolding(current, a);
                    }
                }
            }
        }
    }

    private void _CreateAristocratHolding(WorldTile t, Aristocrat a)
    {
        int x = (int) t.coords.x;
        int y = (int) t.coords.y;
        GD.Randomize();
        int rand = 0;
        int factor = 10;

        HashSet<Vector2> holdingsToAdd = new HashSet<Vector2>();
        
        for (int i = 0; i < 8; i++) // create a randomly shaped large manor holding
        {
            rand = (int) (GD.Randi() % factor);
            if ((x - 1) >= 0 && (y - 1) >= 0) // top left
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x - 1, y - 1));
                }
            }
            rand = (int) (GD.Randi() % factor);
            if ((y - 1) >= 0) // top
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x, y - 1));
                }               
            }
            rand = (int) (GD.Randi() % factor);
            if ((x + 1) < width && (y - 1) >= 0) // top right
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x + 1, y - 1));
                }                
            }
            rand = (int) (GD.Randi() % factor);
            if ((x + 1) < width) // right
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x + 1, y));
                } 
            }
            rand = (int) (GD.Randi() % factor);
            if ((x + 1) < width && (y + 1) < height) // bottom right
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x + 1, y + 1));
                } 
            }
            rand = (int) (GD.Randi() % factor);
            if ((y + 1) < height) // bottom
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x, y + 1));
                } 
            }
            rand = (int) (GD.Randi() % factor);
            if ((x - 1) >= 0 && (y + 1) < height) // bottom left
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x - 1, y + 1));
                } 
            }
            rand = (int) (GD.Randi() % factor);
            if ((x - 1) >= 0) // left
            {
                if (rand == 2)
                {
                    holdingsToAdd.Add(new Vector2(x - 1, y));
                } 
            }
        }

        foreach (Vector2 v in holdingsToAdd)
        {
            GD.Randomize();
            rand = (int) (GD.Randi() % 3); // generate either more demense land, villein tenures or copyhold tenures

            Holding h = world[(int) v.x,(int) v.y].holding;
            if (rand == 0) // demense
            {
                h.type = Holding.HOLDING_TYPE.Manor;
                h.owner = a;
                h.occupier = a;
            } else {
                if (rand == 1) // villeinage
                {
                    h.type = Holding.HOLDING_TYPE.Villeinage;
                } else { // copyhold
                    h.type = Holding.HOLDING_TYPE.Copyhold;
                }
                
                h.owner = a;

                // peasant occupier
                PeasantFamily peasant = new PeasantFamily();
                peasant._InitData(peasantID, world, width, height);
                peasant._CalculateAdjacencies(x, y);

                h.occupier = peasant;
                peasant.holdings.Add(h);
                a.tenants.Add(peasant);

                peasantID++;
            } 
        }

    }
}
