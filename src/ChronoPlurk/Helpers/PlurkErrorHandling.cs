using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Services;
using Plurto.Core;
using Plurto.Exceptions;

namespace ChronoPlurk.Helpers
{
    public static class PlurkErrorHandling
    {
        public static IObservable<TSource> PlurkException<TSource>
            (this IObservable<TSource> source, Action<PlurkError> onError, IProgressService progressService = null)
        {
            return source.Catch<TSource, Exception>(ex =>
            {
                if (progressService != null)
                {
                    Execute.OnUIThread(progressService.Hide);
                }
                if (ex is TimeoutException)
                {
                    return TimeoutHandling(source, onError, progressService);
                }
                PlurkExceptionHandling(ex, onError);
                return Observable.Empty<TSource>();
            });
        }

        private static IObservable<TSource> TimeoutHandling<TSource>
            (IObservable<TSource> source, Action<PlurkError> onError, IProgressService progressService)
        {
            const string timeoutErrorMessage =
                    "Request timeout.\nPlurk service might be busy now.\nPress OK to retry.\nPress cancel to stop.";
            return Observable.Start(() =>
            {
                switch (MessageBox.Show(timeoutErrorMessage, "Timeout", MessageBoxButton.OKCancel))
                {
                    case MessageBoxResult.OK:
                        return source.PlurkException(onError, progressService);
                }
                onError(PlurkError.UnknownError);
                return Observable.Empty<TSource>();
            }, DispatcherScheduler.Instance).Merge();
        }

        public static void PlurkExceptionHandling(Exception ex, Action<PlurkError> onError)
        {
            string errorMessage;
            var error = PlurkError.UnknownError;
            if (ex is PlurkErrorException)
            {
                error = ((PlurkErrorException) ex).Error;
                errorMessage = ConvertPlurkErrorMessage(error);
            }
            else if (ex is RequestFailException)
            {
                errorMessage = "Request failed. Check your internet connection.";
            }
            else
            {
                errorMessage = "Oops. Unknown error on this application. Leave a note to us.";
#if DEBUG
                throw ex;
#endif
            }

            Execute.OnUIThread(() => 
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
                onError(error);
            });
        }

        private static string ConvertPlurkErrorMessage(PlurkError plurkError)
        {
            // TODO: Localizations.
            switch (plurkError)
            {
                case PlurkError.EmailInvalid: return "Email invalid";
                case PlurkError.UserAlreadyFound: return "User already found";
                case PlurkError.EmailAlreadyFound: return "Email already found";
                case PlurkError.PasswordTooSmall: return "Password too small";
                case PlurkError.NicknameTooSmall: return "Nick name must be at least 3 characters long";
                case PlurkError.NicknameCharError: return "Nick name can only contain letters, numbers and _";
                case PlurkError.InternalServiceError: return "Internal service error. Please, try later";
                case PlurkError.InvalidLogin: return "Invalid login";
                case PlurkError.TooManyLogins: return "Too many logins";
                case PlurkError.InvalidPassword: return "Invalid current password";
                case PlurkError.NameTooLong: return "Display name too long, should be less than 15 characters long";
                case PlurkError.RequiresLogin: return "Requires login";
                case PlurkError.NotSupportedImage: return "Not supported image format or image too big";
                case PlurkError.InvalidUserId: return "Invalid user_id";
                case PlurkError.UserNotFound: return "User not found";
                case PlurkError.PlurkOwnerNotFound: return "Plurk owner not found";
                case PlurkError.PlurkNotFound: return "Plurk not found";
                case PlurkError.NoPermissions: return "No permissions";
                case PlurkError.InvalidData: return "Invalid data";
                case PlurkError.MustBeFriends: return "Must be friends";
                case PlurkError.ContentIsEmpty: return "Content is empty";
                case PlurkError.AntiFloodSameContent: return "anti-flood-same-content";
                case PlurkError.AntiFloodTooManyNew: return "anti-flood-too-many-new";
                case PlurkError.InvalidFile: return "Invalid file";
                case PlurkError.UserCantBefriended: return "User can't be befriended";
                case PlurkError.UserAlreadyBefriended: return "User already befriended";
            }
            return "Unknown problems on Plurk service.";
        }
    }
}
