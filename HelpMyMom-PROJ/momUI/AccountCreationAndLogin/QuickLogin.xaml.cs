using momUI.models;
using Newtonsoft.Json;


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
            try
            {
                HttpResponseMessage accountResponse = await client.GetAsync($"{URL}/{"Accounts"}");
                String accountJSON = await accountResponse.Content.ReadAsStringAsync();
                List<Account>? accountsList = JsonConvert.DeserializeObject<List<Account>>(accountJSON); // accountsList.

                HttpResponseMessage helperResponse = await client.GetAsync($"{URL}/{"Helpers"}");
                String helperJSON = await helperResponse.Content.ReadAsStringAsync();
                List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(helperJSON); // helpersList.

                if (accountResponse.IsSuccessStatusCode && helperResponse.IsSuccessStatusCode)
                {
                    Account tempAccount = new Account();
                    Helper tempHelper = new Helper();

                    if (accountsList != null) // find tempAccount.
                    {
                        bool found = false;
                        foreach (Account a in accountsList)
                        {
                            if (a.HelperId != null)
                            {
                                tempAccount = a;
                                found = true;
                                break;
                            }
                        }
                        if (!found) { await DisplayAlert("AccountNotFound", "Error! Account not found.", "Ok."); }
                    }
                    else { await DisplayAlert("AccountsNotFound", "Error! Failed to find any accounts.", "Ok."); }

                    if (helpersList != null) // find tempHelper.
                    {
                        bool found = false;
                        foreach (Helper h in helpersList)
                        {
                            if (h.Id == tempAccount.HelperId)
                            {
                                tempHelper = h;
                                found = true;
                                break;  
                            }
                        }
                        if (!found) { await DisplayAlert("HelpersNotFound", "Error! Helper not found.", "Ok."); }
                    }
                    else { await DisplayAlert("HelpersNotFound", "Error! Failed to find any helpers.", "Ok."); }

                    if (tempHelper != null && tempAccount != null)
                    {
                        if (Application.Current != null)
                        {
                            Application.Current.MainPage = new NavigationPage(new HelperView(tempAccount));
                        }
                        else { await DisplayAlert("NoCurrentMainpage", "Error! No current mainpage set.", "Ok."); }
                    }
                    else { await DisplayAlert("NoObjectsFound", "Error! tempHelper and tempAccount not populated.", "Ok."); }
                }
                else { await DisplayAlert("DatabaseConnectionFailure", "Error! Failed to access the database.", "Ok."); }
            }
            catch(Exception except)
            {
                await DisplayAlert("Exception", $"Exception occurred ... {except}", "Ok.");
                return;
            }
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
            await Navigation.PushAsync(new MomMenu());
        }

    }
}