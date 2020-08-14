using Godot;
using System;

/* The World manages the simulation, such as splitting the map up for updates
   and holding various logistical data. 
*/

/* TODO
    - create a more responsive system for changing speed
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
    public Date date { get; private set; }

    // Flags
    bool speedUp = false;
    bool slowDown = false;

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
        _Update();

        if (timer == tickRate) // every tick of in-game time
        {
            _TickUpdate();
        }
        timer++;      
    }

    public override void _Input(InputEvent @event)
    {
        // speed up
        if (Input.IsActionPressed("speed_up")
            && Input.IsActionJustPressed("speed_up")) 
        {
            speedUp = true;
        }

        // slow down
        if (Input.IsActionPressed("speed_down")
            && Input.IsActionJustPressed("speed_down"))
        {
            slowDown = true;
        }
    }

    // Updates data and map between in-game ticks
    private void _Update()
    {
        if (currentRegion < batchRegions) 
        {
            int remainderToAdd = 0;
            int xloc = ((int)regionDimensions.x * currentRegion);
            if ((currentRegion == batchRegions - 1) && remainder > 0)
                remainderToAdd = remainder;

            data._UpdateRegion(xloc, remainderToAdd, regionDimensions, date);
            map._BuildMapRegion(xloc, remainderToAdd, regionDimensions);

            currentRegion++;
        }
    }

    // Performs updates required at the end of each in-game tick
    private void _TickUpdate()
    {
            date._UpdateDate();

            map._UpdateColorsAll(); // render calculated map colors
            data._FinishUpdate(); // prepare data for next update
            
            // process speed updates
            if (slowDown)
            {
                if (tickRate < 80)
                {
                    tickRate = tickRate + 10;
                    GD.Print("Slowing down" + tickRate);
                }
                slowDown = false;
            } else if (speedUp)
            {
                if (tickRate > 10)
                {
                    tickRate = tickRate - 10;
                    GD.Print("Speeding up" + tickRate);
                }
                speedUp = false;
            }

            currentRegion = 0; // start updates for the next tick
            timer = 0; // reset tick timer
    }

    // Calculates the size of an update region based on the size of the world
    // and the number of regions required (batchRegions)
    private void _CalculateRegionDimensions()
    {
        regionDimensions = new Vector2(width / batchRegions, height);
        if (width % batchRegions != 0)
        {
            remainder = width % batchRegions; // in the case of uneven division
        } 
    }

}
