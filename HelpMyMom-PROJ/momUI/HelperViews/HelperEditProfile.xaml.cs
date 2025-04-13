using momUI.models;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperEditProfile : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String newUsername;

	public HelperEditProfile()
	{
		InitializeComponent();
	}

    private void UsernameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newUsername = e.NewTextValue;
    }

    async private void UsernameEditButton_Clicked(object sender, EventArgs e)
    {
        using(HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Helpers"}"); // HELPER.
                    String json1 = await response1.Content.ReadAsStringAsync();
                    List<Helper> listHelpers = JsonConvert.DeserializeObject<List<Helper>>(json1);

                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Accounts"}"); // HELPER.
                    String json2 = await response2.Content.ReadAsStringAsync();
                    List<Account> listAccounts = JsonConvert.DeserializeObject<List<Account>>(json2);

                if(response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    Account tempAccount = listAccounts.First();
                    String accountUsername = tempAccount.Username;
                    int? helperID = tempAccount.HelperId;

                    tempAccount.Username = newUsername; // Update username.

                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/{"Helpers"}/{"DefinetlyNotPartOfTheMafia"}", tempAccount);

                    if (response3.IsSuccessStatusCode)
                    {
                        UsernameEditButton.Text = "Success.";
                    }
                    else
                    {
                        UsernameEditButton.Text = "Not success.";
                    }
                }
            }
            catch (Exception except)
            {
                UsernameEditButton.Text = $"Exception Occured: {except}";
            }
        }
    }
}