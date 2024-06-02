using Core.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridApp.Services
{
    public class LoginService : BaseService
    {
        // Simulate sending OTP to the mobile number
        public async Task<bool> SendOtpAsync(string mobileNumber, string countryCode)
        {
            // In a real application, send the OTP via SMS service
            if (!string.IsNullOrEmpty(mobileNumber) && !string.IsNullOrEmpty(countryCode))
            {
                var url = $"{ApiBaseURL}{APIs.SendOTP}";
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var response = await httpClient.PostAsync(url + "/?mobileNumber=" + mobileNumber + "&countryCode=" + countryCode, null);
                        if (response != null && response.IsSuccessStatusCode)
                        {
                            var mainRespionse = JsonConvert.DeserializeObject<MainResponse>(await response.Content.ReadAsStringAsync());
                            return mainRespionse.IsSuccess;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            // If we got this far, something failed, redisplay form
            return false;
        }

        // Simulate verifying the OTP
        public async Task<string> VerifyOtpAsync(string mobileNumber, string otp)
        {
            if (mobileNumber != null)
            {
                var loginViewModel = new LoginRegisterMobileViewModel
                {
                    MobileNumber = mobileNumber,
                    CountryCode = "91",
                    OTP = otp,
                    Role = "appuser"
                };

                var jsonData = JsonConvert.SerializeObject(loginViewModel);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var url = $"{ApiBaseURL}{APIs.MobileLogin}";
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        try
                        {
                            var response = await httpClient.PostAsync(url, content);
                            if (response != null)
                            {
                                return await LoginOnPostAsync(response);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            // If we got this far, something failed, redisplay form
            return null;
        }

        public void Logout()
        {

        }

        private async Task<string> LoginOnPostAsync(HttpResponseMessage response)
        {
            if (response != null && response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody != null)
                {
                    var mainResponse = JsonConvert.DeserializeObject<MainResponse>(responseBody);

                    if (mainResponse != null && mainResponse.IsSuccess)
                    {
                        if (mainResponse.Content != null)
                        {
                            var authenticationResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(mainResponse.Content.ToString());
                            if (authenticationResponse != null)
                            {
                                var handler = new JwtSecurityTokenHandler();
                                var jsontoken = handler.ReadToken(authenticationResponse.AccessToken) as JwtSecurityToken;
                                if (!string.IsNullOrWhiteSpace(authenticationResponse.AccessToken))
                                {
                                    string userID = jsontoken.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.NameId).Value;
                                    string name = jsontoken.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.UniqueName).Value;
                                    string userAvatar = jsontoken.Claims.FirstOrDefault(f => f.Type == "UserAvatar").Value;
                                    string role = jsontoken.Claims.FirstOrDefault(f => f.Type == "role").Value;
                                    string mobileNo = jsontoken.Claims.FirstOrDefault(f => f.Type == "MobileNumber").Value;
                                    string balanceTokens = jsontoken.Claims.FirstOrDefault(f => f.Type == "BalanceTokens").Value;
                                    string subscriptionEndDate = jsontoken.Claims.FirstOrDefault(f => f.Type == "SubscriptionEndDate").Value;
                                    string language = jsontoken.Claims.FirstOrDefault(f => f.Type == "Language").Value;
                                    string countryCode = jsontoken.Claims.FirstOrDefault(f => f.Type == "CountryCode").Value;
                                    string gender = jsontoken.Claims.FirstOrDefault(f => f.Type == "Gender").Value;
                                    string email = "";// UserName;

                                    var userDetail = new UserDetail
                                    {
                                        Email = email,
                                        Name = name,
                                        Role = role,
                                        AccessToken = authenticationResponse.AccessToken,
                                        RefreshToken = authenticationResponse.RefreshToken,
                                        UserAvatar = !string.IsNullOrWhiteSpace(userAvatar) ? $"{ApiBaseURL}/{userAvatar}" : "",
                                        UserID = userID,
                                        Tokens = Convert.ToDecimal(balanceTokens),
                                        Language = language,
                                        CountryCode = countryCode,
                                        Gender = gender
                                    };

                                    return JsonConvert.SerializeObject(userDetail);
                                }
                                else
                                {
                                    return "Invalid Token created";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (mainResponse != null && mainResponse.ErrorMessage != null)
                        {
                            return mainResponse.ErrorMessage;
                        }
                    }
                }
            }
            else
            {
                if (response != null && response.ReasonPhrase != null)
                {
                    return response.ReasonPhrase;
                }
            }
            return null;
        }
    }
}
