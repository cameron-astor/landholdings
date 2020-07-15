using Godot;
using System;

/* TODO
# Zoom in moves towards cursor location
# Boundaries
# Smooth slowdown for mouse movement at borders
# Click and drag
*/

public class Camera : Camera2D
{

    private const double ZOOM_FACTOR = 0.06;

    private int zoomCounter = 0;
    private enum ZOOM {
        None, Out, In
    }

    public override void _PhysicsProcess(float delta)
    {
        _CalculateZoom();
        _GetMouseInput();
        _GetKeyboardInput();
        _CalculateBorderMouse();
    }

    public void _GetMouseInput()
    {

    }

    public void _GetKeyboardInput()
    {
        if (Input.IsActionPressed("map_right"))
            Translate(new Vector2(20, 0));
        if (Input.IsActionPressed("map_up"))
            Translate(new Vector2(0, -20));
        if (Input.IsActionPressed("map_left"))
            Translate(new Vector2(-20, 0));
        if (Input.IsActionPressed("map_down"))
            Translate(new Vector2(0, 20));
    }

    public void _CalculateBorderMouse()
    {

    }

    public void _CalculateZoom()
    {

    }
}
