using momUI.models;
using Newtonsoft.Json;
using System;


namespace momUI;

public partial class QuickLogin : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    public QuickLogin()
	{
		InitializeComponent();
	}

    async private void Helper_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}/{1}");

            string json = await response2.Content.ReadAsStringAsync();
            Helper helper = JsonConvert.DeserializeObject<Helper>(json);
            await Navigation.PushAsync(new HelperView());
        }

    }

    async private void Child_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Children"}/{1}");

            string json = await response2.Content.ReadAsStringAsync();
            Child child = JsonConvert.DeserializeObject<Child>(json);
            await Navigation.PushAsync(new ChildMenu());
        }

    }

    async private void Mom_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Mothers"}/{1}");

            string json = await response2.Content.ReadAsStringAsync();
            Mother mom = JsonConvert.DeserializeObject<Mother>(json);
            await Navigation.PushAsync(new MomMenu("LoveMyRan", 0));
        }

    }
}