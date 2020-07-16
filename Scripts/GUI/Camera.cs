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

    private const float ZOOM_FACTOR = 0.06f; // magnitude of zoom

    private int zoomCounter = 0;
    private enum ZOOM {
        None, Out, In
    }
    private ZOOM zoom = ZOOM.None;

    public override void _PhysicsProcess(float delta)
    {
        _CalculateZoom();
        // _GetMouseInput();
        _GetKeyboardInput();
        _CalculateBorderMouse();
    }

    // Intended to use the input map, currently broken 
    // (perhaps input map doesnt work with mouse wheel?)
    public void _GetMouseInput()
    {
        if (Input.IsActionPressed("zoom_in"))
        {
            if (this.Zoom.x > 0.5 && this.Zoom.y > 0.5)
            {
                zoom = ZOOM.In;
                zoomCounter = 5;
            }
        }
        if (Input.IsActionPressed("zoom_out"))
        {
            if (this.Zoom.x < 1.3 && this.Zoom.y < 1.3)
            {
                zoom = ZOOM.Out;
                zoomCounter = 5;
            }
        }
    }

    // Mouse wheel detection
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton) 
        {
            InputEventMouseButton e = (InputEventMouseButton) @event;
            if (e.IsPressed())
            {
                if (e.ButtonIndex == (int) ButtonList.WheelUp)
                {
                    if (this.Zoom.x > 0.5 && this.Zoom.y > 0.5)
                    {
                        zoom = ZOOM.In;
                        zoomCounter = 5;
                    }
                }
                if (e.ButtonIndex == (int) ButtonList.WheelDown)
                {
                    if (this.Zoom.x < 1.3 && this.Zoom.y < 1.3)
                    {
                        zoom = ZOOM.Out;
                        zoomCounter = 5;
                    }
                }  
            }
        } 
    }

    // Keyboard input map check
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

    // The mouse moves the screen if it touches the border
    public void _CalculateBorderMouse()
    {
        float viewportX = GetViewport().Size.x;
        float viewportY = GetViewport().Size.y;

        float mouseX = GetViewport().GetMousePosition().x;
        float mouseY = GetViewport().GetMousePosition().y;

        if (mouseX > (viewportX - 5))
            Translate(new Vector2(20, 0));
        if (mouseX < (viewportX - (viewportX - 5)))
            Translate(new Vector2(-20, 0));
 	    if (mouseY > (viewportY - 5))
		    Translate(new Vector2(0, 20));
	    if (mouseY < (viewportY - (viewportY - 5)))
		    Translate(new Vector2(0, -20));     
    }

    // Smooth zoom calculations
    public void _CalculateZoom()
    {
        if (zoomCounter > 0) 
        {
            if (zoom == ZOOM.In) 
            {
                this.Zoom = new Vector2(this.Zoom.x - ZOOM_FACTOR, this.Zoom.y - ZOOM_FACTOR);
            }
            if (zoom == ZOOM.Out)
            {
                this.Zoom = new Vector2(this.Zoom.x + ZOOM_FACTOR, this.Zoom.y + ZOOM_FACTOR);
            }
            zoomCounter--;
        }
    }
}
