using Godot;
using System;

public partial class PuertaControl : ObjetoRigidoControl
{
	float TiempoRecuperada = 3f;
	RigidBody3D Puerta;
	bool JugadorEntro;

	public override void _Ready()
	{
		SetCronometro(TiempoRecuperada);
		Puerta = GetNode<RigidBody3D>("Puerta");
		base._Ready();
	}
	public override void _PhysicsProcess(double delta)
	{
		AbrirCerrarPuerta(delta);
		JugadorDetector();
		base._PhysicsProcess(delta);
	}
	
	private void JugadorDetector()
	{
			foreach (var i in Puerta.GetCollidingBodies())
			{
				if (i is JugadorControl)
				{
					JugadorEntro = true;
					break;
				}
				else
				{
					JugadorEntro = false;
					break;
				}
			}

	}
	private void AbrirCerrarPuerta(double delta)
	{
		if (!JugadorEntro)
		{
			SetCronometro(GetCronometro()-(float)delta);
			if (GetCronometro()<= 0.0)
			{
				Puerta.Rotation = Puerta.Rotation.MoveToward(Vector3.Zero,(float)delta);
				Puerta.Position = Puerta.Position.MoveToward(Vector3.Zero,(float)delta);
				Puerta.LinearVelocity = Vector3.Zero;
			}
		}
		else
		{
			SetCronometro(TiempoRecuperada);
		}

	}


}
