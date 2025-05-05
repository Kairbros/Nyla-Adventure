using Godot;
using System;

public partial class ObjetoRigidoControl : Node3D
{
    SfxControl Sonido;
    Vector3 ParentScale;
    float Cronometro;
	float CronometroDos;
    ParticulasControl Particulas;

    public override void _Ready()
    {
        Particulas = new ParticulasControl();
		AddChild(Particulas);
        ParentScale = Scale;
        Sonido = new SfxControl();
		AddChild(Sonido);
    }

    public SfxControl GetSfx()
    {
        return Sonido;
    }
    public float GetCronometro()
    {
        return Cronometro;
    }
    public float GetCronometroDos()
    {
        return CronometroDos;
    }
    public ParticulasControl GetParticulas()
    {
        return Particulas;
    }

    public void SetCronometro(float NewCronometro)
    {
        Cronometro = NewCronometro;
    }
    public void SetCronometroDos(float NewCronometro)
    {
        CronometroDos = NewCronometro;
    }

    public void ShakeTween(Node3D Objeto)
    {
        Tween tweenRot = GetTree().CreateTween();
		tweenRot.TweenProperty(Objeto,"rotation_degrees",new Vector3(0,10,0),0.08f);
		tweenRot.TweenProperty(Objeto,"rotation_degrees",new Vector3(0,-10,0),0.08f);
		tweenRot.TweenProperty(Objeto,"rotation_degrees",new Vector3(0,10,0),0.08f);
		tweenRot.TweenProperty(Objeto,"rotation_degrees",new Vector3(0,-10,0),0.08f);
		tweenRot.TweenProperty(Objeto,"rotation_degrees",new Vector3(0,10,0),0.08f);
		tweenRot.TweenProperty(Objeto,"rotation_degrees",Vector3.Zero,0.05f);
    }

}
