using System;
using System.Linq;
using System.Reflection;

namespace Plurto.Core
{
    public static partial class EnumExtensions
    {
        #region HttpVerb
        public static string ToKey(this HttpVerb httpVerb)
        {
            switch (httpVerb)
            {
                case HttpVerb.Get:
                    return "GET";
                case HttpVerb.Post:
                    return "POST";
            }
            return httpVerb.ToString().ToUpperInvariant();
        }
        #endregion

        #region OAuthSignatureMethod
        public static string ToKey(this OAuthSignatureMethod oAuthSignatureMethod)
        {
            switch (oAuthSignatureMethod)
            {
                case OAuthSignatureMethod.PLAINTEXT:
                    return "PLAINTEXT";
                case OAuthSignatureMethod.HMACSHA1:
                    return "HMAC-SHA1";
                case OAuthSignatureMethod.RSASHA1:
                    return "RSA-SHA1";
            }
            return oAuthSignatureMethod.ToString().ToUpperInvariant();
        }
        #endregion

        #region PostContentType
        public static string ToKey(this PostContentType postContentType)
        {
            switch (postContentType)
            {
                case PostContentType.UrlEncoded:
                    return "application/x-www-form-urlencoded";
                case PostContentType.Multipart:
                    return "multipart/form-data";
            }
            return postContentType.ToString().ToLowerInvariant();
        }
        #endregion

        #region UserPrivacy
        public static string ToKey(this UserPrivacy privacy)
        {
            switch (privacy)
            {
                case UserPrivacy.World: return "world";
                case UserPrivacy.OnlyFriends: return "only_friends";
                case UserPrivacy.OnlyMe: return "only_me";
            }
            return privacy.ToString().ToLowerInvariant();
        }
        #endregion
        
        #region Qualifier
        public static string ToKey(this Qualifier qualifier)
        {
            switch (qualifier)
            {
                case Qualifier.Loves: return "loves";
                case Qualifier.Likes: return "likes";
                case Qualifier.Shares: return "shares";
                case Qualifier.Gives: return "gives";
                case Qualifier.Hates: return "hates";
                case Qualifier.Wants: return "wants";
                case Qualifier.Has: return "has";
                case Qualifier.Will: return "will";
                case Qualifier.Asks: return "asks";
                case Qualifier.Wishes: return "wishes";
                case Qualifier.Was: return "was";
                case Qualifier.Feels: return "feels";
                case Qualifier.Thinks: return "thinks";
                case Qualifier.Says: return "says";
                case Qualifier.Is: return "is";
                case Qualifier.FreestyleColon: return ":";
                case Qualifier.Freestyle: return "freestyle";
                case Qualifier.Hopes: return "hopes";
                case Qualifier.Needs: return "needs";
                case Qualifier.Wonders: return "wonders";
            }
            return qualifier.ToString().ToLowerInvariant();
        } 
        #endregion

        #region PlurksFilter
        public static string ToKey(this PlurksFilter filter)
        {
            switch (filter)
            {
                case PlurksFilter.OnlyUser: return "only_user";
                case PlurksFilter.OnlyResponded: return "only_responded";
                case PlurksFilter.OnlyPrivate: return "only_private";
                case PlurksFilter.OnlyFavorite: return "only_favorite";
            }
            return null;
        } 
        #endregion

        #region UnreadFilter
        public static string ToKey(this UnreadFilter filter)
        {
            switch (filter)
            {
                case UnreadFilter.All:
                    return "all";
                case UnreadFilter.My:
                    return "my";
                case UnreadFilter.Responded:
                    return "responded";
                case UnreadFilter.Private:
                    return "private";
                case UnreadFilter.Favorite:
                    return "favorite";
            }
            return null;
        }
        #endregion

        #region PlurkTopSorting
        public static string ToKey(this PlurkTopSorting sorting)
        {
            switch (sorting)
            {
                case PlurkTopSorting.Hot:
                    return "hot";
                case PlurkTopSorting.New:
                    return "new";
            }
            return null;
        }
        #endregion
    }
}
