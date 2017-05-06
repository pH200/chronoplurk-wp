using System;
using System.Collections.Generic;
using System.Linq;

namespace Plurto.Entities
{
    public interface ITimeline
    {
        IDictionary<int, User> Users { get; set; }

        IList<Plurk> Plurks { get; set; }

        int GetUserIdFromPlurk(Plurk plurk);
    }

    public static class TimelineExtensions
    {
        public static IEnumerable<UserPlurk> ToUserPlurks(this ITimeline timeline)
        {
            if (timeline == null)
            {
                throw new ArgumentNullException("timeline");
            }
            if (timeline.Users == null || timeline.Plurks == null)
            {
                return null;
            }

            var list = from plurk in timeline.Plurks
                       let userId = timeline.GetUserIdFromPlurk(plurk)
                       where timeline.Users.ContainsKey(userId)
                       select new UserPlurk() {Plurk = plurk, User = timeline.Users[userId]};

            return list;
        }
    }
}
