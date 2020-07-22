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
    public WorldTile[,] world { get; private set; }

    public override void _Ready()
    {
        // init containers, etc
        world = new WorldTile[width, height];
        updatedPeasants = new HashSet<PeasantFamily>();

        _GenerateWorld();
    }

    // Sets which keep track of what higher level entities have been updated in the 
    // current update so far. (move into another file pls)
    private HashSet<PeasantFamily> updatedPeasants;

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
                // update based on peasants
                PeasantFamily p = current.holding.owner;
                if (p != null) // if the tile is owned
                {
                    if (!updatedPeasants.Contains(p)) // and the owner has not yet been updated
                    {
                        foreach (Holding h in p.holdings)
                        {
                            foreach(WorldTile land in h.constituentTiles)
                            {
                                Sim._UpdateEcology(land, date); // perform tile updates for all tiles owned by this peasant
                            }
                        }

                        // perform peasant-wide operations here
                        p._Harvest();
                        p._Metabolize();

                        updatedPeasants.Add(p); // mark as updated
                    }
                    if (p.dead == true) // remove dead peasants
                        current.holding.owner = null;
                } else { // if the tile is not owned
                    Sim._UpdateEcology(current, date);
                }
            }
        }

    }

    // Runs all generation functions
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
    public void _GenerateHoldings()
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

    // Generates peasant families. Puts one peasant family in each single-tile holding
    public void _GeneratePeasants()
    {
        WorldTile current;
        PeasantFamily peasant;
        int id = 0;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++)
            {
                current = world[x, y];
                if (current.holding.type != Holding.HOLDING_TYPE.None)
                {
                    // init peasant
                    peasant = new PeasantFamily();
                    peasant.size = ((int) GD.Randi() % 10) + 1;
                    peasant.id = id;
                    peasant.holdings.Add(current.holding); // register holding with peasant
                    current.holding.owner = peasant; // register peasant with holding

                    id++;
                }
            }
        }
    }

}
