using Godot;
using System;

public class GUIRoot : Control
{

    // GUI nodes
    Label date; 
    Label FPS;

    // Data
    Data worldData;
    World world;
    Date dateData;

    public override void _Ready()
    {
        worldData = GetNode<Data>("../../World/Data");
        world = GetNode<World>("../../World");
        dateData = world._GetSimDateTime();

        date = GetNode<Label>("DateUI");
        FPS = GetNode<Label>("FPS");
    }

    public override void _Process(float delta)
    {
        _UpdateFPS();
        _UpdateDate();
    }

    // TODO: split into two labels so the UI isnt so jarring to look at as it changes
    private void _UpdateDate()
    {
        int m = dateData.month;

        if (m == 1) // adjust to correct date (tick system makes display one month ahead)
            m = 12;
        else 
            m = m - 1;

        string month = ((Date.MONTHS) m).ToString();

        int year = dateData.year; // likewise for year
        if (m == 12)
            year = year - 1;

        date.Text = (month + ", " + year);
    }

    private void _UpdateFPS()
    {
        FPS.Text = ("FPS: " + Engine.GetFramesPerSecond());
    }
}
