using Crystals.Core.Models;
using Windows.Media.Control;

namespace Crystals.Core.Sources;

public abstract class BaseGSMTCSource : ISource
{
    public event Action<Color>? OnColorChanged;
    public event Action? RequestFocus;

    private GlobalSystemMediaTransportControlsSessionManager? _manager;
    private GlobalSystemMediaTransportControlsSession? _currentSession;

    private string? _lastTitle;
    private string? _lastArtist;

    public virtual async Task Start()
    {
        _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        await HandleNewMediaSession(_manager.GetCurrentSession());

        _manager.CurrentSessionChanged += OnSessionChanged;
    }

    protected virtual Task HandleNewMediaSession(GlobalSystemMediaTransportControlsSession? session)
    {
        if (session == null || session == _currentSession) return Task.CompletedTask;
        
        _currentSession?.MediaPropertiesChanged -= OnMediaPropertiesChanged;
        _currentSession?.PlaybackInfoChanged -= OnPlaybackInfoChanged;
        return Task.CompletedTask;
    }

    private async void OnSessionChanged(
        GlobalSystemMediaTransportControlsSessionManager sender,
        object? args
    )
    {
        var newSession = sender.GetCurrentSession();
        await HandleNewMediaSession(newSession);
    }

    protected virtual async void OnMediaPropertiesChanged(
        GlobalSystemMediaTransportControlsSession sender,
        MediaPropertiesChangedEventArgs? args
    )
    {
        var props = await sender.TryGetMediaPropertiesAsync();

        if (props == null || string.IsNullOrEmpty(props.Title))
            return;
        
        if (sender.GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused) 
            return;

        if (props.Title == _lastTitle && props.Artist == _lastArtist)
            return;

        _lastTitle = props.Title;
        _lastArtist = props.Artist;
        
        Console.WriteLine($"Now playing: {props.Title} by {props.Artist}");
    }

    protected virtual void OnPlaybackInfoChanged(
        GlobalSystemMediaTransportControlsSession sender,
        PlaybackInfoChangedEventArgs args
    )
    {
    }

    protected void FollowSession(GlobalSystemMediaTransportControlsSession session)
    {
        _currentSession = session;

        _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
        _currentSession.PlaybackInfoChanged += OnPlaybackInfoChanged;
        
        OnMediaPropertiesChanged(session, null);
    }

    protected void UnfollowCurrentSession()
    {
        _currentSession?.MediaPropertiesChanged -= OnMediaPropertiesChanged;
        _currentSession?.PlaybackInfoChanged -= OnPlaybackInfoChanged;
        _currentSession = null;
    }

    protected void InvokeRequestFocus() => RequestFocus?.Invoke();
}