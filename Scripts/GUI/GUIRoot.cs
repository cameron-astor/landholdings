using Godot;
using System;

public class GUIRoot : Control
{

    // GUI nodes
    Label date; 
    Label FPS;
    Data worldData;

    public override void _Ready()
    {
        worldData = GetNode<Data>("../../World/Data");
        date = GetNode<Label>("DateUI");
        FPS = GetNode<Label>("FPS");
    }

    public override void _Process(float delta)
    {
        _UpdateFPS();
    }

    private void _UpdateDate()
    {
        // todo
    }

    private void _UpdateFPS()
    {
        FPS.Text = ("FPS: " + Engine.GetFramesPerSecond());
    }
}
