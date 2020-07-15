using Godot;
using System;

public class Date : Node
{

    private int month;
    private int year;

    public void _UpdateDate()
    {
        if (month == 12)
        {
            month = 1;
            year++;
        } else {
            month++;
        }
    }

}
