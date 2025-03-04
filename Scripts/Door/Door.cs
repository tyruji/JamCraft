using Godot;
using System;

public class Door : Spatial
{
    [Export]
    private AudioStream _OpenSound { get; set; } = null;

    [Export]
    private Vector2 _OpenSfxVolumeRange { get; set; } = new Vector2( -.1f, .1f );

    [Export]
    private Vector2 _OpenSfxPitchRange { get; set; } = new Vector2( 1, 1.1f );

    [Export]
    private AudioStream _CloseSound { get; set; } = null;
 
    [Export]
    private Vector2 _CloseSfxVolumeRange { get; set; } = new Vector2( -.1f, .1f );

    [Export]
    private Vector2 _CloseSfxPitchRange { get; set; } = new Vector2( 1, 1.1f );

    [Export]
    private float _SfxUnitSize { get; set; } = 5;

    protected AnimationPlayer _animPlayer = null;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>( nameof( AnimationPlayer ) );
    }

    public void Open()
    {
        _animPlayer.Play( nameof( Open ) );

        if( _OpenSound == null ) return;

        var sfx_settings = new RandomizedSoundSettings( _OpenSfxVolumeRange, _OpenSfxPitchRange );
        AudioManager.Instance.Play3D( _OpenSound, GlobalTranslation, sfx_settings, _SfxUnitSize );
    }

    public void Close()
    {
        _animPlayer.Play( nameof( Close ) );

        if( _CloseSound == null ) return;

        var sfx_settings = new RandomizedSoundSettings( _CloseSfxVolumeRange, _CloseSfxPitchRange );
        AudioManager.Instance.Play3D( _CloseSound, GlobalTranslation, sfx_settings, _SfxUnitSize );
    }
}
