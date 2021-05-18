using UnityEngine;

public class InputTouchAdapter : IInputAdapter
{
    private Camera camera;
    private Vector2 lastPosition;

    public InputTouchAdapter(Camera camera)
    {
        this.camera = camera;
    }
    
    public Vector2 GetDirection()
    {
        if (!Input.GetButton("Fire") && !Input.GetMouseButton(0)) return lastPosition;
        
        lastPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }

    public bool GetButton(string name)
    {
        return true;
    }
}