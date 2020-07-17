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
        string month = ((Date.MONTHS) dateData.month).ToString();
        int year = dateData.year;
        date.Text = (month + ", " + year);
    }

    private void _UpdateFPS()
    {
        FPS.Text = ("FPS: " + Engine.GetFramesPerSecond());
    }
}
