using System;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace ChronoPlurk.ViewModels.FriendsFans
{
    [NotifyForAll]
    public class PeopleItemViewModel : PropertyChangedBase
    {
        public int Id { get; set; }

        public Uri AvatarView { get; set; }

        public string Username { get; set; }

        public string NickName { get; set; }

        public BirthdayPrivacy BirthdayPrivacy { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [DependsOn("DateOfBirth", "BirthdayPrivacy")]
        public string Age
        {
            get
            {
                if (BirthdayPrivacy == BirthdayPrivacy.ShowAll && DateOfBirth.HasValue)
                {
                    var year = DateTime.Now.Year - DateOfBirth.Value.Year;
                    return string.Format(AppResources.userAgeFormat, year);
                }
                return null;
            }
        }

        public Gender Gender { get; set; }

        public string Location { get; set; }

        [DependsOn("Gender", "Location")]
        public string GenderAndLocation
        {
            get
            {
                var output = "";
                switch (Gender)
                {
                    case Gender.Male:
                        output += AppResources.genderMale;
                        break;
                    case Gender.Female:
                        output += AppResources.genderFemale;
                        break;
                }
                if (!string.IsNullOrWhiteSpace(Location))
                {
                    output += AppResources.userFrom + Location;
                }
                return output == "" ? null : output;
            }
        }

        public double Karma { get; set; }

        [DependsOn("Karma")]
        public string KarmaView
        {
            get
            {
                if ((int)Karma == 0)
                {
                    return null;
                }
                else
                {
                    return "karma: " + Karma;
                }
            }
        }
    }
}