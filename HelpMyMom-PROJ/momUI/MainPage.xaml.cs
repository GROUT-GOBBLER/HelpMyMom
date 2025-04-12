using System;
using System.Net.Http.Json;
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

                    Account a = JsonConvert.DeserializeObject<Account>(json);


                    LoginButton.Text = $" id: {a.Username} ChildId: {a.ChildId}, MomId: {a.MomId}, HelperId {a.HelperId}";

                    
                }
                catch (Exception ex)
                {
                    LoginButton.Text = $" {ex}";
                }




            }

        }
    }

}
