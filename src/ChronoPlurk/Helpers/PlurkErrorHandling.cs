using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using Microsoft.Phone.Controls;
using Plurto.Core;
using Plurto.Exceptions;
using Action = System.Action;

namespace ChronoPlurk.Helpers
{
    public static class PlurkErrorHandling
    {
        public static IObservable<TSource> PlurkException<TSource>
            (this IObservable<TSource> source, Action<PlurkError> onError = null, IProgressService progressService = null)
        {
            return source.PlurkException(Application.Current.GetActivePage(), onError, progressService);
        }

        public static IObservable<TSource> PlurkException<TSource>(
            this IObservable<TSource> source,
            PhoneApplicationPage page,
            Action<PlurkError> onError = null,
            IProgressService progressService = null)
        {
            Action hideProgress = () =>
            {
                if (progressService != null)
                {
                    Execute.OnUIThread(() =>
                    {
                        var currentPage = Application.Current.GetActivePage();
                        if (currentPage == page)
                        {
                            progressService.Hide();
                        }
                    });
                }
            };
            return source
                .Catch<TSource, TimeoutException>(ex =>
                {
                    hideProgress();
                    return TimeoutHandling(source, page, onError, progressService);
                })
                .Retry(DefaultConfiguration.RetryCount)
                .Catch<TSource, Exception>(ex =>
                {
                    hideProgress();
                    return PlurkExceptionHandling<TSource>(ex, page, onError);
                });
        }

        private static IObservable<TSource> TimeoutHandling<TSource>(
            IObservable<TSource> source,
            PhoneApplicationPage page,
            Action<PlurkError> onError,
            IProgressService progressService)
        {
            var timeoutErrorMessage = AppResources.timeoutMessage.Replace("\\n", Environment.NewLine);
            return Observable.Start(() =>
            {
                if (page == Application.Current.GetActivePage())
                {
                    switch (MessageBox.Show(timeoutErrorMessage, "Timeout", MessageBoxButton.OKCancel))
                    {
                        case MessageBoxResult.OK:
                            return source.PlurkException(onError, progressService);
                    }
                }
                if (onError != null)
                {
                    onError(PlurkError.UnknownError);
                }
                return Observable.Empty<TSource>();
            }, DispatcherScheduler.Instance).Merge();
        }

        public static IObservable<TSource> PlurkExceptionHandling<TSource>(
            Exception ex,
            PhoneApplicationPage page,
            Action<PlurkError> onError)
        {
            string errorMessage;
            var error = PlurkError.UnknownError;
            if (ex is PlurkErrorException)
            {
                var plurkException = (PlurkErrorException)ex;
                error = plurkException.Error;
                if (error == PlurkError.UnknownError && plurkException.RawError.Contains("invalid access token"))
                {
                    error = PlurkError.RequiresLogin;
                }
                errorMessage = ConvertPlurkErrorMessage(error);
                if (error == PlurkError.UnknownError)
                {
                    errorMessage += Environment.NewLine + Environment.NewLine +
                                    plurkException.RawError;
                }
            }
            else if (ex is RequestFailException)
            {
                errorMessage = AppResources.requestFailedMessage.Replace("\\n", Environment.NewLine);
            }
            else if (ex is UnauthorizedException)
            {
                error = PlurkError.RequiresLogin;
                errorMessage = AppResources.errRequiresLogin;
            }
            else
            {
                errorMessage = AppResources.unhandledErrorMessage.Replace("\\n", Environment.NewLine);
                var jsonException = ex as PlurtoJsonSerializationException;
                if (jsonException != null)
                {
                    if (jsonException.Response != null && jsonException.Response.ResponseUri != null)
                    {
                        errorMessage += "\n\n" + jsonException.Response.ResponseUri;
                    }
                }
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return Observable.Throw<TSource>(ex);
#endif
            }

            Execute.OnUIThread(() => 
            {
                if (page == Application.Current.GetActivePage())
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
                            plurkService.FlushConnection();
                            navigationService.GotoLoginPage();
                        }
                    }
                }
            });
            return Observable.Empty<TSource>();
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
