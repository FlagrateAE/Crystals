using Crystals.Core.Models;
using Windows.Media.Control;

namespace Crystals.Core.Sources.Services;

public class GSMTCService
{
    public Action<Media> OnMediaChanged;

    private GlobalSystemMediaTransportControlsSessionManager? _manager;
    private GlobalSystemMediaTransportControlsSession? _currentSession;

    // Cache to prevent duplicate event invocations for the same media
    private string? _lastTitle;
    private string? _lastArtist;

    public async Task Start()
    {
        _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        HandleNewMediaSession(_manager.GetCurrentSession());

        _manager.CurrentSessionChanged += (sender, args) =>
        {
            var newSession = sender.GetCurrentSession();
            HandleNewMediaSession(newSession);
        };
    }

    private void HandleNewMediaSession(GlobalSystemMediaTransportControlsSession? session)
    {
        if (session == _currentSession) return;

        if (_currentSession != null)
        {
            _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
        }

        _currentSession = session;

        if (_currentSession != null)
        {
            _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
            
            UpdateMediaProperties(_currentSession);
        }
        else
        {
            // Console.WriteLine("No active media sessions found.");
        }
    }

    private async void UpdateMediaProperties(GlobalSystemMediaTransportControlsSession session)
    {
        try
        {
            var props = await session.TryGetMediaPropertiesAsync();
            if (
                props == null ||
                
                props.Title == null || props.Title.Length == 0 ||
                
                session.GetPlaybackInfo().PlaybackStatus ==
                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused ||
                
                props.Title == _lastTitle && props.Artist == _lastArtist

            ) return;
            
            _lastTitle = props.Title;
            _lastArtist = props.Artist;
            
            var media = new Media(session.SourceAppUserModelId, props.Title, props.Artist, props.Thumbnail);
            OnMediaChanged.Invoke(media);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving metadata: {ex.Message}");
        }
    }

    private void OnMediaPropertiesChanged(
        GlobalSystemMediaTransportControlsSession sender,
        MediaPropertiesChangedEventArgs? args
    )
    {
        UpdateMediaProperties(sender);
    }
}