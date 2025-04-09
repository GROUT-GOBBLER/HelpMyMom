using System;
using System.Net.Http.Json;

using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        string URL = $"http://localhost:5124/api";
        Account account;
        Mother mother;
        Child child;
        Helper helper;

        public MainPage()
        {
            InitializeComponent();
        }

        async private void OnCounterClicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Accounts"}/{"GodHelpOurMothers"}");

                    string json = await response2.Content.ReadAsStringAsync();

                    Account a = JsonConvert.DeserializeObject<Account>(json);
                    

                    CounterBtn.Text = $" id: {a.Username} ChildId: {a.ChildId}, MomId: {a.MomId}, HelperId {a.HelperId}";
                    
                    account = a;
                }
                    catch (Exception ex)
                {
                    CounterBtn.Text = $" {ex}";
                }
                   



            }
        }

        async private void post_Clicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (account == null) {
                        post.Text = $" No account";
                        return;
                    }

                    if( account.Password != PasswordEntry.Text)
                    {
                        post.Text = $" invalid password";
                        return;
                    }
                    if (account.ChildId != null)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Children"}/{account.ChildId}");
                        if (!response2.IsSuccessStatusCode)
                        {
                            post.Text = $" {response2} ";
                            return;
                        }
                        string json = await response2.Content.ReadAsStringAsync();
                        child = JsonConvert.DeserializeObject<Child>(json);
                       
                        post.Text = $" child {child.FName} {child.LName} ";
                    }
                    else if (account.HelperId != null)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}/{account.HelperId}");
                        string json = await response2.Content.ReadAsStringAsync();

                        helper = JsonConvert.DeserializeObject<Helper>(json);
                        post.Text = $" helper {helper.FName} {helper.LName} ";
                    }
                    else if (account.MomId != null)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Mothers"}/{account.MomId}");
                        string json = await response2.Content.ReadAsStringAsync();
                       
                        mother = JsonConvert.DeserializeObject<Mother>(json);
                        post.Text = $" mother {mother.FName} {mother.LName} ";
                    }


                    post.Text = $"Some sort of error happened with the account creation";


 

                }
                catch (Exception ex)
                {
                    post.Text = $" {ex}";
                }




            }

        }
    }

}
