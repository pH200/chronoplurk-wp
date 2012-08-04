using System.Collections.Generic;

namespace ChronoPlurk.ViewModels.Core
{
    public interface IPlurkHolder
    {
        IEnumerable<long> PlurkIds { get; }
        void Favorite(long plurkId);
        void Unfavorite(long plurkId);
        void Mute(long plurkId);
        void Unmute(long plurkId);
        void SetAsRead(long plurkId);
        void Replurk(long plurkId);
        void Unreplurk(long plurkId);
    }
}
