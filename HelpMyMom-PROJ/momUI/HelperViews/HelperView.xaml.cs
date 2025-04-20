using momUI.HelperViews;
using momUI.models;
using Newtonsoft.Json;

namespace momUI;

public partial class HelperView : ContentPage
{
    String MASTER_ACCOUNT_USERNAME = "Cats2019";
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

    public HelperView()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing() // determines what the page does when it opens.
    {
        using(HttpClient client = new HttpClient())
        {
            try
            {
                // Get lists of Accounts and Helpers.
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}");
                    String json1 = await response1.Content.ReadAsStringAsync();
                    List<Account> accountsList = JsonConvert.DeserializeObject<List<Account>>(json1); // accountsList.
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
                    String json2 = await response2.Content.ReadAsStringAsync();
                    List<Helper> helpersList = JsonConvert.DeserializeObject<List<Helper>>(json2); // helpersList.

                // Define some temporary variables.
                Helper tempHelper = new Helper();
                int? helperID = -1;
                bool found = false;
                double? balance = 0.0;

                // Find account and helper.
                foreach(Account a in accountsList)
                {
                    if(a.Username == MASTER_ACCOUNT_USERNAME)
                    {
                        helperID = a.HelperId;
                        break;
                    }
                }

                if(helperID == -1)
                {
                    await DisplayAlert("AccountNotFound", "Error! Account not found.", "Ok.");
                    return;
                }

                foreach(Helper h in helpersList)
                {
                    if(h.Id == helperID)
                    {
                        tempHelper = h;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    await DisplayAlert("HelperNotFound", "Error! Helper not found.", "Ok.");
                    return;
                }

                // Return balance.
                    balance = tempHelper.Tokens;
                    CurrentBalanceLabel.Text = $"Current Balance: {balance}";
            }
            catch(Exception e)
            {
                CurrentBalanceLabel.Text = $"Error occured ... {e}";
            }
        }
    }
    
    async private void OpenChatsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperCurrentChats());
    }

    async private void OpenTicketsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperAvailableTickets());
    }

    async private void OpenProfileButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperOpenProfile());
    }
}