using Godot;
using System;

public class InteractiveStructure : Spatial, ISteppable, IStompable
{
    [Export]
    private AudioStream _StepAudio { get; set; } = null;

    [Export]
    private float _StepCooldown = 1;

    private RandomizedSoundSettings _stepSoundSettings = new RandomizedSoundSettings( -20, -13, .7f, 0.8f );
    private RandomizedSoundSettings _stompSoundSettings = new RandomizedSoundSettings( -16, -13, 1.1f, 1.4f );


    private float _elapsed = 0.0f;

    public override void _Process( float delta )
    {
        _elapsed += delta;
    }

    public void StepOn()
    {
        if( _elapsed < _StepCooldown ) return;

        _elapsed = 0.0f;
        AudioManager.Instance.Play( _StepAudio, _stepSoundSettings );
    }

    public void StompOn()
    {
        _elapsed = 0.0f;
        AudioManager.Instance.Play( _StepAudio, _stompSoundSettings );
    }
}
