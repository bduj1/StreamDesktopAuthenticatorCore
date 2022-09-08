﻿using SteamAuthCore.Abstractions;
using SteamAuthCore.Models;
using SteamAuthenticatorCore.MobileMaui.Models;
using SteamAuthenticatorCore.Shared.Abstractions;

namespace SteamMobileAuthenticator.Services;

internal class ConfirmationViewModelFactory : IConfirmationViewModelFactory
{
    public ConfirmationViewModelFactory(IPlatformImplementations platformImplementations, ISteamGuardAccountService accountService)
    {
        _platformImplementations = platformImplementations;
        _accountService = accountService;
    }

    private readonly IPlatformImplementations _platformImplementations;
    private readonly ISteamGuardAccountService _accountService;

    public IConfirmationViewModel Create(SteamGuardAccount account, IEnumerable<ConfirmationModel> confirmationModels) => new ConfirmationViewModel(account, confirmationModels, _platformImplementations, _accountService);
}
