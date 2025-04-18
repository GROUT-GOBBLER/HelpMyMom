using System;
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
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Accounts"}/{UsernameEntry.Text}");

                    string json = await response2.Content.ReadAsStringAsync();

                    Account account = JsonConvert.DeserializeObject<Account>(json);


                    LoginButton.Text = $" id: {account.Username} ChildId: { account.ChildId}, MomId: {account.MomId}, HelperId {account.HelperId}";

                    //post.Text = $" {PasswordEntry.Text} ";
                    //return;

                    if (account == null)
                    {
                        LoginButton.Text = $" No account";
                        return;
                    }

                    if (account.Password != PasswordEntry.Text)
                    {
                        LoginButton.Text = $" invalid password";
                        return;
                    }
                    if (account.ChildId != null)
                    {
                        response2 = await client.GetAsync($"{URL}/{"Children"}/{account.ChildId}");
                        if (!response2.IsSuccessStatusCode)
                        {
                            LoginButton.Text = $" {response2} ";
                            return;
                        }
                        json = await response2.Content.ReadAsStringAsync();
                        Child child = JsonConvert.DeserializeObject<Child>(json);

                        LoginButton.Text = $" child {child.FName} {child.LName} ";
                        return;
                    }
                    else if (account.HelperId != null)
                    {
                        response2 = await client.GetAsync($"{URL}/{"Helpers"}/{account.HelperId}");
                        json = await response2.Content.ReadAsStringAsync();

                        Helper helper = JsonConvert.DeserializeObject<Helper>(json);
                        LoginButton.Text = $" helper {helper.FName} {helper.LName} ";
                        return;
                    }
                    else if (account.MomId != null)
                    {
                        response2 = await client.GetAsync($"{URL}/{"Mothers"}/{account.MomId}");
                        json = await response2.Content.ReadAsStringAsync();

                        Mother mother = JsonConvert.DeserializeObject<Mother>(json);
                        LoginButton.Text = $" mother {mother.FName} {mother.LName} ";
                        return;
                    }


                    LoginButton.Text = $"Some sort of error happened with the account creation";



                }
                catch (Exception ex)
                {
                    LoginButton.Text = $" {ex}";
                }





            }

        }

        async private void SigninButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        private void AccessibiltySettings_Clicked(object sender, EventArgs e)
        {
        }

        async private void QuickLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QuickLogin());
        }
    }
}
