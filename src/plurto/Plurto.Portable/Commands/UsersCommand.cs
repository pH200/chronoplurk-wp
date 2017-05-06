using System;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class UsersCommand
    {
        [LegacyCommand]
        [SecureRequest]
        public static CommandBase<User> Register(string nickName, string fullName, string password, Gender gender, DateTime dateOfBirth, string email=null)
        {
            var command = new CommandBase<User>(HttpVerb.Get, "/Users/register");
            if (nickName.Length <= 3 || !Regex.IsMatch(nickName, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException(
                    "Should be longer than 3 characters. Should be ASCII. Nick name can only contain letters, numbers and _.",
                    "nickName");
            }
            if (fullName.Length <= 0)
            {
                throw new ArgumentException("Can't be empty.", "fullName");
            }
            if (password.Length <= 3)
            {
                throw new ArgumentException("Should be longer than 3 characters.", "password");
            }
            command.AddParameter("nick_name", nickName);
            command.AddParameter("full_name", fullName);
            command.AddParameter("password", password);
            switch (gender)
            {
                case Gender.Female:
                    command.AddParameter("gender", "female");
                    break;
                case Gender.Male:
                    command.AddParameter("gender", "male");
                    break;
                case Gender.Other:
                    command.AddParameter("gender", "other");
                    break;
            }
            command.AddParameter("date_of_birth", dateOfBirth.ToUniversalTime().ToString("yyyy-MM-dd"));
            command.AddParameter("email", email);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [LegacyCommand]
        [SecureRequest]
        public static CommandBase<LoginProfile> Login(string username, string password)
        {
            var command = new CommandBase<LoginProfile>(HttpVerb.Get, "/Users/login");
            command.AddParameter("username", username);
            command.AddParameter("password", password);

            command.Deserializer = response => new LoginProfile()
                                                   {
                                                       Username = username,
                                                       Password = password,
                                                       Cookies = response.Cookies,
                                                       Profile = JsonConvert.DeserializeObject<Profile>(response.Body),
                                                   };

            return command;
        }

        [LegacyCommand]
        [SecureRequest]
        public static CommandBase<CookieCollection> LoginNoData(string username, string password)
        {
            var command = new CommandBase<CookieCollection>(HttpVerb.Get, "/Users/login");
            command.AddParameter("username", username);
            command.AddParameter("password", password);
            command.AddParameter("no_data", 1);

            command.Deserializer = response => response.Cookies;

            return command;
        }

        [LegacyCommand]
        [RequireLogin]
        public static CommandBase<bool> Logout()
        {
            var command = new CommandBase<bool>(HttpVerb.Get, "/Users/logout");
            command.SetSuccessTextDeserializer();

            return command;
        }

        [Api2]
        [RequireAccessToken]
        public static CommandBase<User> CurrentUser()
        {
            var command = new CommandBase<User>(HttpVerb.Get, "/Users/currUser");
            command.SetDefaultJsonDeserializer();

            return command;
        }

        [SecureRequest]
        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<User> Update(string currentPassword,
            string fullname, string newPassword, string email, string displayName,
            UserPrivacy? privacy, DateTime dateOfBirth)
        {
            if (currentPassword == null)
            {
                throw new ArgumentNullException("currentPassword");
            }

            var command = new CommandBase<User>(HttpVerb.Get, "/Users/update");
            command.AddParameter("current_password", currentPassword);
            command.AddParameter("full_name", fullname);
            command.AddParameter("new_password", newPassword);
            command.AddParameter("email", email);
            command.AddParameter("display_name", displayName);
            if (privacy.HasValue)
            {
                command.AddParameter("privacy", privacy.Value.ToKey());
            }
            command.AddParameter("date_of_birth", dateOfBirth.ToUniversalTime().ToString("yyyy-MM-dd"));

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<User> UpdatePicture(UploadFile picture)
        {
            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }

            var command = new CommandBase<User>(HttpVerb.Post, "/Users/updatePicture");
            picture.FieldName = "profile_image";
            command.UploadFile = picture;

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<KarmaStats> GetKarmaStats()
        {
            var command = new CommandBase<KarmaStats>(HttpVerb.Get, "/Users/getKarmaStats");
            command.SetDefaultJsonDeserializer();

            return command;
        }
    }
}
