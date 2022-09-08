﻿using System.Collections.ObjectModel;
using System.Text.Json;
using SteamAuthCore.Models;
using SteamAuthenticatorCore.MobileMaui.Extensions;
using SteamAuthenticatorCore.Shared.Abstractions;

namespace SteamMobileAuthenticator.Services;

internal class SecureStorageService : IAccountsFileService
{
    public SecureStorageService(ObservableCollection<SteamGuardAccount> accounts)
    {
        _accounts = accounts;
    }

    private readonly ObservableCollection<SteamGuardAccount> _accounts;
    private readonly List<string> _fileNames = new();
    private const string Key = "AccountsNames";
    private bool _isInitialized;

    public async ValueTask InitializeOrRefreshAccounts()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;
            _fileNames.AddRange(GetFileNames());
        }


        await MainThreadExtensions.InvokeOnMainThread(() =>
        {
            _accounts.Clear();
        });

        foreach (var accountsName in _fileNames)
        {
            if (await GetFromSecureStorage<SteamGuardAccount>(accountsName).ConfigureAwait(false) is not { } account)
                continue;

            await MainThreadExtensions.InvokeOnMainThread(() =>
            {
                _accounts.Add(account);
            });
        }
    }

    public async ValueTask<bool> SaveAccount(Stream stream, string fileName)
    {
        if (await JsonSerializer.DeserializeAsync<SteamGuardAccount>(stream).ConfigureAwait(false) is not { } account)
            return false;

        stream.Seek(0, SeekOrigin.Begin);

        using var streamReader = new StreamReader(stream);
        var json = await streamReader.ReadToEndAsync();
        await SecureStorage.SetAsync(account.AccountName, json).ConfigureAwait(false);

        _fileNames.Add(account.AccountName);
        Preferences.Set(Key, JsonSerializer.Serialize(_fileNames));

        await MainThreadExtensions.InvokeOnMainThread(() =>
        {
            _accounts.Add(account);
        });

        return true;
    }

    public async ValueTask SaveAccount(SteamGuardAccount account)
    {
        var json = JsonSerializer.Serialize(account);
        await SecureStorage.SetAsync(account.AccountName, json).ConfigureAwait(false);
    }

    public async ValueTask DeleteAccount(SteamGuardAccount accountToRemove)
    {
        _fileNames.Remove(accountToRemove.AccountName);
        Preferences.Set(Key, JsonSerializer.Serialize(_fileNames));

        SecureStorage.Remove(accountToRemove.AccountName);

        await MainThreadExtensions.InvokeOnMainThread(() =>
        {
            _accounts.Remove(accountToRemove);
        });
    }

    private static string[] GetFileNames()
    {
        if (Preferences.Get(Key, string.Empty) is not { } json)
            return Array.Empty<string>();

        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
        }
        catch (Exception)
        {
            return Array.Empty<string>();
        }
    }

    private static async ValueTask<T?> GetFromSecureStorage<T>(string key)
    {
        try
        {
            var json = await SecureStorage.GetAsync(key).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception)
        {
            return default;
        }
    }
}
