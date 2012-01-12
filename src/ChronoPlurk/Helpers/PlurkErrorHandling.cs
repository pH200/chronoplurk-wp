using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Plurto.Core;
using Plurto.Exceptions;

namespace ChronoPlurk.Helpers
{
    public static class PlurkErrorHandling
    {
        public static IObservable<TSource> PlurkException<TSource>
            (this IObservable<TSource> source, Action<PlurkError> onError = null, IProgressService progressService = null)
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
            var timeoutErrorMessage = AppResources.timeoutMessage.Replace("\\n", Environment.NewLine);
            return Observable.Start(() =>
            {
                switch (MessageBox.Show(timeoutErrorMessage, "Timeout", MessageBoxButton.OKCancel))
                {
                    case MessageBoxResult.OK:
                        return source.PlurkException(onError, progressService);
                }
                if (onError != null)
                {
                    onError(PlurkError.UnknownError);
                }
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
                errorMessage = AppResources.requestFailedMessage.Replace("\\n", Environment.NewLine);
            }
            else
            {
                errorMessage = AppResources.unhandledErrorMessage.Replace("\\n", Environment.NewLine);
#if DEBUG
                throw ex;
#endif
            }

            Execute.OnUIThread(() => 
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK);
                if (onError != null)
                {
                    onError(error);
                }
                if (error == PlurkError.RequiresLogin)
                {
                    var plurkService = IoC.Get<IPlurkService>();
                    var navigationService = IoC.Get<INavigationService>();
                    if (plurkService != null && navigationService != null)
                    {
                        plurkService.ClearUserCookie();
                        navigationService.GotoLoginPage();
                    }
                }
            });
        }

        private static string ConvertPlurkErrorMessage(PlurkError plurkError)
        {
            // TODO: Localizations.
            switch (plurkError)
            {
                case PlurkError.EmailInvalid:
                    return AppResources.errEmailInvalid;
                case PlurkError.UserAlreadyFound:
                    return AppResources.errUserAlreadyFound;
                case PlurkError.EmailAlreadyFound:
                    return AppResources.errEmailAlreadyFound;
                case PlurkError.PasswordTooSmall:
                    return AppResources.errPasswordTooSmall;
                case PlurkError.NicknameTooSmall:
                    return AppResources.errNicknameTooSmall;
                case PlurkError.NicknameCharError:
                    return AppResources.errNicknameCharError;
                case PlurkError.InternalServiceError:
                    return AppResources.errInternalServiceError;
                case PlurkError.InvalidLogin:
                    return AppResources.errInvalidLogin;
                case PlurkError.TooManyLogins:
                    return AppResources.errTooManyLogins;
                case PlurkError.InvalidPassword:
                    return AppResources.errInvalidPassword;
                case PlurkError.NameTooLong:
                    return AppResources.errNameTooLong;
                case PlurkError.RequiresLogin:
                    return AppResources.errRequiresLogin;
                case PlurkError.NotSupportedImage:
                    return AppResources.errNotSupportedImage;
                case PlurkError.InvalidUserId:
                    return AppResources.errInvalidUserId;
                case PlurkError.UserNotFound:
                    return AppResources.errUserNotFound;
                case PlurkError.PlurkOwnerNotFound:
                    return AppResources.errPlurkOwnerNotFound;
                case PlurkError.PlurkNotFound:
                    return AppResources.errPlurkNotFound;
                case PlurkError.NoPermissions:
                    return AppResources.errNoPermissions;
                case PlurkError.InvalidData:
                    return AppResources.errInvalidData;
                case PlurkError.MustBeFriends:
                    return AppResources.errMustBeFriends;
                case PlurkError.ContentIsEmpty:
                    return AppResources.errContentIsEmpty;
                case PlurkError.AntiFloodSameContent:
                    return AppResources.errAntiFloodSameContent;
                case PlurkError.AntiFloodTooManyNew:
                    return AppResources.errAntiFloodTooManyNew;
                case PlurkError.InvalidFile:
                    return AppResources.errInvalidFile;
                case PlurkError.UserCantBefriended:
                    return AppResources.errUserCantBefriended;
                case PlurkError.UserAlreadyBefriended:
                    return AppResources.errUserAlreadyBefriended;
            }
            return AppResources.errUnknown;
        }
    }
}
