using Godot;
using System;
using System.Collections.Generic;

public class PeasantFamily
{

    public PeasantFamily()
    {
        holdings = new HashSet<Holding>();
    }

    public HashSet<Holding> holdings { get; private set; } // the holdings of the peasant
    public int id  { get; set; } = -1;
    public int size { get; set; } = 0;
    private double foodSupply = 0.0;

    public void _Sow()
    {

    }

    public void _Harvest()
    {
        
    }

    public void _Metabolize()
    {
        
    }

}
