using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class HelperOpenProfile : ContentPage
{
	public HelperOpenProfile()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing() // determines what the page does when it opens.
	{
        string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
		// Specs

		using (HttpClient client = new HttpClient())
		{
			try
			{
				// Get lists of Accounts, Helpers, and Specs.
				HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}");
					String json1 = await response1.Content.ReadAsStringAsync();
					List<Account> accountsList = JsonConvert.DeserializeObject<List<Account>>(json1); // accountsList.
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
					String json2 = await response2.Content.ReadAsStringAsync();
					List<Helper> helpersList = JsonConvert.DeserializeObject<List<Helper>>(json2); // helpersList.
                HttpResponseMessage response3 = await client.GetAsync($"{URL}/{"Specs"}");
					String json3 = await response3.Content.ReadAsStringAsync();
					List<Spec> specsList = JsonConvert.DeserializeObject<List<Spec>>(json3); // specsList.

                // Something else...

            }
			catch (Exception except)
			{
				ExceptionDisplayLabel.Text = $"Exception occured {except}";
            }
		}
    }
}