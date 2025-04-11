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

        async private void OnCounterClicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Specs"}/{1}");
                    
                    string json = await response2.Content.ReadAsStringAsync();

                    Spec Specs = JsonConvert.DeserializeObject<Spec>(json);

                    CounterBtn.Text = $" id: {Specs.Id} String: {Specs.Name}";

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

                    Spec s = new Spec();
                    s.Id = 2;
                    s.Name = "Microsoft Word";
                    HttpResponseMessage response = await client.PostAsJsonAsync($"{URL}/{"Specs"}", s); 

                   

                    post.Text = $" success";

                }
                catch (Exception ex)
                {
                    post.Text = $" {ex}";
                }




            }

            EmailServices.SendNotifcation("hmmprojectmom@hotmail.com", "completed", 1);

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

        }
    }

}
