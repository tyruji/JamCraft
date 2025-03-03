using Godot;
using System;

public class CameraHandler : Camera
{
    [Export]
    private float _PositionDampening { get; set; } = 8.0f;
    
    [Export]
    private float _RotationDampening { get; set; } = 18.0f;

    public eCameraState CameraState { get; set; } = eCameraState.PLAYER_FIRSTPERSON;

    private Player _player = null;


    public override void _Ready()
    {
        _player = GetNode<Player>( "../Player" );
    }

    public override void _PhysicsProcess( float delta )
    {
        switch( CameraState )
        {
            case eCameraState.PLAYER_FIRSTPERSON:
                var tr = GlobalTransform.Orthonormalized();
                tr.basis = tr.basis.Slerp( _player.Head.GlobalTransform.Orthonormalized().basis, 
                    delta * _RotationDampening );

                tr.origin = tr.origin.LinearInterpolate( _player.Head.GlobalTransform.origin,
                    delta * _PositionDampening );

                GlobalTransform = tr;
                break;
        }
        

    }
}

public enum eCameraState 
{
    PLAYER_FIRSTPERSON,
    SMOOTH_FOLLOW_TARGET
}