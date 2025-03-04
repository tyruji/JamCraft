using Godot;
using System;

public class AutomaticDoor : Door
{
    private int _bodiesInRange = 0;

    private bool _closed = true;

    public override void _Process( float delta )
    {
        if( _animPlayer.IsPlaying() ) return;

        if( _bodiesInRange == 0 && !_closed )
        {
            Close();
            _closed = true;
        }
        else if( _bodiesInRange > 0 && _closed )
        {
            Open();
            _closed = false;
        }
    }

    public void OnBodyEntered( Node body )
    {
            // Only detect moving objects.
        if( !( body is KinematicBody ) ) return;
        ++_bodiesInRange;
    }

    public void OnBodyExited( Node body )
    {
        if( !( body is KinematicBody ) ) return;
        --_bodiesInRange;
    }
}
