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

    // Property
    public Holding holding { get; set; }

    private void _UpdateEcology()
    {

    }

}
