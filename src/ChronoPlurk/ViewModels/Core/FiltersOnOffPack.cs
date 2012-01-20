using System;
using System.Collections.Generic;

namespace ChronoPlurk.ViewModels.Core
{
    public struct FiltersOnOffPack
    {
        private bool _all;
        public bool All
        {
            get { return _all; }
            set { _all = value; }
        }

        private bool _unread;
        public bool Unread
        {
            get { return _unread; }
            set { _unread = value; }
        }

        private bool _my;
        public bool My
        {
            get { return _my; }
            set { _my = value; }
        }

        private bool _private;
        public bool Private
        {
            get { return _private; }
            set { _private = value; }
        }

        private bool _responded;
        public bool Responded
        {
            get { return _responded; }
            set { _responded = value; }
        }

        private bool _liked;
        public bool Liked
        {
            get { return _liked; }
            set { _liked = value; }
        }

        public FiltersOnOffPack(bool all, bool unread, bool my, bool @private, bool responded, bool liked)
        {
            _all = all;
            _unread = unread;
            _my = my;
            _private = @private;
            _responded = responded;
            _liked = liked;
        }

        public static FiltersOnOffPack CreateAllTrue()
        {
            return new FiltersOnOffPack(true, true, true, true, true, true);
        }

        public IEnumerable<bool> IterateAll()
        {
            yield return All;
            yield return Unread;
            yield return My;
            yield return Private;
            yield return Responded;
            yield return Liked;
        }

        private static bool Equals(FiltersOnOffPack f1, FiltersOnOffPack f2)
        {
            return f1.All == f2.All &&
                   f1.Unread == f2.Unread &&
                   f1.My == f2.My &&
                   f1.Private == f2.Private &&
                   f1.Responded == f2.Responded &&
                   f1.Liked == f2.Liked;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(FiltersOnOffPack)) return false;
            return Equals((FiltersOnOffPack)obj);
        }

        public static bool operator ==(FiltersOnOffPack f1, FiltersOnOffPack f2)
        {
            return Equals(f1, f2);
        }

        public static bool operator !=(FiltersOnOffPack f1, FiltersOnOffPack f2)
        {
            return !(Equals(f1, f2));
        }

        public bool Equals(FiltersOnOffPack other)
        {
            return Equals(this, other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _all.GetHashCode();
                result = (result * 397) ^ _unread.GetHashCode();
                result = (result * 397) ^ _my.GetHashCode();
                result = (result * 397) ^ _private.GetHashCode();
                result = (result * 397) ^ _responded.GetHashCode();
                result = (result * 397) ^ _liked.GetHashCode();
                return result;
            }
        }
    }
}
