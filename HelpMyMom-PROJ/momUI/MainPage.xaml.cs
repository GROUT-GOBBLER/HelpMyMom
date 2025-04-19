﻿using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        //string URL = $"http://localhost:5124/api";
        string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

        public MainPage()
        {
            InitializeComponent();
        }
       
        async private void LoginButton_Clicked(object sender, EventArgs e)
        {
            LoginButton.IsEnabled = false;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Accounts"}/{UsernameEntry.Text}");

                    string json = await response2.Content.ReadAsStringAsync();

                    Account account = JsonConvert.DeserializeObject<Account>(json);


                   // LoginButton.Text = $" id: {account.Username} ChildId: { account.ChildId}, MomId: {account.MomId}, HelperId {account.HelperId}";

                    //post.Text = $" {PasswordEntry.Text} ";
                    //return;

                    if (account == null)
                    {
                        ErrorLabel.Text = $" No account";
                        LoginButton.IsEnabled = true;
                        return;
                    }

                    if (account.Password != PasswordEntry.Text)
                    {
                        ErrorLabel.Text = $" invalid password";
                        LoginButton.IsEnabled = true;
                        return;
                    }
                    if (account.ChildId != null)
                    {
                        response2 = await client.GetAsync($"{URL}/{"Children"}/{account.ChildId}");
                        if (!response2.IsSuccessStatusCode)
                        {
                             ErrorLabel.Text = $" Something happed to the connection ";
                            LoginButton.IsEnabled = true;
                            return;
                        }
                        json = await response2.Content.ReadAsStringAsync();
                        Child child = JsonConvert.DeserializeObject<Child>(json);

                       // LoginButton.Text = $" child {child.FName} {child.LName} ";
                        await Navigation.PushAsync(new ChildMenu());
                        LoginButton.IsEnabled = true;
                        return;
                    }
                    else if (account.HelperId != null)
                    {
                        response2 = await client.GetAsync($"{URL}/{"Helpers"}/{account.HelperId}");
                        json = await response2.Content.ReadAsStringAsync();

                        Helper helper = JsonConvert.DeserializeObject<Helper>(json);
                        if (helper.Banned == 1.0)
                        {
                            LoginButton.Text = $"This account is banned";
                            return;
                        }
                        
                        await Navigation.PushAsync(new HelperView());
                        LoginButton.IsEnabled = true;
                        return;
                    }
                    else if (account.MomId != null)
                    {
                        response2 = await client.GetAsync($"{URL}/{"Mothers"}/{account.MomId}");
                        json = await response2.Content.ReadAsStringAsync();

                        Mother mother = JsonConvert.DeserializeObject<Mother>(json);

                        await Navigation.PushAsync(new MomMenu());
                        LoginButton.IsEnabled = true;
                        //  LoginButton.Text = $" mother {mother.FName} {mother.LName} ";
                        return;
                    }


                    ErrorLabel.Text = $"Some sort of error happened with the account creation";



                }
                catch (Exception ex)
                {
                    LoginButton.Text = $" {ex}";
                }





            }
            LoginButton.IsEnabled = true;
        }

        async private void SigninButton_Clicked(object sender, EventArgs e)
        {
            SigninButton.IsEnabled = false;
            await Navigation.PushAsync(new SignUpPage());
            SigninButton.IsEnabled = true;
        }

        private void AccessibiltySettings_Clicked(object sender, EventArgs e)
        {
            AccessibiltySettings.IsEnabled = false;


            AccessibiltySettings.IsEnabled = true;
        }

        async private void QuickLogin_Clicked(object sender, EventArgs e)
        {
            QuickLogin.IsEnabled = false;
            await Navigation.PushAsync(new QuickLogin());
            QuickLogin.IsEnabled = true;
        }
    }
}
