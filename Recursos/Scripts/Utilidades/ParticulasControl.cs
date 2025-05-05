using Godot;
using System;

public partial class ParticulasControl : Node3D
{
    public ParticleProcessMaterial EfectoHechizoParticula = (ParticleProcessMaterial)ResourceLoader.Load("res://Recursos/Materiales/Particulas/EfectoHechizoParticula.tres");
    public ParticleProcessMaterial EfectoCaminarParticula = (ParticleProcessMaterial)ResourceLoader.Load("res://Recursos/Materiales/Particulas/EfectoCaminarParticula.tres");
    public ParticleProcessMaterial EfectoSaltoParticula = (ParticleProcessMaterial)ResourceLoader.Load("res://Recursos/Materiales/Particulas/EfectoSaltoParticula.tres");

    public BoxMesh MallaPolvoParticula = (BoxMesh)ResourceLoader.Load("res://Recursos/Materiales/Particulas/MallaPolvoParticula.tres");
    public BoxMesh MallaHechizoParticula = (BoxMesh)ResourceLoader.Load("res://Recursos/Materiales/Particulas/MallaHechizoParticula.tres");

    GpuParticles3D ParticulasUno;
    GpuParticles3D ParticulasDos;
    
    public override void _Ready()
    {
        ParticulasUno = new GpuParticles3D();
        ParticulasDos = new GpuParticles3D();
        AddChild(ParticulasUno);
        AddChild(ParticulasDos);
    }
    public GpuParticles3D GetParticulasUno()
    {
        return ParticulasUno;
    }
    public GpuParticles3D GetParticulasDos()
    {
        return ParticulasDos;
    }
}
