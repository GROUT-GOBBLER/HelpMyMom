using momUI.HelperViews;
using momUI.models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace momUI;

public partial class HelperView : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    Account masterAccount;
    Helper masterHelper;

    public HelperView(Account a)
	{
		InitializeComponent();

        masterAccount = a;
        masterHelper = new Helper();
        
        populateHelper();
    }

    private async void populateHelper() // puts the valid helper object into masterHelper. 
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get list of all helpers.
                HttpResponseMessage helperResponse = await client.GetAsync($"{URL}/{"Helpers"}");
                String helperJson = await helperResponse.Content.ReadAsStringAsync();
                List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(helperJson); // helpersList.

                if (helpersList != null)
                {
                    bool found = false;
                    foreach (Helper h in helpersList)
                    {
                        if (h.Id == masterAccount.HelperId)
                        {
                            masterHelper = h;
                            found = true;
                            break;
                        }
                    }
                    if (!found) { await DisplayAlert("HelperNotFound", $"A helper with ID {masterAccount.HelperId} was not found.", "Ok."); }
                }
                else { await DisplayAlert("HelpersNotFound", "Error! Helpers not found.", "Ok."); }
            }
            catch (Exception e)
            {
                await DisplayAlert("ExceptionOccured", $"An exception occurred ... {e}", "Ok.");
            }
        }

        CurrentBalanceLabel.Text = $"Balance: {masterHelper.Tokens}";
    }
    
    async private void OpenChatsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperCurrentChats(masterAccount, masterHelper));
    }

    async private void OpenTicketsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperAvailableTickets(masterAccount, masterHelper));
    }

    async private void OpenProfileButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperOpenProfile(masterAccount, masterHelper));
    }

    private async void LogOutButton_Clicked(object sender, EventArgs e)
    {
        if(Application.Current != null)
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
        else { await DisplayAlert("NoCurrentPage", "Error! Current main page not set.", "Ok."); }
    }

    private async void SettingsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Accessibility_Settings());
    }
}