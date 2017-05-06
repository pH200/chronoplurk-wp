using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Plurto.Core;

namespace Plurto.Exceptions
{
    public class PlurkErrorException : Exception
    {
        public PlurkError Error { get; private set; }

        public string RawError { get; private set; }

        public ResponseData Response { get; private set; }

        public PlurkErrorException(ResponseData response)
            : this(response, response.Body)
        {
        }

        public PlurkErrorException(ResponseData response, string message)
            : base(message)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            if (response.Body == null)
            {
                throw new ArgumentNullException("response", "response.Body is null.");
            }
            Response = response;
            RawError = response.Body;
            
            const string pattern = @"{\s*""error_text""\s*:\s*""([^""]+)""\s*}";
            var match = Regex.Match(response.Body, pattern);
            if (match.Success && match.Groups.Count > 1)
            {
                var errorText = match.Groups[1].Value;
                Error = ErrorConverter(errorText);
            }
        }

        private static readonly Dictionary<string, PlurkError> ErrorDictionary = new Dictionary<string, PlurkError>()
        {
            {"Email invalid", PlurkError.EmailInvalid},
            {"User already found", PlurkError.UserAlreadyFound},
            {"Email already found", PlurkError.EmailAlreadyFound},
            {"Password too small", PlurkError.PasswordTooSmall},
            {"Nick name must be at least 3 characters long", PlurkError.NicknameTooSmall},
            {"Nick name can only contain letters, numbers and _", PlurkError.NicknameCharError},
            {"Internal service error. Please, try later", PlurkError.InternalServiceError},
            {"Invalid login", PlurkError.InvalidLogin},
            {"Too many logins", PlurkError.TooManyLogins},
            {"Invalid current password", PlurkError.InvalidPassword},
            {"Display name too long, should be less than 15 characters long", PlurkError.NameTooLong},
            {"Requires login", PlurkError.RequiresLogin},
            {"Not supported image format or image too big", PlurkError.NotSupportedImage},
            {"Invalid user_id", PlurkError.InvalidUserId},
            {"User not found", PlurkError.UserNotFound},
            {"Plurk owner not found", PlurkError.PlurkOwnerNotFound},
            {"Plurk not found", PlurkError.PlurkNotFound},
            {"No permissions", PlurkError.NoPermissions},
            {"Invalid data", PlurkError.InvalidData},
            {"Must be friends", PlurkError.MustBeFriends},
            {"Content is empty", PlurkError.ContentIsEmpty},
            {"anti-flood-same-content", PlurkError.AntiFloodSameContent},
            {"anti-flood-spam-domain", PlurkError.AntiFloodSpamDomain},
            {"anti-flood-too-many-new", PlurkError.AntiFloodTooManyNew},
            {"Invalid file", PlurkError.InvalidFile},
            {"User can't be befriended", PlurkError.UserCantBefriended},
            {"User already befriended", PlurkError.UserAlreadyBefriended},
            {"User must be befriended before you can follow them", PlurkError.UserMustBefriendedBeforeFollow},
            {"Clique not created", PlurkError.CliqueNotCreated},
            {"timed out", PlurkError.TimedOut}, // I got this.
            {"timed_out", PlurkError.TimedOut}, // Can't sure that there's a underscore or not.
            {"40002:unknown oauth request", PlurkError.UnknownOAuthRequest},
            {"40009:missing access token", PlurkError.MissingAccessToken},
            {"40106:invalid access token", PlurkError.InvalidAccessToken},
            {"40004:invalid timestamp or nonce", PlurkError.InvalidTimestampOrNonce}
        };

        private static PlurkError ErrorConverter(string value)
        {
            PlurkError plurkError;
            if (ErrorDictionary.TryGetValue(value, out plurkError))
            {
                return plurkError;
            }
            else
            {
                return PlurkError.UnknownError;
            }
        }
    }
}
