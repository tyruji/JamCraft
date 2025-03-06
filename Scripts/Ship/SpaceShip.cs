using Godot;
using System;

public class SpaceShip : KinematicBody
{
    [Export]
    public float HoverSwayFrequency { get; set; } = 5f;

    [Export]
    public float HoverSwayAmplitude { get; set; } = 3f;

    [Export]
    public float HoverSwayDampening { get; set; } = 10;

    [Export]
    public float MouseSensitivity { get; set; } = .12f;

    [Export]
    public float TiltLimit { get; set; } = 75;

    public bool EnableControl 
    {
        get => _enableControl;
        set
        {
            _enableControl = value;   
            _engineRunningTime = 0f;
        }
    }
    private bool _enableControl = false;

    public Spatial CameraPivot { get; set; }

    public Spatial CameraFollow { get; set; }

    public Vector3 velocity = Vector3.Zero;

    private float _engineRunningTime = 0;

    public override void _Ready()
    {
        CameraPivot = GetNode<Spatial>( nameof( CameraPivot ) );
        CameraFollow = CameraPivot
                    .GetNode( nameof( SpringArm ) )
                    .GetNode<Spatial>( nameof( CameraFollow ) );
    }

    public override void _Input( InputEvent @event )
    {
        if( !EnableControl ) return;

        if( !( @event is InputEventMouseMotion mouse_event ) ) return;

        var rot = CameraPivot.Rotation;

        rot.x -= mouse_event.Relative.y * MouseSensitivity;
        var angle_lim = Mathf.Deg2Rad( TiltLimit );
        rot.x = Mathf.Clamp( rot.x, -angle_lim, angle_lim );
        rot.y -= mouse_event.Relative.x * MouseSensitivity;
        CameraPivot.Rotation = rot;
    }

    public override void _Process( float delta )
    {
        if( !EnableControl ) return;

        _engineRunningTime += delta;
    }

    public override void _PhysicsProcess( float delta )
    {
        if( !EnableControl ) return;
        
        HandleInput( delta );
        HandleMovement( delta );
    }

    public void HandleInput( float delta )
    {
        
    }

    public void HandleMovement( float delta )
    {
            // Hover up and down periodically
        var hover_vel = GetHoverSwayElevationVelocity();
        velocity.y = Mathf.Lerp( velocity.y, hover_vel, HoverSwayDampening * delta );

        velocity = MoveAndSlide( velocity, Vector3.Up );
    }

    private float GetHoverSwayElevationVelocity()
    {
        float lambda = 2f * Mathf.Pi / HoverSwayFrequency;
        
        if( _engineRunningTime >= lambda ) 
        {
            return HoverSwayFrequency * Mathf.Cos( HoverSwayFrequency * _engineRunningTime );
        }

        float a = ( lambda * HoverSwayFrequency - HoverSwayAmplitude ) / ( lambda * lambda );

        return 2f * a * ( _engineRunningTime - lambda ) + HoverSwayFrequency;
    }
}
