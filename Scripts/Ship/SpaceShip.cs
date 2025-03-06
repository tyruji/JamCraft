using Godot;
using System;

public class SpaceShip : KinematicBody
{
    [Export]
    public float MaxSpeed { get; set; } = 20;

    [Export]
    public float Acceleration { get; set; } = 4.5f;
    
    [Export]
    public float Deacceleration { get; set; } = 16;

    [Export]
    public float MaxRotationSpeed { get; set; } = 5;

    [Export]
    public float RotationAcceleration { get; set; } = 4.5f;
    
    [Export]
    public float RotationDeacceleration { get; set; } = 16;

    [Export]
    public float HoverSwayFrequency { get; set; } = 5f;

    [Export]
    public float HoverHeight { get; set; } = 3f;

    [Export]
    public float HoverSwayAmplitude { get; set; } = .2f;

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

    public Vector3 angularVelocity = Vector3.Zero;

    public Vector3 inputDirection = Vector3.Zero;

    public float forwardAcceleration = 0;

    private Vector3 _rotationDirection = Vector3.Zero;

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
        if( !EnableControl && !IsOnFloor() )
        {
            velocity.y = -HoverSwayDampening;
            velocity = MoveAndSlide( velocity, Vector3.Up );
        }

        if( !EnableControl ) return;
        
        HandleInput( delta );
        HandleMovement( delta );
    }

    public void HandleInput( float delta )
    {
        _rotationDirection = Vector3.Zero;

        inputDirection = Vector3.Zero;

        if( Input.IsActionPressed( InputActions.FORWARD ) ) inputDirection += Vector3.Forward;
        if( Input.IsActionPressed( InputActions.BACK ) ) inputDirection += Vector3.Back;
        if( Input.IsActionPressed( InputActions.RIGHT ) ) inputDirection += Vector3.Right;
        if( Input.IsActionPressed( InputActions.LEFT ) ) inputDirection += Vector3.Left;

        forwardAcceleration = inputDirection.z;
        _rotationDirection += inputDirection.x * CameraPivot.GlobalTransform.basis.x.Normalized();
        //inputDirection = inputDirection.Normalized();
        //_rotationDirection += inputDirection.z * CameraFollow.GlobalTransform.basis.z.Normalized();

            // Capture or free cursor
        if( Input.IsActionJustPressed( InputActions.CANCEL ) )
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible
                ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;   
        }
    }

    public void HandleMovement( float delta )
    {
            // Hover up and down periodically
        var hover_vel = GetHoverSwayElevationVelocity();
        velocity.y = Mathf.Lerp( velocity.y, hover_vel, HoverSwayDampening * delta );

        Vector3 forward_dir = GlobalTransform.basis.z.Normalized();
        //float rot_dot = _rotationDirection.Dot( forward_dir );
        //float rot_coeff = Mathf.Sign( rot_dot ) - rot_dot;

        //RotateY( rot_dot * RotationSpeed * delta );

        var rot_acc = inputDirection.x == 0 ? RotationDeacceleration : RotationAcceleration;
        var y_rot = forwardAcceleration * inputDirection.x * MaxRotationSpeed;
        angularVelocity.y = Mathf.Lerp( angularVelocity.y, y_rot, rot_acc * delta );
        RotateY( angularVelocity.y * delta );


        var horizontal_vel = velocity;
        horizontal_vel.y = 0;

        var forward_acc = forwardAcceleration * forward_dir;
        var acc = forwardAcceleration > 0 ? Acceleration : Deacceleration;

        horizontal_vel = horizontal_vel.LinearInterpolate( forward_acc * MaxSpeed, acc * delta );

        velocity.x = horizontal_vel.x;
        velocity.z = horizontal_vel.z;


        velocity = MoveAndSlide( velocity, Vector3.Up );
    }

    private float GetHoverSwayElevationVelocity()
    {
        float lambda = 2f * Mathf.Pi / HoverSwayFrequency;
        
        if( _engineRunningTime >= lambda ) 
        {
            return HoverSwayFrequency * Mathf.Cos( HoverSwayFrequency * _engineRunningTime );
        }

        float a = lambda * HoverSwayFrequency * HoverSwayAmplitude - HoverHeight;
        a /= lambda * lambda;

        return 2f * a * ( _engineRunningTime - lambda ) + HoverSwayFrequency * HoverSwayAmplitude;
    }
}
