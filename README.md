# ChronoPlurk for Windows Phone

The Open Source [Plurk](https://plurk.com) App for Windows Phone 8.x and Windows 10 Mobile. This app is developed for Windows Phone 8 Silverlight platform.

See [ChronoPlurk](https://www.microsoft.com/store/apps/9nblggh0b1nw) on Microsoft Store.

## Prerequisites

You must regitered a valid [Plurk App](https://www.plurk.com/API) to replace the app key and app secret strings in this repo.

## Build

1. Install Visual Studio 2015 (Windows Phone 8 is unsupported for 2017+)
2. Replace app key and app secret in `src/ChronoPlurk/DefaultConfiguration.conf.cs` and `src/plurto/Plurto.Test/TestConfigStrings.cs`
3. Build / F5

## Contribution

We're looking for active maintainers for this project. Feel free to drop a note in GitHub issues.

## Status

We will maintain ChronoPlurk's existing features, which means we will respond to issues and pull requests. However, we don't have plan to invest new features for this app.

I believe that flaws like poor GIF animation performance or binding errors can only be solved by rewriting the whole app. UWP/HTML5 platform is the answer to that. Unfortunately, no promise can be made at this moment.
