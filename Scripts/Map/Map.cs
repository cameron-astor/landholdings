using Godot;
using System;

/* TODO
# Take user input code out of here and put in another node

# Factor colors and mapmodes out into different file
# to define them

#
*/
public class Map : Node2D
{

    // References to world data
    private Data worldData;
    private WorldTile[,] world;
    private Godot.Collections.Dictionary<int, int> peasantHoldings;

    // Terrain colors
    private Color cArable;
    private Color cPasture;
    private Color cWaste;

    // Colormaps
    // Maps colors to a particular data id
    private Godot.Collections.Dictionary<int, Color> holdingColors;
    private Godot.Collections.Dictionary<int, Color> peasantColors;

    // Map modes
    private enum MAP_MODES {
        Terrain, Population, Landholdings, Peasants, Food
    }
    private int currentMapMode = 0;
    private MAP_MODES[] mapModes = {MAP_MODES.Terrain, MAP_MODES.Population, MAP_MODES.Landholdings, 
				                MAP_MODES.Peasants, MAP_MODES.Food};

    // Map data
    private Sprite[,] map;
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
        }
    }

    public override void _Ready()
    {
        _InitializeGraphics();
        _InitializeData();
        _GenerateMap();
        // _GenerateMapImage();
    }

    public override void _Process(float delta)
    {
        _UpdateMap();
    }

    private void _InitializeGraphics()
    {
        baseTile = (Texture) GD.Load("res://Graphics/Tile.png");

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
        peasantHoldings = worldData.peasantHoldings;

        map = new Sprite[width, height];

        // Init colormaps
        peasantColors = new Godot.Collections.Dictionary<int, Color>();
        holdingColors = new Godot.Collections.Dictionary<int, Color>();

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

                // Add holding data to color map
                Color color = new Color((float) GD.RandRange(0, 1), 
                                        (float) GD.RandRange(0, 1), 
                                        (float) GD.RandRange(0, 1));
                int holdingID = world[x, y].holdingID;
                if (holdingID != -1)
                {
                    holdingColors[holdingID] = color;
                }

                // Add peasant data to color map
                color = new Color((float) GD.RandRange(0, 1), 
                                  (float) GD.RandRange(0, 1), 
                                  (float) GD.RandRange(0, 1));
			    holdingID = world[x, y].holdingID;
			    if (holdingID != -1)
                {
                    int peasantID = peasantHoldings[world[x, y].holdingID];
                    peasantColors[peasantID] = color;
                }

                map[x, y] = sprite;
                this.AddChild(sprite);
            }
        }
    }

    private void _PrintTileSummary()
    {
        Godot.Collections.Dictionary<string, string> tileSummary = new Godot.Collections.Dictionary<string, string>();
        Vector2 coordinates = new Vector2(
            Mathf.Round(GetGlobalMousePosition().x / 16),
            Mathf.Round(GetGlobalMousePosition().y / 16)
        );
        if (coordinates.x < width && coordinates.y < height)
        {
            WorldTile tile = world[(int) coordinates.x, (int) coordinates.y];
            tileSummary["1. Coordinates"] = coordinates.ToString();
            tileSummary["2. Agriculture"] = tile.AgType.ToString();
            tileSummary["3. Holding ID"] = tile.holdingID.ToString();
            if (tile.holdingID != -1) 
            {
                tileSummary["5. Peasant Family ID"] = peasantHoldings[tile.holdingID].ToString();
            }
            else {
                tileSummary["5. Peasant Family ID"] = "No peasants";
            }

            tileSummary["6. Food"] = tile.food.ToString();
            
            GD.Print(tileSummary);	
        }
    }

    // Updates the map tiles for the current mapmode
    private void _UpdateMap()
    {
        WorldTile current;
        double alpha;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Map mode logic
                current = world[x, y];
                if (mapModes[currentMapMode] == MAP_MODES.Terrain)
                {
                    if (current.AgType == WorldTile.AGRICULTURE_TYPE.Arable)
                        map[x, y].Modulate = cArable;
                    if (current.AgType == WorldTile.AGRICULTURE_TYPE.Pasture)
                        map[x, y].Modulate = cPasture;
                    if (current.AgType == WorldTile.AGRICULTURE_TYPE.Waste)
                        map[x, y].Modulate = cWaste;
                }

                if (mapModes[currentMapMode] == MAP_MODES.Population)
                {
                    // Not implemented
                    map[x, y].Modulate = new Color(0, 0, 0, 0.0f);
                }

                if (mapModes[currentMapMode] == MAP_MODES.Landholdings)
                {
                    if (current.holdingID != -1)
                        map[x, y].Modulate = holdingColors[current.holdingID];
                }

                if (mapModes[currentMapMode] == MAP_MODES.Peasants)
                {
                    if (current.holdingID != -1)
                        map[x, y].Modulate = peasantColors[peasantHoldings[current.holdingID]];
                }

                if (mapModes[currentMapMode] == MAP_MODES.Food)
                {
                    alpha = (float) current.food / current.food + 5;
                    map[x, y].Modulate = new Color(0, 1, 0, (float) alpha);
                }
            }
        }
    }

    // EXPERIMENTAL Image-based map
    // This one is a failure - slower than the Sprite based approach, probably
    // because of the number of calculations required to fill in every pixel (this is probably
    // handled much more efficiently by the Engine with Sprite)
    Image mapImage;
    ImageTexture mapImageTexture;
    Sprite mapSprite;
    private void _GenerateMapImage()
    {
        // Instantiate an empty image
        mapImage = new Image();
        mapImage.Create(width * 16, height * 16, false, Image.Format.Rgbaf);

        mapImageTexture = new ImageTexture();

        // test pixel setting
        mapImage.Lock(); // allow access to data
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _GenerateTileFromPixels(x, y);
            }
        }
        mapImage.Unlock();

        // set texture
        mapImageTexture.CreateFromImage(mapImage);
        
        // setup sprite
        mapSprite = new Sprite();
        mapSprite.Texture = mapImageTexture;

        // add to scene graph
        this.AddChild(mapSprite);
    }

    private void _GenerateTileFromPixels(int x, int y)
    {
        int pixelCoordX = x * 16;
        int pixelCoordY = y * 16;
        for (int pixelx = 0; pixelx < pixelCoordX + 16; pixelx++)
        {
            for (int pixely = 0; pixely < pixelCoordY + 16; pixely++)
            {
                mapImage.SetPixel(pixelx, pixely, new Color(0, 0, 0, 0.75f));
            }
        }
    }

}
