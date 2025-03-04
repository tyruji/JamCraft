using Godot;
using System;

[Tool]
public class SimpleRotator : Spatial
{
    [Export]
    private Vector3 _RotationAxis = Vector3.Right;

    [Export]
    private float _RotationSpeed = 10.0f;

    public override void _Process( float delta )
    {
        RotationDegrees += _RotationSpeed * delta * _RotationAxis;
    }
}
