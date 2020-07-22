using Godot;
using System;
using System.Collections.Generic;

/* 
    Contains static functions which make up the simulation.
    Each function acts on a single world tile, accepting references to whatever external 
    data is required.
 */
public static class Sim
{

    /* Accepts a WorldTile and checks whether the peasant family in that tile has been updated yet.
       If it hasn't, updates all tiles belonging to that peasant family, else only updates ecology */
    public static void _SimPeasants(WorldTile current, HashSet<PeasantFamily> updatedPeasants, Date date)
    {
        // update based on peasants
        PeasantFamily p = current.holding.owner;
        if (p != null) // if the tile is owned
        {
            if (!updatedPeasants.Contains(p)) // and the owner has not yet been updated
            {
                foreach (Holding h in p.holdings)
                {
                    foreach(WorldTile land in h.constituentTiles)
                    {
                        Sim._UpdateEcology(land, date); // perform tile updates for all tiles owned by this peasant
                    }
                }

                // perform peasant-wide operations here
                p._Harvest();
                p._Metabolize();

                updatedPeasants.Add(p); // mark as updated
            }
            if (p.dead == true) // remove dead peasants
                current.holding.owner = null;
        } else { // if the tile is not owned
            Sim._UpdateEcology(current, date);
        }
    }

    // Updates food in a tile based on month
    public static void _UpdateEcology(WorldTile t, Date date)
    {
	    int rand = 0;
	    if (t.AgType == WorldTile.AGRICULTURE_TYPE.Arable)
        {
            if (date.month >= 11 || date.month <= 2) // winter
                t.food = 0;
            if (date.month == 3) // march
                rand = (int) GD.Randi() % 3;
                if (rand == 1)
                    t.food = 1;
            if (date.month == 4) // april
            {
                rand = (int) GD.Randi() % 3;
                switch (rand)
                {
                    case 0:
                        t.food = 1;
                        break;
                    case 1:
                        t.food = 2;
                        break;
                    case 2:
                        t.food = 2;
                        break;
                }
            }
            if (date.month == 5) // may
            {
                rand = (int) GD.Randi() % 4;
                switch (rand)
                {
                    case 0:
                        t.food = 2;
                        break;
                    case 1:
                        t.food = 2;
                        break;
                    case 2:
                        t.food = 3;
                        break;
                    case 3:
                        t.food = 5;
                        break;
                }
            }  
            if (date.month == 6) // june
            {
                rand = (int) GD.Randi() % 3;
                switch (rand)
                {
                    case 0:
                        t.food = 3;
                        break;
                    case 1:
                        t.food = 4;
                        break;
                    case 2:
                        t.food = 5;
                        break;
                }
            } 
            if (date.month == 7) // july
            {
                rand = (int) GD.Randi() % 3;
                switch (rand)
                {
                    case 0:
                        t.food = 4;
                        break;
                    case 1:
                        t.food = 4;
                        break;
                    case 2:
                        t.food = 5;
                        break;
                }
            } 
            if (date.month == 8) // August
            {
                rand = (int) GD.Randi() % 4;
                switch (rand)
                {
                    case 0:
                        t.food = 2;
                        break;
                    case 1:
                        t.food = 2;
                        break;
                    case 2:
                        t.food = 3;
                        break;
                    case 3:
                        t.food = 4;
                        break;
                }
            }  
            if (date.month == 9) // sept
            {
                rand = (int) GD.Randi() % 4;
                switch (rand)
                {
                    case 0:
                        t.food = 1;
                        break;
                    case 1:
                        t.food = 1;
                        break;
                    case 2:
                        t.food = 2;
                        break;
                    case 3:
                        t.food = 2;
                        break;
                }
            }  
            if (date.month == 10) // oct
            {
                rand = (int) GD.Randi() % 4;
                switch (rand)
                {
                    case 0:
                        t.food = 0;
                        break;
                    case 1:
                        t.food = 1;
                        break;
                    case 2:
                        t.food = 1;
                        break;
                    case 3:
                        t.food = 1;
                        break;
                }
            }  
        } else {
            t.food = 0;
        }
    }

}
