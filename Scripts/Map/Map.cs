using Godot;
using System;
using System.Collections.Generic;

/* TODO
# Abstract the messy swapping colormaps code behind another class or something

# Take user input code out of here and put in another node

# Factor colors and mapmodes out into different file
# to define them

# Use arrays to replace dictionaries for faster color lookups (the 
  array index can be the id)
*/
public class Map : Node2D
{

    // References to world data
    private Data worldData;
    private WorldTile[,] world;
    // private Godot.Collections.Dictionary<int, int> peasantHoldings;

    // Terrain colors
    private Color cArable;
    private Color cPasture;
    private Color cWaste;

    private Color BLANK;

    // Colormaps
    // Maps colors to a particular data id
    private Godot.Collections.Dictionary<int, Color> holdingColors;
    private Godot.Collections.Dictionary<int, Color> peasantColors;
    private Godot.Collections.Dictionary<int, Color> aristocratColors;

    // Map modes
    private enum MAP_MODES {
        Terrain = 0, Landholdings = 1, Peasants = 2, Aristocrats = 3, Food = 4, FoodSupply = 5
    }
    private int currentMapMode = 0;
    private MAP_MODES[] mapModes = {MAP_MODES.Terrain, MAP_MODES.Landholdings, 
				                    MAP_MODES.Peasants, MAP_MODES.Aristocrats, MAP_MODES.Food, MAP_MODES.FoodSupply};

    // Map data
    private Sprite[,] map;
    private List<Color>[,] allColors; // will hold the color for every map mode at every tile.
    private List<Color>[,] allColorsCopy; // A copy of allColors to allow for saving calculated 
                                          // map colors between ticks (for switching mapmodes, etc.)
    private List<Color>[,] currentColorMap; // Reference to whichever color map is the one being updated
    private List<Color>[,] displayColorMap; // Reference to whichever color map is the current one for this tick

    private int width;
    private int height;
    
    // Base map tile
    private Texture baseTile;

    // USER INPUT
    public override void _Input(InputEvent @event)
    {
        // Tile click
        if (@event is InputEventMouseButton e && e.Pressed)
        {
            if ((ButtonList) e.ButtonIndex is ButtonList.Left)
            {
                _PrintTileSummary();
            }
        }

        // Switch mapmode
        if (Input.IsActionPressed("mapmode_forward")
            && Input.IsActionJustPressed("mapmode_forward")) 
        {
            if (currentMapMode == mapModes.Length - 1)
            {
                currentMapMode = 0;
            } else {
                currentMapMode++;
            }
            GD.Print("Switch mapmode to " + mapModes[currentMapMode].ToString());
            _UpdateColorsAll(displayColorMap);
        }
    }

    public override void _Ready()
    {
        _InitializeGraphics();
        _InitializeData();
        _GenerateMap();
    }

    public override void _PhysicsProcess(float delta)
    {

    }

    private void _InitializeGraphics()
    {
        baseTile = (Texture) GD.Load("res://Graphics/Tile.png");

        BLANK = new Color(255, 255, 255, 0);

        cArable = new Color("5F6B41");
        cPasture = new Color("DBC164");
        cWaste = new Color("723E1B");
    }

    private void _InitializeData()
    {
        // Get references to data fields
        worldData = GetNode<Data>("../Data");
        world = worldData.world;
        width = worldData.width;
        height = worldData.height;

        // Init map
        map = new Sprite[width, height];
        allColors = new List<Color>[width, height];
        allColorsCopy = new List<Color>[width, height];

        for (int x = 0; x < width; x++) // init lists in allColors and copy
        {
            for (int y = 0; y < height; y++)
            {
                List<Color> list = new List<Color>();
                List<Color> list2 = new List<Color>();
                for (int i = 0; i < mapModes.Length; i++)
                {
                    list.Add(BLANK);
                    list2.Add(BLANK);
                }
                allColors[x, y] = list;
                allColorsCopy[x, y] = list2;
            }
        }


        // Init colormaps
        peasantColors = new Godot.Collections.Dictionary<int, Color>();
        holdingColors = new Godot.Collections.Dictionary<int, Color>();
        aristocratColors = new Godot.Collections.Dictionary<int, Color>();

    }

