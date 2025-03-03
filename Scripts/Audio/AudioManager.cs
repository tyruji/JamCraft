using Godot;
using System;
using System.Linq;

public class AudioManager : Spatial
{
    public static AudioManager Instance { get; private set; }

    private const int AUDIO3D_STREAM_COUNT = 16;
    private const int AUDIO_STREAM_COUNT = 16;

    private AudioStreamInfo[] _audioStreams = new AudioStreamInfo[ AUDIO_STREAM_COUNT ];
    private Audio3DStreamInfo[] _audio3DStreams = new Audio3DStreamInfo[ AUDIO3D_STREAM_COUNT ];

    public override void _Ready()
    {
        Instance = this;

            // Setup audio players
        for( int i = 0; i < AUDIO_STREAM_COUNT; ++i )
        {
            AudioStreamPlayer plyr = new AudioStreamPlayer();
            AddChild( plyr );
            plyr.Owner = this;
            _audioStreams[ i ].audioPlayer = plyr;
        }
        for( int i = 0; i < AUDIO3D_STREAM_COUNT; ++i )
        {
            AudioStreamPlayer3D plyr = new AudioStreamPlayer3D();
            AddChild( plyr );
            plyr.Owner = this;
            _audio3DStreams[ i ].audioPlayer = plyr;
        }
    }

    public void Play( AudioStream stream )
    {
        foreach( var stream_info in _audioStreams )
        {
            if( stream_info.IsBusy() ) continue;

            var plyr = stream_info.audioPlayer;
            plyr.Stream = stream;
            plyr.Play();
            break;
        }
    }

    public void Play( AudioStream stream, ISoundSettings soundSettings )
    {
        foreach( var stream_info in _audioStreams )
        {
            if( stream_info.IsBusy() ) continue;

            var plyr = stream_info.audioPlayer;
            plyr.Stream = stream;
            plyr.PitchScale = soundSettings.GetPitch();
            plyr.VolumeDb = soundSettings.GetVolume();
            plyr.Play();
            break;
        }
    }

    public void Play3D( AudioStream stream, Vector3 position )
    {
        foreach( var stream_info in _audio3DStreams )
        {
            if( stream_info.IsBusy() ) continue;

            var plyr = stream_info.audioPlayer;
            plyr.Stream = stream;
            plyr.Translation = position;
            plyr.Play();
            break;
        }
    }

    public void Play3D( AudioStream stream, Vector3 position, ISoundSettings soundSettings )
    {
        foreach( var stream_info in _audio3DStreams )
        {
            if( stream_info.IsBusy() ) continue;

            var plyr = stream_info.audioPlayer;
            plyr.Stream = stream;
            plyr.Translation = position;
            plyr.PitchScale = soundSettings.GetPitch();
            plyr.UnitDb = soundSettings.GetVolume();
            plyr.Play();
            break;
        }
    }


    private struct AudioStreamInfo
    {
        public AudioStreamPlayer audioPlayer;

        public bool IsBusy() => audioPlayer.Playing;
    }

    private struct Audio3DStreamInfo
    {
        public AudioStreamPlayer3D audioPlayer;

        public bool IsBusy() => audioPlayer.Playing;
    }
}



