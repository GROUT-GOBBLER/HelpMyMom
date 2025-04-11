using System;
using System.Net.Http.Json;

using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        //string URL = $"http://localhost:5124/api";
        string URL = $" https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
     
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
                    HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Accounts"}/{"ImFine"}");

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
                    //post.Text = $" {PasswordEntry.Text} ";
                    //return;

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
                        return;
                    }
                    else if (account.HelperId != null)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}/{account.HelperId}");
                        string json = await response2.Content.ReadAsStringAsync();

                        helper = JsonConvert.DeserializeObject<Helper>(json);
                        post.Text = $" helper {helper.FName} {helper.LName} ";
                        return;
                    }
                    else if (account.MomId != null)
                    {
                        HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Mothers"}/{account.MomId}");
                        string json = await response2.Content.ReadAsStringAsync();
                       
                        mother = JsonConvert.DeserializeObject<Mother>(json);
                        post.Text = $" mother {mother.FName} {mother.LName} ";
                        return;
                    }


                    post.Text = $"Some sort of error happened with the account creation";



                }
                catch (Exception ex)
                {
                    post.Text = $" {ex}";
                }




            }

        }

        async private void CreateAccount_Clicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                
                Child c = new Child();
                c.Id = 4;
                c.FName = "Bruce";
                c.LName = "Bankman";
                c.Email = "hmmprojectchild@hotmail.com";
                HttpResponseMessage response2 = await client.PostAsJsonAsync($"{URL}/{"Children"}",c);
                CreateAccount.Text = await response2.Content.ReadAsStringAsync();
                
                Account a = new Account();
                a.Password = null;
                a.Username = "IveSeenShit";
                a.ChildId = c.Id;
                a.HelperId = null;
                a.MomId = null;
                response2 = await client.PostAsJsonAsync($"{URL}/{"Accounts"}", a);
                CreateAccount.Text = await response2.Content.ReadAsStringAsync();

                Mother m = new Mother();
                m.Id = 2;
                m.FName = "Bailey";
                m.LName = "Bankman";
                m.Tokens = 0;
                m.Email = "hmmprojectmom@hotmail.com";
                //  m.Relationships = 
                //  m.Tickets =
                //  m.Reviews = 
                // m.Reports =
                response2 = await client.PostAsJsonAsync($"{URL}/{"Mothers"}", m);
                CreateAccount.Text = await response2.Content.ReadAsStringAsync();

                Account a1 = new Account();
                a1.Password = null;
                a1.Username = "IveTriedToEscapeMyPast";
                a1.ChildId = null;
                a1.HelperId = null;
                a1.MomId = m.Id;
                response2 = await client.PostAsJsonAsync($"{URL}/{"Accounts"}", a1);
                CreateAccount.Text = await response2.Content.ReadAsStringAsync();


                Relationship relationship = new Relationship();
                relationship.Id = 3;
                relationship.ChildId = c.Id;
                relationship.MomId = m.Id;
                relationship.Child = null;
                relationship.Mom = null;
                response2 = await client.PostAsJsonAsync($"{URL}/{"Relationships"}", relationship);
                CreateAccount.Text = await response2.Content.ReadAsStringAsync();
                

                Helper h = new Helper();
                h.Id = 1;
                h.FName = "Rob";
                h.LName = "Bankman";
                h.Tokens = 0;
                h.Dob = null;
                h.Email = "hmmprojecthelper@hotmail.com";
                h.Banned = 0;
                h.Pfp = null;
                h.Specs = "";
                h.Description = "";
                 response2 = await client.PostAsJsonAsync($"{URL}/{"Helpers"}",h);
                CreateAccount.Text = await response2.Content.ReadAsStringAsync();
                
                Account a2 = new Account();
                a.Password = null;
                a.Username = "DefinetlyNotPartOfTheMafia";
                a.ChildId = null;
                a.HelperId = h.Id;
                a.MomId = null;
                response2 = await client.PostAsJsonAsync($"{URL}/{"Accounts"}", a);
                CreateAccount.Text = await response2.Content.ReadAsStringAsync();






            }
        }

        async private void switchPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Page2());
        }
    }

}