    private void _GenerateMap()
    {
        // Come up with a better way to do the map than all these sprites..
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++)
            {
                // Create tile sprite
                Sprite sprite = new Sprite();
                sprite.Texture = baseTile;
                sprite.Position = new Vector2((x * 16), (y * 16));

                _InitColorDicts(x, y);

                map[x, y] = sprite;
                this.AddChild(sprite);
            }
        }

        currentColorMap = allColors;
        displayColorMap = allColorsCopy;
        _BuildWholeMap();
        _UpdateColorsAll(currentColorMap);
    }

    private void _InitColorDicts(int x, int y)
    {
        // Add holding data to color map
        Color color = new Color((float) GD.RandRange(0, 1), 
                                (float) GD.RandRange(0, 1), 
                                (float) GD.RandRange(0, 1));
        int holdingID;
        holdingID = world[x, y].holding.holdingID;
        holdingColors[holdingID] = color;

        // Add peasant data to color map
        color = new Color((float) GD.RandRange(0, 1), 
                            (float) GD.RandRange(0, 1), 
                            (float) GD.RandRange(0, 1));
 
        if (world[x, y].holding.owner != null && world[x, y].holding.owner is PeasantFamily)
        {
            PeasantFamily p = (PeasantFamily) world[x, y].holding.owner;
            int peasantID = p.id;
            peasantColors[peasantID] = color;
        }

        // Add aristocrat data to colormap
        if (world[x, y].holding.owner != null && world[x, y].holding.owner is Aristocrat)
        {
            Aristocrat a = (Aristocrat) world[x, y].holding.owner;
            int id = a.id;
            aristocratColors[id] = color;
        }
    }

    private void _PrintTileSummary()
    {
        Vector2 coordinates = new Vector2(
            Mathf.Round(GetGlobalMousePosition().x / 16),
            Mathf.Round(GetGlobalMousePosition().y / 16)
        );
        if (coordinates.x < width && coordinates.y < height
            && coordinates.x > -1 && coordinates.y > -1)
        {
            WorldTile tile = world[(int) coordinates.x, (int) coordinates.y];
            tile._PrintDebug();
            if (tile.holding.owner != null) 
            {
                if (tile.holding.owner is PeasantFamily)
                {
                    PeasantFamily p = (PeasantFamily) tile.holding.owner;
                    p._PrintDebug();
                } else if (tile.holding.owner is Aristocrat)
                {
                    Aristocrat a = (Aristocrat) tile.holding.owner;
                    a._PrintDebug();
                }
            }
            else {
                GD.Print("No strata present");
            }
        }
    }

    // Calculates the color values for the given map region based on current 
    // data. Adds those values to the color array.
    public void _BuildMapRegion(int xloc, int r, Vector2 regionDimensions)
    {
        if (xloc == 0) {
            // if it's a new tick
            List<Color>[,] temp = displayColorMap; // swap colormaps 
            displayColorMap = currentColorMap;
            currentColorMap = temp; 
        }

        WorldTile current;
        for (int x = xloc; x < (xloc + regionDimensions.x + r); x++) 
        {
            for (int y = 0; y < regionDimensions.y; y++)
            {
                current = world[x, y];
                _CalculateAllColors(current, x, y);
            }
        }
    }

    // Calculates color values for the whole map based on current data.
    // Adds those values to the color array.
    public void _BuildWholeMap()
    {
        WorldTile current;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                current = world[x, y];
                _CalculateAllColors(current, x, y);
            }
        }
    }

    // Updates the whole map's colors based on the current color map
    // and the current map mode
    private void _UpdateColorsAll(List<Color>[,] colormap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].Modulate = colormap[x, y][currentMapMode];
            }
        }        
    }

    // Public accessor for _UpdateColorsAll
    public void _UpdateColorsAll()
    {
        _UpdateColorsAll(currentColorMap);
    }

    // For a single world tile, calculates the colors for all mapmodes
    private void _CalculateAllColors(WorldTile current, int x, int y)
    {
        double alpha;

        if (current.AgType == WorldTile.AGRICULTURE_TYPE.Arable) // assign terrain colors
            currentColorMap[x, y][(int)MAP_MODES.Terrain] = cArable;
        if (current.AgType == WorldTile.AGRICULTURE_TYPE.Pasture)
            currentColorMap[x, y][(int)MAP_MODES.Terrain] = cPasture;
        if (current.AgType == WorldTile.AGRICULTURE_TYPE.Waste)
            currentColorMap[x, y][(int)MAP_MODES.Terrain] = cWaste;

        // assign holding colors
        int hid = 0;
        if (current.holding != null && current.holding.type != Holding.HOLDING_TYPE.None)
        {
            hid = current.holding.holdingID;   
            currentColorMap[x, y][(int)MAP_MODES.Landholdings] = holdingColors[hid];
        }

        if (current.holding != null) 
        {
            if (current.holding.owner != null && current.holding.owner is PeasantFamily)
            {

                // assign peasant colors
                PeasantFamily p = (PeasantFamily) current.holding.owner;
                int pid = p.id;
                currentColorMap[x, y][(int) MAP_MODES.Peasants] = peasantColors[pid];

                // assign food supply colors
                double foodSupply = p.foodSupply;
                alpha = foodSupply / (foodSupply + 50);
                currentColorMap[x, y][(int)MAP_MODES.FoodSupply] = new Color(1, 0, 0, (float) alpha);
            } else {
                currentColorMap[x, y][(int)MAP_MODES.Peasants] = BLANK;
            }
            if (current.holding.owner != null && current.holding.owner is Aristocrat)
            {
                Aristocrat a = (Aristocrat) current.holding.owner;
                int aid = a.id;
                currentColorMap[x, y][(int) MAP_MODES.Aristocrats] = aristocratColors[aid];
            }
        }                

        // assign food colors
        alpha = current.food / (current.food + 5);
        currentColorMap[x, y][(int)MAP_MODES.Food] = new Color(0, 1, 0, (float) alpha);

    }
}
