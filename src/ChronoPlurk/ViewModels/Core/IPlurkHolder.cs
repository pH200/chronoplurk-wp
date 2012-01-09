using System.Collections.Generic;

namespace ChronoPlurk.ViewModels.Core
{
    public interface IPlurkHolder
    {
        IEnumerable<int> PlurkIds { get; }
        void Favorite(int id);
        void Unfavorite(int id);
        void Mute(int id);
        void Unmute(int id);
        void SetAsRead(int id);
    }
}
