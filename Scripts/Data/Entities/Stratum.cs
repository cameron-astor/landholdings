using Godot;
using System;

public class Stratum
{

    // Reference to the world
    public WorldTile[,] world { protected get; set; }
    public int width { protected get; set; }
    public int height { protected get; set; }

    public int id { get; set; }= -1;

    public int size { get; set; } = 0;
    public double foodSupply { get; set; } = 0.0;

    public virtual void _PrintDebug()
    {
        GD.Print("default stratum");
    }

}
