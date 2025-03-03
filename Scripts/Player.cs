using Godot;
using System;

public class Player : KinematicBody
{
    [Export]
    public float Gravity { get; set; } = 24f;

    [Export]
    public float MaxSpeed { get; set; } = 20;

    [Export]
    public float JumpSpeed { get; set; } = 18;

    [Export]
    public float Acceleration { get; set; } = 4.5f;
    
    [Export]
    public float Friction { get; set; } = 16;

    [Export]
    public float MaxSlopeAngle { get; set; } = 40; 

    [Export]
    public float HeadRotationAngleClamp { get; set; } = 70;

    [Export]
    public float MouseSensitivity { get; set; } = .05f;

    [Export]
    public float HeadBobAmplitude { get; set; } = .2f;
    
    [Export]
    public float HeadBobFrequency { get; set; } = 20f;

    public Spatial Head { get; private set;}

    public Vector3 velocity = Vector3.Zero;

    private Vector3 _inputDirection = Vector3.Zero;

    private float _headBobPhase = 0.0f;

    private Vector3 _initialHeadPosition = Vector3.Zero;

    private bool _jumping = false;

    public override void _Ready()
    {
        Head = GetNode<Spatial>( nameof( Head ) );
        _initialHeadPosition = Head.Translation;

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input( InputEvent @event )
    {
        if( !( @event is InputEventMouseMotion motion_event ) ) return;
        if( Input.MouseMode != Input.MouseModeEnum.Captured ) return;

        Head.RotateX( Mathf.Deg2Rad( -motion_event.Relative.y * MouseSensitivity ) );
        RotateY( Mathf.Deg2Rad( - motion_event.Relative.x * MouseSensitivity ) );

        var head_rot = Head.RotationDegrees;
        head_rot.x = Mathf.Clamp( head_rot.x, -HeadRotationAngleClamp, HeadRotationAngleClamp );
        Head.RotationDegrees = head_rot;
    }

    public override void _PhysicsProcess( float delta )
    {
        HandleInput( delta );
        HandleMovement( delta );
    }

    public override void _Process( float delta )
    {
        HandleStepping();
        HandleStomping();
        HandleHeadBob( delta );
    }

    private void HandleStepping()
    {
        if( !IsOnFloor() || _inputDirection.LengthSquared() == 0 ) return;

        for( int i = 0; i < GetSlideCount(); ++i )
        {
            var col = GetSlideCollision( i );
            if( !( col.Collider is ISteppable steppable ) ) continue;
            steppable.StepOn();
        }
    }
    
    private void HandleStomping()
    {
        if( !IsOnFloor() || !_jumping ) return;

        _jumping = false;

        for( int i = 0; i < GetSlideCount(); ++i )
        {
            var col = GetSlideCollision( i );
            if( !( col.Collider is IStompable stompable) ) continue;
            stompable.StompOn();
        }
    }
    
    private void HandleInput( float delta )
    {
        _inputDirection = Vector3.Zero;
            // Walking
        Vector3 dir = Vector3.Zero;

        if( Input.IsActionPressed( InputActions.FORWARD ) ) dir += Vector3.Forward;
        if( Input.IsActionPressed( InputActions.BACK ) ) dir += Vector3.Back;
        if( Input.IsActionPressed( InputActions.RIGHT ) ) dir += Vector3.Right;
        if( Input.IsActionPressed( InputActions.LEFT ) ) dir += Vector3.Left;

        dir = dir.Normalized();
        _inputDirection += dir.x * Head.GlobalTransform.basis.x.Normalized();
        _inputDirection += dir.z * Head.GlobalTransform.basis.z.Normalized();

            // Jumping
        if( IsOnFloor() && Input.IsActionJustPressed( InputActions.JUMP ) )
        {
            velocity.y = JumpSpeed;
            _jumping = true;
        }

            // Capture or free cursor
        if( Input.IsActionJustPressed( InputActions.CANCEL ) )
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible
                ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;   
        }
    }
    
    private void HandleMovement( float delta )
    {
        velocity.y -= delta * Gravity;

        var horizontal_vel = velocity;
        horizontal_vel.y = 0;

        var acc = _inputDirection.Dot( horizontal_vel ) > 0 ? Acceleration : Friction;

        horizontal_vel = horizontal_vel.LinearInterpolate( _inputDirection * MaxSpeed, acc * delta );

        velocity.x = horizontal_vel.x;
        velocity.z = horizontal_vel.z;
        float slope_angle = Mathf.Deg2Rad( MaxSlopeAngle );

        velocity = MoveAndSlide( velocity, Vector3.Up, true, floorMaxAngle: slope_angle );
    }

    private void HandleHeadBob( float delta )
    {
        if( _inputDirection.LengthSquared() > 0 && IsOnFloor() )
        {
            _headBobPhase += HeadBobFrequency * delta;
        }

        if( _headBobPhase < 0 ) _headBobPhase = 0;
        if( _headBobPhase >= 2 * Mathf.Pi ) _headBobPhase = 0;

        float offset = HeadBobAmplitude * Mathf.Sin( _headBobPhase );

        Head.Translation = _initialHeadPosition + Vector3.Up * offset;
    }
}
