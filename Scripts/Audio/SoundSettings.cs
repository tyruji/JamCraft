using Godot;

public interface ISoundSettings
{
    float GetVolume();
    float GetPitch();
}

public struct SoundSettings : ISoundSettings
{
    float volume;
    float pitch;

    public SoundSettings( float volume, float pitch )
    {
        this.volume = volume;
        this.pitch = pitch;
    }

    public float GetVolume() => volume;
    public float GetPitch() => pitch;
}

public struct RandomizedSoundSettings : ISoundSettings
{
    public float volume_min;
    public float volume_max;
    public float pitch_min, pitch_max;

    public RandomizedSoundSettings( Vector2 volume_range, Vector2 pitch_range )
    {
        this.volume_min = volume_range.x;
        this.volume_max = volume_range.y;
        this.pitch_min = pitch_range.x;
        this.pitch_max = pitch_range.y;
    }

    public RandomizedSoundSettings( float volume_min, float volume_max,
                                    float pitch_min, float pitch_max )
    {
        this.volume_min = volume_min;
        this.volume_max = volume_max;
        this.pitch_min = pitch_min;
        this.pitch_max = pitch_max;
    }

    public float GetVolume() => ( float ) GD.RandRange( volume_min, volume_max );
    public float GetPitch() => ( float ) GD.RandRange( pitch_min, pitch_max );
}