using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ChronoPlurk.Helpers;

namespace ChronoPlurk.Services
{
    public class RecentEmoticonsService
    {
        private const string IsoStoreFormat = "recent_emoticons_{0}.bin";

        /// <summary>
        /// Use dictionary for serialization.
        /// Can't serialize with List(Of KeyValuePair)
        /// http://sharpserializer.codeplex.com/workitem/16
        /// </summary>
        private Dictionary<string, string> Emoticons { get; set; }

        public int Maximum { get; set; }

        public IEnumerable<KeyValuePair<string, string>> List
        {
            get
            {
                if (Emoticons == null)
                {
                    return null;
                }
                else
                {
                    return Emoticons.AsEnumerable();
                }
            }
        }

        public RecentEmoticonsService()
        {
            Maximum = 10;
        }

        public void Replace(IEnumerable<KeyValuePair<string, string>> collection)
        {
            if (collection != null)
            {
                Emoticons = collection.Take(Maximum).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        public static void Insert(
            IList<KeyValuePair<string, string>> emoticonCollection,
            KeyValuePair<string, string> emoticon)
        {
            if (emoticonCollection.All(emo => emo.Key != emoticon.Key))
            {
                emoticonCollection.Insert(0, emoticon);
            }
        }

        public void Clear()
        {
            Emoticons.Clear();
        }


        /// <summary>
        /// Save emoticons list for specific user. Executes asynchronously.
        /// </summary>
        /// <param name="id"></param>
        public void Save(int id)
        {
            ThreadEx.OnThreadPool(() =>
            {
                IsoSettings.SerializeStore(Emoticons, string.Format(IsoStoreFormat, id));
            });
        }

        public void Load(int id)
        {
            var emoticons = IsoSettings.DeserializeLoad(string.Format(IsoStoreFormat, id))
                            as Dictionary<string, string>;
            if (emoticons != null)
            {
                Emoticons = emoticons;
            }
        }
    }
}
