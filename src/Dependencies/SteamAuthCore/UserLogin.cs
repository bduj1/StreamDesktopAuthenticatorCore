﻿using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SteamAuthCore.Models;
using SteamAuthCore.Models.Internal;

namespace SteamAuthCore
{
    public class UserLogin
    {
        public UserLogin(string username, string password)
        {
            Username = username;
            Password = password;

            _cookies = new CookieContainer();
        }

        #region Fiedls

        public string Username { get; }
        public string Password { get; }
        public UInt64 SteamId { get; private set; }

        public bool RequiresCaptcha { get; private set; }
        public string? CaptchaGid { get; private set; }
        public string? CaptchaText { get; set; } = null;

        public bool RequiresEmail { get; private set; }
        public string? EmailDomain { get; } = null;
        public string? EmailCode { get; set; } = null;

        public bool Requires2Fa { get; private set; }

        public string? TwoFactorCode { get; set; } = null;

        public SessionData Session { get; private set; } = null!;
        public bool LoggedIn { get; private set; }

        #endregion

        #region Variables

        private readonly CookieContainer _cookies;

        #endregion

        public async Task<LoginResult> DoLogin()
        {
            /*NameValueCollection postData = new();
            CookieContainer cookies = _cookies;

            if (cookies.Count == 0)
            {
                //Generate a SessionID
                cookies.Add(new Cookie("mobileClientVersion", "0 (2.1.3)", "/", ".steamcommunity.com"));
                cookies.Add(new Cookie("mobileClient", "android", "/", ".steamcommunity.com"));
                cookies.Add(new Cookie("Steam_Language", "english", "/", ".steamcommunity.com"));

                var headers = new NameValueCollection {{"X-Requested-With", "com.valvesoftware.android.steam.community"}};

                await SteamApi.MobileLoginRequest("https://steamcommunity.com/login?oauth_client_id=DE45CD61&oauth_scope=read_profile%20write_profile%20read_client%20write_client", SteamApi.RequestMethod.Get, null, cookies, headers);
            }

            postData.Add("donotcache", (TimeAligner.GetSteamTime() * 1000).ToString());
            postData.Add("username", Username);

            if (await SteamApi.MobileLoginRequest<RsaResponse>(ApiEndpoints.CommunityBase + "/login/getrsakey", SteamApi.RequestMethod.Post, postData, cookies) is not {  } rsaResponse)
                return LoginResult.GeneralFailure;

            if (!rsaResponse.Success)
                return LoginResult.BadRsa;

            await Task.Delay(350); //Sleep for a bit to give Steam a chance to catch up??

            byte[] encryptedPasswordBytes;
            using (var rsaEncryptor = new RSACryptoServiceProvider())
            {
                var passwordBytes = Encoding.ASCII.GetBytes(this.Password);
                var rsaParameters = rsaEncryptor.ExportParameters(false);
                rsaParameters.Exponent = Util.HexStringToByteArray(rsaResponse.Exponent);
                rsaParameters.Modulus = Util.HexStringToByteArray(rsaResponse.Modulus);
                rsaEncryptor.ImportParameters(rsaParameters);
                encryptedPasswordBytes = rsaEncryptor.Encrypt(passwordBytes, false);
            }

            string encryptedPassword = Convert.ToBase64String(encryptedPasswordBytes);

            postData.Clear();

            postData.Add("donotcache", (TimeAligner.GetSteamTime() * 1000).ToString());

            postData.Add("password", encryptedPassword);
            postData.Add("username", Username);
            postData.Add("twofactorcode", TwoFactorCode ?? "");

            postData.Add("emailauth", RequiresEmail ? EmailCode : "");
            postData.Add("loginfriendlyname", "");
            postData.Add("captchagid", RequiresCaptcha ? CaptchaGid : "-1");
            postData.Add("captcha_text", RequiresCaptcha ? CaptchaText : "");
            postData.Add("emailsteamid", (Requires2Fa || RequiresEmail) ? SteamId.ToString() : "");

            postData.Add("rsatimestamp", rsaResponse.Timestamp);
            postData.Add("remember_login", "true");
            postData.Add("oauth_client_id", "DE45CD61");
            postData.Add("oauth_scope", "read_profile write_profile read_client write_client");

            if (await SteamApi.MobileLoginRequest<LoginResponse>(ApiEndpoints.CommunityBase + "/login/dologin", SteamApi.RequestMethod.Post, postData, cookies) is not { } loginResponse)
                return LoginResult.GeneralFailure;

            if (loginResponse.LoginComplete)
            {
                CookieCollection readableCookies = cookies.GetCookies(new Uri("https://steamcommunity.com"));
                var oAuthData = loginResponse.OAuthData;

                UInt64 id = UInt64.Parse(oAuthData.SteamId);

                Session = new SessionData(
                    readableCookies["sessionid"]!.Value,
                    oAuthData.SteamId + "%7C%7C" + oAuthData.SteamLogin,
                    oAuthData.SteamId + "%7C%7C" + oAuthData.SteamLogin,
                    oAuthData.Webcookie,
                    oAuthData.OAuthToken,
                    id);

                LoggedIn = true;
                return LoginResult.LoginOkay;
            }

            if (loginResponse.Message is null)
                return LoginResult.GeneralFailure;

            if (loginResponse.Message.Contains("There have been too many login failures"))
                return LoginResult.TooManyFailedLogins;

            if (loginResponse.Message.Contains("Incorrect login"))
                return LoginResult.BadCredentials;

            if (loginResponse.CaptchaNeeded)
            {
                RequiresCaptcha = true;
                CaptchaGid = loginResponse.CaptchaGid;
                return LoginResult.NeedCaptcha;
            }

            if (loginResponse.EmailAuthNeeded)
            {
                RequiresEmail = true;
                SteamId = loginResponse.EmailSteamID;
                return LoginResult.NeedEmail;
            }

            if (loginResponse.TwoFactorNeeded && !loginResponse.Success)
            {
                Requires2Fa = true;
                return LoginResult.Need2Fa;
            }

            if (loginResponse.OAuthData?.OAuthToken is null || loginResponse.OAuthData.OAuthToken.Length == 0)
                return LoginResult.GeneralFailure;

            if (!loginResponse.LoginComplete)
                return LoginResult.BadCredentials;*/

            return LoginResult.GeneralFailure;
        }
    }
}
