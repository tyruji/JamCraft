using Godot;
using System;

public class ShipInteractHandler : Area, IInteractable, IHighlightable
{
    [Export]
    public MeshInstance HighlightModel { get; set; }

    public SpaceShip SpaceShip { get; set; } = null;

    public Player Player { get; set; }

    public Spatial PlayerLeaveShipPoint { get; set; }

    public override void _Ready()
    {
        HighlightModel = GetNode<MeshInstance>( nameof( HighlightModel ) );
        SpaceShip = GetParent<SpaceShip>();
        PlayerLeaveShipPoint = SpaceShip.GetNode<Spatial>( nameof( PlayerLeaveShipPoint ) );
    }

    public override void _Process( float delta )
    {
        if( !SpaceShip.EnableControl ) return;

        HandleLeavingShip();
    }

    public void Interact( Node interactant )
    {
        if ( !( interactant is Player player ) ) return;

        Player = player;

        CameraHandler cam_handle = GetViewport().GetCamera() as CameraHandler;

        cam_handle.CameraState = eCameraState.SMOOTH_FOLLOW_TARGET;
        cam_handle.FollowTarget = SpaceShip.CameraFollow;
        Player.EnableControl = false;
        Player.selectedHighlightable = null;
        SpaceShip.EnableControl = true;
        Unhighlight();
    }

    public void HandleLeavingShip()
    {
            // ESC
        if( !Input.IsActionJustPressed( InputActions.CANCEL ) ) return;
        
        CameraHandler cam_handle = GetViewport().GetCamera() as CameraHandler;

        cam_handle.CameraState = eCameraState.PLAYER_FIRSTPERSON;
        cam_handle.FollowTarget = null;
        SpaceShip.EnableControl = false;
        Player.GlobalTranslation = PlayerLeaveShipPoint.GlobalTranslation;
        Player.EnableControl = true;
    }

    public void Highlight()
    {
        HighlightModel.Show();
    }

    public void Unhighlight()
    {
        HighlightModel.Hide();
    }
}
