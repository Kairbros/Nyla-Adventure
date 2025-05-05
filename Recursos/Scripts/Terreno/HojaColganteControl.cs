using Godot;
using System;

public partial class HojaColganteControl : ObjetoRigidoControl
{

	[Export] float TiempoCaida = 0.4f;
	[Export] float TiempoRecuperada = 6f;

	RigidBody3D Hoja;
	bool JugadorEntro;


    public override void _Ready()
    {
		base._Ready();
		
		ParticulasInit();
		Hoja = GetNode<RigidBody3D>("Hoja");
		SetCronometro(TiempoCaida);
		SetCronometroDos(TiempoRecuperada);
    }
    
	public override void _PhysicsProcess(double delta)
    {	
		base._PhysicsProcess(delta);

		JugadorDetector();
		CaidaPlataformaControl(delta);	
    }
	private void ParticulasInit()
	{
		GetParticulas().GetParticulasUno().Emitting = false;
		GetParticulas().GetParticulasUno().DrawPass1 = GetParticulas().MallaHechizoParticula;
		GetParticulas().GetParticulasUno().ProcessMaterial = GetParticulas().EfectoHechizoParticula;
		GetParticulas().GetParticulasUno().OneShot = true;
		GetParticulas().GetParticulasUno().Amount = 50;
		GetParticulas().GetParticulasUno().Lifetime = 0.5f;
		GetParticulas().GetParticulasUno().FixedFps = 60;
	}
	
	private void CaidaPlataformaControl(double delta)
	{	
		if (JugadorEntro)
		{
			SetCronometro(GetCronometro() - (float)delta);
			if (GetCronometro() <= 0.0)
			{
				Hoja.GravityScale  = 5;
				SetCronometroDos(GetCronometroDos() - (float)delta);
				if (GetCronometroDos() <= 0.0)
				{
					RecoverControl();
					SetCronometro(TiempoCaida);
					SetCronometroDos(TiempoRecuperada);
					JugadorEntro = false;
				}	
			}
			else
			{
				Hoja.LinearVelocity = Vector3.Zero;
			}
		}
		else
		{
			Hoja.Rotation = Hoja.Rotation.MoveToward(Vector3.Zero,3*(float)delta);
			Hoja.Position = Hoja.Position.MoveToward(Vector3.Zero,(float)delta);
			// MoveToward Mueve un Numero, o un Vector a otro en una velocidad determinada por un tercer parametro, es mas preciso que Lerp
			Hoja.GravityScale  = 0;
		}
	}
	private void JugadorDetector()
	{
		if (!JugadorEntro)
		{
			foreach (var i in Hoja.GetCollidingBodies())
			{
				if (i is JugadorControl)
				{
					ShakeControl();
					JugadorEntro = true;
					break;
				}
			}
		}
	}
	private void RecoverControl()
	{
		GetParticulas().GetParticulasUno().Emitting = true;
		GetSfx().GetReproductorDos().Stream = GetSfx().SpellSound;
		GetSfx().GetReproductorDos().Play();
		GetSfx().GetReproductorDos().VolumeDb = 5;
	}
	private void ShakeControl()
	{
		GetSfx().GetReproductor().Stream = GetSfx().BreakSound;
		GetSfx().GetReproductor().Play();
		ShakeTween(Hoja);
	}
}
