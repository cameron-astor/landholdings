using Godot;
using System;

public class WorldTile : Node
{

    public WorldTile(int x, int y)
    {
        coords = new Vector2(x, y);
    }

    public enum AGRICULTURE_TYPE {
        None, Arable, Pasture, Waste
    }

    // coords
    public Vector2 coords { get; private set; }

    // Terrain and ecology
    public AGRICULTURE_TYPE AgType { get; set; } = AGRICULTURE_TYPE.None;
    public double food  { get; set; } = 0.0;

    // Property
    public Holding holding { get; set; }

    private void _UpdateEcology()
    {

    }

}
