using Godot;
using System;

public class CameraHandler : Camera
{
    [Export]
    private float _PlayerFollowPositionDampening { get; set; } = 33f;
    
    [Export]
    private float _PlayerFollowRotationDampening { get; set; } = 33f;

    [Export]
    private float _TargetFollowPositionDampening { get; set; } = 18f;
    
    [Export]
    private float _TargetFollowRotationDampening { get; set; } = 12f;

    public Spatial FollowTarget { get; set; } = null;

    public eCameraState CameraState { get; set; } = eCameraState.PLAYER_FIRSTPERSON;

    private Player _player = null;


    public override void _Ready()
    {
        _player = GetNode<Player>( "../Player" );
    }

    public override void _Process( float delta )
    {
        switch( CameraState )
        {
            case eCameraState.PLAYER_FIRSTPERSON:
                var rot_damp = _PlayerFollowRotationDampening;
                var pos_damp = _PlayerFollowPositionDampening;
                var tr = GlobalTransform.Orthonormalized();

                tr.basis = tr.basis.Slerp( _player.Head.GlobalTransform.Orthonormalized().basis, 
                    delta * rot_damp );

                tr.origin = tr.origin.LinearInterpolate( _player.Head.GlobalTransform.origin,
                    delta * pos_damp );

                GlobalTransform = tr;
                break;

            case eCameraState.SMOOTH_FOLLOW_TARGET:
                rot_damp = _TargetFollowRotationDampening;
                pos_damp = _TargetFollowPositionDampening;
                tr = GlobalTransform.Orthonormalized();

                tr.basis = tr.basis.Slerp( FollowTarget.GlobalTransform.Orthonormalized().basis, 
                    delta * rot_damp );

                tr.origin = tr.origin.LinearInterpolate( FollowTarget.GlobalTransform.origin,
                    delta * pos_damp );

                GlobalTransform = tr;
                break;
        }
    }
}

public enum eCameraState 
{
    PLAYER_FIRSTPERSON,
    SMOOTH_FOLLOW_TARGET,
}