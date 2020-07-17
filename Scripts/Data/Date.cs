using Godot;
using System;

public class Date : Node
{
    public enum MONTHS {
        January = 1, February = 2, March = 3,
        April = 4, May = 5, June = 6,
        July = 7, August = 8, September = 9,
        October = 10, November = 11, December = 12
    }

    public int month { get; set; } = 1;
    public int year { get; set; } = 0;

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
