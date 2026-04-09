using Windows.Media.Control;
using Crystals.Core.Utilities;

namespace Crystals.Core.Sources;

public class MusicSource : BaseGSMTCSource
{
    protected override async Task HandleNewMediaSession(GlobalSystemMediaTransportControlsSession? session)
    {
        await base.HandleNewMediaSession(session);

        if (session == null) return;

        if (await IsMusicSession(session))
        {
            FollowSession(session);
            InvokeRequestFocus();
        }
        else
        {
            Console.WriteLine("Not a music session");
            UnfollowCurrentSession();
        }
    }

    private async Task<bool> IsMusicSession(GlobalSystemMediaTransportControlsSession session)
    {
        var props = await session.TryGetMediaPropertiesAsync();
        if (props == null || string.IsNullOrEmpty(props.Title)) return false;

        return await ImageUtilities.IsThumbnailMusic(props.Thumbnail);
    }
}