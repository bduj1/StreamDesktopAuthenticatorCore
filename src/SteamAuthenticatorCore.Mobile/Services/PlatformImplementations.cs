﻿using SteamAuthenticatorCore.Mobile.Abstractions;
using SteamAuthenticatorCore.Shared.Abstractions;
using SteamAuthenticatorCore.Shared.Models;

namespace SteamAuthenticatorCore.Mobile.Services;

internal class PlatformImplementations : IPlatformImplementations
{
    public PlatformImplementations(IStatusBar statusBar)
    {
        _statusBar = statusBar;
    }

    private readonly IStatusBar _statusBar;

    public object CreateImage(string imageSource)
    {
        return ImageSource.FromUri(new Uri(imageSource, UriKind.Absolute));
    }

    public async ValueTask InvokeMainThread(Action method)
    {
        if (MainThread.IsMainThread)
        {
            method.Invoke();
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(method);
    }

    public Task DisplayAlert(string message)
    {
        return Application.Current!.MainPage!.DisplayAlert("Alert", message, "Ok");
    }

    public void SetTheme(Theme theme)
    {
        if (Application.Current == null)
            return;

        Application.Current.UserAppTheme = theme switch
        {
            Theme.System => AppTheme.Unspecified,
            Theme.Light => AppTheme.Light,
            Theme.Dark => AppTheme.Dark,
            _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
        };

        _statusBar.SetStatusBarColorBasedOnAppTheme();
    }
}