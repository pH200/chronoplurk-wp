using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChronoPlurk.ViewModels.Core;

namespace ChronoPlurk.Services
{
    public class PlurkHolderService
    {
        private readonly List<IPlurkHolder> _plurkHolders = new List<IPlurkHolder>();

        public IEnumerable<IPlurkHolder> Search(int id)
        {
            return from plurkHolder in _plurkHolders
                   from plurkId in plurkHolder.PlurkIds
                   where id == plurkId
                   select plurkHolder;
        }

        private void SearchAndAction(int id, Action<IPlurkHolder> action)
        {
            var holders = Search(id);
            foreach (var plurkHolder in holders)
            {
                action(plurkHolder);
            }
        }
        
        public void Favorite(int id)
        {
            SearchAndAction(id, holder => holder.Favorite(id));
        }

        public void Unfavorite(int id)
        {
            SearchAndAction(id, holder => holder.Unfavorite(id));
        }
        
        public void Mute(int id)
        {
            SearchAndAction(id, holder => holder.Mute(id));
        }
        
        public void Unmute(int id)
        {
            SearchAndAction(id, holder => holder.Unmute(id));
        }

        public void SetAsRead(int id)
        {
            SearchAndAction(id, holder => holder.SetAsRead(id));
        }


        public void Add(IPlurkHolder item)
        {
            _plurkHolders.Add(item);
        }

        public void Clear()
        {
            _plurkHolders.Clear();
        }

        public bool Contains(IPlurkHolder item)
        {
            return _plurkHolders.Contains(item);
        }

        public bool Remove(IPlurkHolder item)
        {
            return _plurkHolders.Remove(item);
        }

        public void RemoveAll(IPlurkHolder item)
        {
            bool remove;
            do
            {
                remove = _plurkHolders.Remove(item);
            } while (remove);
        }

        public int Count
        {
            get { return _plurkHolders.Count; }
        }

        //public void CopyTo(IPlurkHolder[] array, int arrayIndex)
        //{
        //    _plurkHolders.CopyTo(array, arrayIndex);
        //}

        //public bool IsReadOnly
        //{
        //    get { return _plurkHolders.IsReadOnly; }
        //}

        //public IEnumerator<IPlurkHolder> GetEnumerator()
        //{
        //    return _plurkHolders.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        //public int IndexOf(IPlurkHolder item)
        //{
        //    return _plurkHolders.IndexOf(item);
        //}

        //public void Insert(int index, IPlurkHolder item)
        //{
        //    _plurkHolders.Insert(index, item);
        //}

        //public void RemoveAt(int index)
        //{
        //    _plurkHolders.RemoveAt(index);
        //}

        //public IPlurkHolder this[int index]
        //{
        //    get { return _plurkHolders[index]; }
        //    set { _plurkHolders[index] = value; }
        //}
    }
}
