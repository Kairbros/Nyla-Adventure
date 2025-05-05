using Godot;
using System;

public partial class SfxControl : Node3D
{

    public AudioStream JumpSound = (AudioStream)ResourceLoader.Load("res://Recursos/VFX/Jump.wav");
    public AudioStream WalkSound = (AudioStream)ResourceLoader.Load("res://Recursos/VFX/Walk.wav");
    public AudioStream SpellSound = (AudioStream)ResourceLoader.Load("res://Recursos/VFX/Hechizo.wav");
	public AudioStream BreakSound = (AudioStream)ResourceLoader.Load("res://Recursos/VFX/Crunch.wav");

    AudioStreamPlayer3D Reproductor;
    AudioStreamPlayer3D ReproductorDos;

    public override void _Ready()
    {
        Reproductor = new AudioStreamPlayer3D();
        ReproductorDos = new AudioStreamPlayer3D();
        AddChild(Reproductor);
        AddChild(ReproductorDos);
    }
    public AudioStreamPlayer3D GetReproductor()
    {
        return Reproductor;
    }
    public AudioStreamPlayer3D GetReproductorDos()
    {
        return ReproductorDos;
    }

}
