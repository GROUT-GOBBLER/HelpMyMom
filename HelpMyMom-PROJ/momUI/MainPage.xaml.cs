using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()

        {
            /*
            Title: 35
            Header: 25
            Normal: 15
            Buttons:
            Small: 20
            Med: 30
            Large: 35
             
             
             
             
             */
            Accessibility a = Accessibility.getAccessibilitySettings();
            AccessibiltySettings.FontSize = a.fontsize + 10;
            QuickLogin.FontSize = a.fontsize + 10;
            head.FontSize = a.fontsize + 20;
            subHead.FontSize = a.fontsize + 10;  
            UsernameEntry.FontSize = a.fontsize;
            PasswordEntry.FontSize = a.fontsize;
            LoginButton.FontSize = a.fontsize + 10;
            SigninButton.FontSize = a.fontsize + 10;
            if (AccessibiltySettings.FontSize > 25)
            {
                AccessibiltySettings.FontSize = 25;
                QuickLogin.FontSize =  25;

            }
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


                        LoginButton.Text = $" child {child.FName} {child.LName} ";
                        await Navigation.PushAsync(new ChildMenu(child));
                        LoginButton.IsEnabled = true;
                        return;
                    }
                    else if (account.HelperId != null)
                    { 
                        if (Application.Current != null)
                        {
                            Application.Current.MainPage = new NavigationPage(new HelperView(account));
                            LoginButton.IsEnabled = true;
                        }
                        
                        LoginButton.IsEnabled = true;
                        return;
                    }
                    else if (account.MomId != null)
                    {
                        response2 = await client.GetAsync($"{URL}/{"Mothers"}/{account.MomId}");
                        json = await response2.Content.ReadAsStringAsync();

                        Mother mother = JsonConvert.DeserializeObject<Mother>(json);

                        int id = (int)account.MomId;
                        await Navigation.PushAsync(new MomMenu(account.Username, id));

                        LoginButton.IsEnabled = true;


                        //  LoginButton.Text = $" mother {mother.FName} {mother.LName} ";
                        return;
                    }

                    ErrorLabel.Text = $"Some sort of error happened with the account creation";
                }
                catch (Exception ex)
                {
                   // LoginButton.Text = $" {ex}";
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

        async private void AccessibiltySettings_Clicked(object sender, EventArgs e)

        {
            AccessibiltySettings.IsEnabled = false;
            await Navigation.PushAsync(new Accessibility_Settings());

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
