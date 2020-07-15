using Godot;
using System;

public class WorldTile : Node
{

    public enum AGRICULTURE_TYPE {
        None, Arable, Pasture, Waste
    }

    // Terrain and ecology
    public AGRICULTURE_TYPE AgType { get; set; } = AGRICULTURE_TYPE.None;
    public double food  { get; set; } = 0.0;

    // Population
    private int pop = 0;

    // Property
    public enum PROPERTY_TYPE {
        None, Freehold
    }

    private PROPERTY_TYPE propertyType = PROPERTY_TYPE.Freehold;
    public int holdingID { get; set; } = -1;

    private void _UpdateEcology()
    {

    }

}
