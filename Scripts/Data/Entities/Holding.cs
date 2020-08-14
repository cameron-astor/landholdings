using Godot;
using System;
using System.Collections.Generic;

/*
    Represents a landholding.
    Is owned by a Stratum (aristocrat or peasant)
*/
public class Holding
{

    public Holding()
    {
        type = HOLDING_TYPE.None;
        constituentTiles = new HashSet<WorldTile>();
    }

    public enum HOLDING_TYPE {
        None, Freehold, Manor, Villeinage, Copyhold
    }

    public int holdingID { get; set; } = -1;
    public Stratum owner { get; set; }
    public Stratum occupier { get; set; }
    public HOLDING_TYPE type { get; set; } = HOLDING_TYPE.Freehold;

    public HashSet<WorldTile> constituentTiles { get; private set; } // The set of tiles that the holding consists of

}
