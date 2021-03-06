﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChronoPlurk.ViewModels.Core;

namespace ChronoPlurk.Services
{
    public class PlurkHolderService
    {
        private readonly List<IPlurkHolder> _plurkHolders = new List<IPlurkHolder>();

        public IEnumerable<IPlurkHolder> Search(long plurkId)
        {
            return from plurkHolder in _plurkHolders
                   from holderPlurkId in plurkHolder.PlurkIds
                   where plurkId == holderPlurkId
                   select plurkHolder;
        }

        private void SearchAndAction(long plurkId, Action<IPlurkHolder> action)
        {
            var holders = Search(plurkId);
            foreach (var plurkHolder in holders)
            {
                action(plurkHolder);
            }
        }
        
        public void Favorite(long plurkId)
        {
            SearchAndAction(plurkId, holder => holder.Favorite(plurkId));
        }

        public void Unfavorite(long plurkId)
        {
            SearchAndAction(plurkId, holder => holder.Unfavorite(plurkId));
        }
        
        public void Mute(long plurkId)
        {
            SearchAndAction(plurkId, holder => holder.Mute(plurkId));
        }
        
        public void Unmute(long plurkId)
        {
            SearchAndAction(plurkId, holder => holder.Unmute(plurkId));
        }

        public void SetAsRead(long plurkId)
        {
            SearchAndAction(plurkId, holder => holder.SetAsRead(plurkId));
        }

        public void MarkAsRead(IEnumerable<long> plurkIds)
        {
            foreach (var plurkId in plurkIds)
            {
                SetAsRead(plurkId);
            }
        }
        
        public void Replurk(long plurkId)
        {
            SearchAndAction(plurkId, holder => holder.Replurk(plurkId));
        }

        public void Unreplurk(long plurkId)
        {
            SearchAndAction(plurkId, holder => holder.Unreplurk(plurkId));
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
    }
}
