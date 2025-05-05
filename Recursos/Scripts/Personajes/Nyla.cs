using Godot;
using System;

public partial class Nyla : JugadorControl
{

	[Export] float VelocidadBase = 7;
	float GravedadBase = 40;
	float FriccionBase = 10;
	float FuerzaSaltoBase = 12;

	bool TieneEstamina;
	bool RecargandoEstamina;

	int NumAtaque;
	bool Ataque;

	public override void _Ready()
	{
		base._Ready();

		ParticulasInit();
		SetVelocidad(VelocidadBase);
		SetFriccion(FriccionBase);
		SetGravedad(GravedadBase);
		SetFuerzaSalto(FuerzaSaltoBase);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		ColisionControl();
		RotacionControl(delta);
		MovimientoControl(delta);
		EstaminaControl(delta);
		
		MoveAndSlide();

		DirMovCamaraControl();
		AnimacionControl();
		SprintControl();
		SaltoControl();
		ParticulasControl();
		SfxControl();
		EmpujeControl();
	}
  
	private void AnimacionControl()
	{
		AnimationTree Animacion = GetNode<AnimationTree>("Animacion");
		float TimeLength = GetVelocidad() <= 5 ? 1.0f : (GetVelocidad() <= 15 ? 1.5f : (GetVelocidad() <= 50 ? 1.8f :1.6f));
	   // Ajustar la longitud de la animación en función de VelocidadBase
		Animacion.Set("parameters/TimeWalk/scale",TimeLength);
		Animacion.Set("parameters/TimeRun/scale",TimeLength);
		Animacion.Set("parameters/TransicionMov/transition_request",  IsOnFloor() ? (GetDirMov() != Vector3.Zero ? (GetVelocidad()>VelocidadBase? "Run" : "Walk") : TieneEstamina ? "Idle" : "IdleRest") : "Fall");
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("click") && !(bool)Animacion.Get("parameters/OneShot/active"))
			{
				NumAtaque = Mathf.Wrap(NumAtaque,0,3);
				NumAtaque += 1; 
				Animacion.Set("parameters/Ataques/transition_request", (int)NumAtaque);
				Animacion.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
			}
		}
		else
		{
			Animacion.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Abort);
		}
		
		Ataque = (bool)Animacion.Get("parameters/OneShot/active");
	}
	private void RotacionControl(double delta)
	{
		Rotacion(delta);
	}
	private void DirMovCamaraControl()
	{
		GetGimbal().Position = Position;
		Vector3 DirMov = new Vector3(Input.GetAxis("a","d"),0,Input.GetAxis("w","s"));
		DirMov = DirMov.Rotated(Vector3.Up, GetGimbal().Rotation.Y).Normalized();
		SetDirMov(DirMov);
	}
	private void MovimientoControl(double delta)
	{
		Movimientos(delta);
	}
	private void EmpujeControl()
	{
		float FuerzaEmpujeBase;
		if (GetDirMov() == Vector3.Zero)
		{
			FuerzaEmpujeBase = 0;
		}
		else
		{
			FuerzaEmpujeBase = 10;
		}
		SetFuerzaEmpuje(GetVelocidad() * FuerzaEmpujeBase);
		Empuje();
	}
	private void SprintControl()
	{
		SetVelocidad(TieneEstamina ? (Input.IsActionPressed("shift") ? VelocidadBase * 1.5f : VelocidadBase) : VelocidadBase * 0.4f);
	}
	private void SaltoControl()
	{
		if (IsOnFloor())
		{
			if (TieneEstamina)
			{
				if (Input.IsActionJustPressed("space"))
				{
					Salto();
				}
			}
		}
	  
	}
	private void EstaminaControl(double delta)
	{
		TextureProgressBar Estamina = GetNode<TextureProgressBar>("UI/Info/BarraEstamina");
		Estamina.MaxValue  = Mathf.Clamp(Estamina.MaxValue, 1000, 10000);        

		Estamina.Value += RecargandoEstamina ? (GetDirMov() == Vector3.Zero ?  (float)delta * (Estamina.MaxValue * 0.50) : (float)delta * (Estamina.MaxValue * 0.30)) :
		Input.IsActionJustPressed("space") && IsOnFloor() ? -Estamina.MaxValue * 0.15 : 
		(GetDirMov() == Vector3.Zero ? (float)delta * (Estamina.MaxValue * 0.40) : 
		Input.IsActionPressed("shift") ? -(float)delta * 250 : (float)delta * (Estamina.MaxValue * 0.35)); 

		TieneEstamina = !RecargandoEstamina && Estamina.Value >= Estamina.MinValue; 
		RecargandoEstamina =  RecargandoEstamina == true ? Estamina.Value < Estamina.MaxValue && RecargandoEstamina : Estamina.Value <= Estamina.MinValue || RecargandoEstamina;
		Estamina.TintProgress = RecargandoEstamina ? new Color("FF0000") : Estamina.Value >= Estamina.MaxValue * 0.5f ? new Color("00FF00") : Estamina.Value >= Estamina.MaxValue * 0.25 ? new Color("FFFF00") : new Color("FF7F00");

		Estamina.Visible = Estamina.Value >= Estamina.MaxValue ? false : true;    
	}
	
	private void ParticulasControl()
	{
		GetParticulasControl().GetParticulasUno().Emitting =  IsOnFloor() && (GetDirMov() != Vector3.Zero);
	
	GetParticulasControl().GetParticulasDos().Emitting =  !GetParticulasControl().GetParticulasDos().Emitting ? TieneEstamina && IsOnFloor() && Input.IsActionJustPressed("space") : GetParticulasControl().GetParticulasDos().Emitting;
	

	}
	private void ParticulasInit()  
	{
		GetParticulasControl().GetParticulasUno().ProcessMaterial = GetParticulasControl().EfectoCaminarParticula;
		GetParticulasControl().GetParticulasUno().DrawPass1 = GetParticulasControl().MallaPolvoParticula;
		GetParticulasControl().GetParticulasUno().Amount = 20;
		GetParticulasControl().GetParticulasUno().Position = new Vector3(0,-0.6f,0);
		
		GetParticulasControl().GetParticulasDos().Emitting =  false;
		GetParticulasControl().GetParticulasDos().ProcessMaterial = GetParticulasControl().EfectoSaltoParticula;
		GetParticulasControl().GetParticulasDos().Position = new Vector3(0,-0.6f,0);
		GetParticulasControl().GetParticulasDos().Amount = 20;
		GetParticulasControl().GetParticulasDos().OneShot = true;
		GetParticulasControl().GetParticulasDos().Explosiveness = 0.8f;
		GetParticulasControl().GetParticulasDos().Lifetime = 0.3f;
		GetParticulasControl().GetParticulasDos().DrawPass1 = GetParticulasControl().MallaPolvoParticula;
	}
	
	private void SfxControl()
	{
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("space"))
			{
				if (TieneEstamina)
				{
					GetVfx().GetReproductor().Stream = GetVfx().JumpSound;
					GetVfx().GetReproductor().Playing = true;
					GetVfx().GetReproductor().VolumeDb = -5;
				}
			}
		}
		if (GetVfx().GetReproductorDos().Stream == null)
		{
			GetVfx().GetReproductorDos().Stream = GetVfx().WalkSound;
			GetVfx().GetReproductorDos().Playing = true;
			GetVfx().GetReproductorDos().VolumeDb = -5;
		}
		GetVfx().GetReproductorDos().StreamPaused = !IsOnFloor() || (GetDirMov() == Vector3.Zero ? true : false);
		GetVfx().GetReproductorDos().PitchScale = GetDirMov() == Vector3.Zero ?
		0.1f : TieneEstamina ? 
		(GetVelocidad() > VelocidadBase ? 
		(float)GenerarNumeroAleatorioEnRango(1.5f,1.7f) : (float)GenerarNumeroAleatorioEnRango(1.0f,1.2f)) : (float)GenerarNumeroAleatorioEnRango(0.7f,0.9f);
	}
	private void ColisionControl()
	{
		RayCast3D GroundDetector = GetNode<RayCast3D>("GroundDetector");
		if (GroundDetector.IsColliding())
		{
			SetFriccion((GroundDetector.GetCollider() as Node3D).IsInGroup("Hielo") ? 1 : (GroundDetector.GetCollider() as Node3D).IsInGroup("Suelo") ? 10: 7);
		}
	}
}
