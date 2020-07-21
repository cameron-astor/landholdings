using Godot;
using System;

/*
    Represents a landholding.
    Is owned by a PeasantFamily.
*/
public class Holding
{

    public enum HOLDING_TYPE {
        None, Freehold
    }

    public int holdingID { get; set; } = -1;
    public PeasantFamily owner { get; set; }
    public HOLDING_TYPE type { get; set; } = HOLDING_TYPE.Freehold;

}
