using Godot;
using System;

public partial class GimbalControl : SpringArm3D
{
	float Sensibilidad = 0.2f;

	Camera3D Camara;
	public override void _Ready()
	{
		Camara = new Camera3D()
		{
			Far = 200,
			Current = true,
		};
		CollisionMask = 4;
		AddChild(Camara);
		Input.MouseMode = Input.MouseModeEnum.Captured;	
	}

    public override void _Process(double delta)
    {
        Vector2 stickInput = Input.GetVector("derecha", "izquierda", "arriba", "abajo");
        RotationDegrees -= new Vector3(stickInput.Y * (Sensibilidad * 2000) * (float)delta , stickInput.X * (Sensibilidad * 2000) * (float)delta, 0);
        RotationDegrees = RotationDegrees.Clamp(new Vector3(-90, RotationDegrees.Y, 0), new Vector3(90, RotationDegrees.Y, 0));
    }

    public override void _Input(InputEvent @event) 
	{
		if (@event is InputEventMouseMotion MousePos)
		{
			RotationDegrees = RotationDegrees.Clamp(new Vector3(-90,RotationDegrees.Y,0),new Vector3(90,RotationDegrees.Y,0));
			RotationDegrees -= new Vector3(MousePos.Relative.Y * Sensibilidad,MousePos.Relative.X * Sensibilidad,0);
		}
		if (@event is InputEventMouseButton MouseButton)
		{
			SpringLength = Mathf.Clamp(SpringLength,3,20);
			if (MouseButton.ButtonIndex == Godot.MouseButton.WheelUp)
			{
				SpringLength -= 0.5f;
			}
			if (MouseButton.ButtonIndex == Godot.MouseButton.WheelDown)
			{
				SpringLength += 0.5f;
			}
		}
	}

	public Camera3D GetCamera()
	{
		return Camara;
	}
	public Vector3 GetRayDir()
	{
        Vector2 MousePos = GetViewport().GetMousePosition();
        return Camara.ProjectRayNormal(MousePos);
	} 
}
