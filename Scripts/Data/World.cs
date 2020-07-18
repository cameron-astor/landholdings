using Godot;
using System;

/* The World manages the simulation, such as splitting the map up for updates
   and holding various logistical data. 
*/
public class World : Node2D
{
    
    private Data data; // Reference to game data
    private int width; // width of whole map
    private int height; // height of whole map

    private Map map; // Reference to map

    // Simulation parameters
    [Export]
    private int tickRate = 50;
    private int timer = 0;
    [Export]
    private int batchRegions = 5; // number of map subdivisions for updates
    private int currentRegion = 0; // Tracks the current region being processed
    private Vector2 regionDimensions; // the dimensions of each region (calculated at start of sim)
    private int remainder = 0; // In the case of uneven division of width into batch regions, a remainder to append
    private int xlocation = 0; // the current upper left corner of the region

    // In-game date
    private Date date;

    public override void _Ready()
    {
        // Get references
        data = GetNode<Data>("Data");
        map = GetNode<Map>("Map");

        // Assign permanent values from data
        width = data.width;
        height = data.height;

        date = new Date();
        _CalculateRegionDimensions();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (currentRegion < batchRegions) 
        {
            int remainderToAdd = 0;
            int xloc = ((int)regionDimensions.x * currentRegion);
            if ((currentRegion == batchRegions - 1) && remainder > 0)
                remainderToAdd = remainder;

            data._UpdateRegion(xloc, remainderToAdd, regionDimensions);
            map._BuildMapRegion(xloc, remainderToAdd, regionDimensions);

            currentRegion++;
        }

        if (timer == tickRate) // every tick of in-game time
        {
            date._UpdateDate();
            // render calculated map colors ***
            //map._UpdateColors();
            map._UpdateColorsAll();

            // start updates for the next tick
            currentRegion = 0;

            // reset tick timer
            timer = 0;
        }
        timer++;      
    }

    private void _CalculateRegionDimensions()
    {
        regionDimensions = new Vector2(width / batchRegions, height);
        if (width % batchRegions != 0)
        {
            remainder = width % batchRegions; // in the case of uneven division
        } 
    }

    public Date _GetSimDateTime()
    {
        return date;
    }

}
