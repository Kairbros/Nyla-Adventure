using System;
using Godot;


public partial class JugadorControl : CharacterBody3D
{
    float Velocidad;
    float FuerzaSalto;
    float Friccion;
    float FuerzaGravedad;
    float FuerzaEmpuje;
    String Lugar;

    Vector3 DirMov;

    GimbalControl Gimbal;
    SfxControl Sonido;
    ParticulasControl Particulas;

    public override void _Ready() 
    {
        Particulas = new ParticulasControl();
        AddChild(Particulas);
        Gimbal = new GimbalControl()
        {
            TopLevel = true,
            Position = Position,
            SpringLength = 15,
            RotationDegrees = new Vector3(-45,0,0)
        };
        AddChild(Gimbal);
        Sonido = new SfxControl();
        AddChild(Sonido);
    }

    public override void _PhysicsProcess(double delta)
    {
        LimitarVariables();   
    }

    private void LimitarVariables()
    {
        Velocity = new Vector3(Velocity.X,Mathf.Clamp(Velocity.Y,-FuerzaGravedad*2,FuerzaGravedad*10),Velocity.Z);
        //Limites de toda la velocidad a la que pueda llegar el personaje si no esta en el piso
    }

    public void SetFriccion(float newFriccion)
    {
        Friccion = newFriccion;
    }   
    public void SetVelocidad(float newVelocidad)
    {
        Velocidad = newVelocidad;
    }
    public void SetFuerzaSalto(float newFuerzaSalto)
    {
        FuerzaSalto = newFuerzaSalto;
    }
    public void SetGravedad(float newFuerzaGravedad)
    {
        FuerzaGravedad = newFuerzaGravedad;
    }
    public void SetDirMov(Vector3 newDirMov)
    {
        DirMov = newDirMov;
    }
    public void SetLugar(String newLugar)
    {
        Lugar = newLugar;
    }
    public void SetFuerzaEmpuje(float newFuerzaEmpuje)
    {
        FuerzaEmpuje = newFuerzaEmpuje;
    }

    public SfxControl GetVfx()
    {
        return Sonido;
    }
    public float GetFriccion()
    {
        return Friccion;
    }
    public String GetLugar()
    {
        return Lugar;
    }
    public float GetFuerzaGravedad()
    {
        return FuerzaGravedad;
    }
    public float GetVelocidad()
    {
        return Velocidad;
    }
    public Vector3 GetDirMov()
    {
        return DirMov;
    } 
    public float GetFuerzaSalto()
    {
        return FuerzaSalto;
    }
    public GimbalControl GetGimbal()
    {
        return Gimbal;
    }
    public ParticulasControl GetParticulasControl()
    {
        return Particulas;
    }

    public void Empuje()
    {
        if (GetSlideCollisionCount() > 0)
        {
            for (int i = 0; i < GetSlideCollisionCount(); i++) 
            {
                var Colisionador = GetSlideCollision(i);
                if (Colisionador.GetCollider() as RigidBody3D is RigidBody3D)
                {
                    Vector3 DirPush = ((Colisionador.GetCollider() as RigidBody3D).GlobalPosition - GlobalPosition ).Normalized();
                    (Colisionador.GetCollider() as RigidBody3D).ApplyCentralForce(FuerzaEmpuje * DirPush);
                }
            }    
        }
    }
    public void Desplazamiento(Vector3 DirDash)
    {
        // Da un breve impulso en el eje horizontal
        Velocity = new Vector3(Velocidad * DirDash.X , Velocity.Y, Velocidad * DirDash.Z);
    }
    public void Salto()
    {
        // Da un breve impulso en el eje verical
        Velocity = new Vector3(Velocity.X,FuerzaSalto,Velocity.Z);
    }
    public void Movimientos(double delta)
    {
        Velocity = new Vector3(Mathf.Lerp(Velocity.X, Velocidad * DirMov.X, Friccion * (float)delta), Velocity.Y, Mathf.Lerp(Velocity.Z, Velocidad * DirMov.Z, Friccion * (float)delta));
        // Esta velocidad funciona en medida de la interpolacion de un Float hacia el numero que se le asigne en medida de un tercer parametro
        // Se mueve y se frena dependiendo si "DirMov" es != (0,0,0) o == (0,0,0)
        // Dato: Velocity de esta manera nunca llega a (0,0,0) debido a que la interpolacion no es precisa, quedando en ocaciones por ejemplo:
        // (-45E-45,0,-45E-45) Este numero equivaliendo a −0.000000000000000000000000000000000000000000045 Aproximadamente, no siendo capaz de mover el personaje pero si conservar la rotacion
        if (!IsOnFloor())
        {
            Velocity -= new Vector3(0, FuerzaGravedad * (float)delta, 0);
            // Gravedad si no esta en el piso
        }

    }
    public void Rotacion(double delta)
    {  
        // La rotacion es controlada directamente por el parametro de velocidad "Velocity" 
        // este crea un angulo entre la velocidad en el eje X y Z siempre y cuando estos no sean 0 para posteriormente interpolar el angulo entre el eje de rotacion Y hacia el angulo de el movimiento de velocidad X y Z
        if (Velocity.X != 0 || Velocity.Z != 0)
        {
            Vector2 AngleMov = new Vector2(Velocity.Z,Velocity.X);
            Rotation = new Vector3(0,Mathf.LerpAngle(Rotation.Y,AngleMov.Angle(),15*(float)delta),0);  
        }
    }
    
    public float GenerarNumeroAleatorioEnRango(float min, float max)
    {
        Random random = new Random();
        return (float)random.NextDouble() * (max - min) + min;
    }
}
