using Newtonsoft.Json;
using Plurto.Converters;

namespace Plurto.Core
{
    public enum LoggingLevel
    {
        None, Response, Verbose,
    }
    public enum HttpVerb
    {
        Get, Post
    }

    public enum PostContentType
    {
        UrlEncoded, Multipart
    }

    public enum OAuthSignatureMethod
    {
        PLAINTEXT,
        HMACSHA1,
        RSASHA1,
    }

    [JsonConverter(typeof(QualifierConverter))]
    public enum Qualifier
    {
        FreestyleColon,
        Freestyle,
        Loves,
        Likes,
        Shares,
        Gives,
        Hates,
        Wants,
        Wishes,
        Needs,
        Will,
        Hopes,
        Asks,
        Has,
        Was,
        Wonders,
        Feels,
        Thinks,
        Says,
        Is,
    }

    public enum UnreadStatus
    {
        Read=0, Unread=1, Muted=2
    }

    public enum CommentMode
    {
        None=0, Disabled=1, FriendsOnly=2
    }

    public enum PlurkType
    {
        Public=0, Private=1, PublicResponded=2, PrivateResponded=3,
    }

    public enum Gender
    {
        Female=0, Male=1, Other=2,
    }

    public enum BirthdayPrivacy
    {
        Hide=0, ShowDate=1, ShowAll=2,
    }

    [JsonConverter(typeof(UserPrivacyConverter))]
    public enum UserPrivacy
    {
        World, OnlyFriends, OnlyMe,
    }

    [JsonConverter(typeof(KarmaFallReasonConverter))]
    public enum KarmaFallReason
    {
        None, FriendsRejections, Inactivity, TooShortResponses,
    }

    public enum PlurksFilter
    {
        None, OnlyUser, OnlyResponded, OnlyPrivate, OnlyFavorite,
    }

    public enum UnreadFilter
    {
        All, My, Responded, Private, Favorite,
    }

    public enum PlurkTopSorting
    {
        Hot, New,
    }

    public enum PlurkError
    {
        UnknownError,
        EmailInvalid,
        UserAlreadyFound,
        EmailAlreadyFound,
        PasswordTooSmall,
        NicknameTooSmall,
        NicknameCharError,
        InternalServiceError,
        InvalidLogin,
        TooManyLogins,
        InvalidPassword,
        NameTooLong,
        RequiresLogin,
        NotSupportedImage,
        InvalidUserId,
        UserNotFound,
        PlurkOwnerNotFound,
        PlurkNotFound,
        NoPermissions,
        InvalidData,
        MustBeFriends,
        ContentIsEmpty,
        AntiFloodSameContent,
        AntiFloodSpamDomain,
        AntiFloodTooManyNew,
        InvalidFile,
        UserCantBefriended,
        UserAlreadyBefriended,
        UserMustBefriendedBeforeFollow,
        CliqueNotCreated,
        TimedOut, // {"error_text": "timed out"} 
        UnknownOAuthRequest, // {"error_text": "40002:unknown oauth request"}
        MissingAccessToken, //"40009:missing access token"
        InvalidAccessToken, // "40106:invalid access token"
        InvalidTimestampOrNonce, // "40004:invalid timestamp or nonce"
    }
}
