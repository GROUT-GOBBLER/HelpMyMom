using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperWriteReport : ContentPage
{
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String ticketSummaryText = "";
    String accountUsername = "ANDwhenYOUcloseYOUReyes";
    int ticketID = 0;

	public HelperWriteReport()
	{
		InitializeComponent();
    }

    async private void SubmitTicketSummaryButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get entries from ACCOUNT table.
                HttpResponseMessage accountResponse = await client.GetAsync($"{URL}/{"Accounts"}");
                    String accountJSON = await accountResponse.Content.ReadAsStringAsync();
                    List<Account>? accountsList = JsonConvert.DeserializeObject<List<Account>>(accountJSON);

                // Get entries from HELPER table.
                HttpResponseMessage helperResponse = await client.GetAsync($"{URL}/{"Helpers"}");
                    String helperJSON = await helperResponse.Content.ReadAsStringAsync();
                    List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(helperJSON);

                // Get entries from TICKET table.
                HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
                    String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON);

                // Temporary variable creation.
                Ticket tempTicket = new Ticket();
                Helper tempHelper = new Helper();
                int? helperID = -1;

                // Populate temporary variables.
                if (accountResponse.IsSuccessStatusCode && helperResponse.IsSuccessStatusCode && ticketResponse.IsSuccessStatusCode)
                {
                    if(accountsList != null)
                    {
                        foreach(Account a in accountsList) // get helper ID from account username.
                        {
                            if(a.Username == accountUsername)
                            {
                                helperID = a.HelperId;
                                break;
                            }
                        }
                        if(helperID == -1) { await DisplayAlert("AccountNotFound", $"Error! Failed to find account with username {accountUsername}", "Ok."); }
                    }
                    else { await DisplayAlert("AccountsNotFound", "Error! Failed to find any accounts.", "Ok."); }

                    if(helpersList != null) 
                    {
                        foreach(Helper h in helpersList) // get helper object from helper ID.
                        {
                            if(h.Id == helperID)
                            {
                                tempHelper = h;
                                break;
                            }
                        }
                        if(tempHelper.Id != helperID) { await DisplayAlert("HelperNotFound", $"Error! Failed to find helper with ID {helperID}", "Ok."); }
                    }
                    else { await DisplayAlert("HelpersNotFound", "Error! Failed to find any helpers.", "Ok."); }

                    if (ticketsList != null)
                    {
                        foreach (Ticket t in ticketsList) // get ticket object from ticket ID.
                        {
                            if (t.Id == ticketID)
                            {
                                tempTicket = t;
                                break;
                            }
                        }
                        if (tempTicket.Id != ticketID) { await DisplayAlert("TicketNotFound", $"Error! Could not find the ticket with ID {ticketID}.", "Ok."); }
                    }
                    else { await DisplayAlert("TicketsNotFound", "Error! Failed to find any tickets.", "Ok."); }

                    // Submit log form into DB.
                    tempTicket.LogForm = ticketSummaryText;

                    // increase balance by 19.99
                    tempHelper.Tokens += 19.99;

                    HttpResponseMessage editTicket = await client.PutAsJsonAsync($"{URL}/Tickets/{ticketID}", tempTicket);
                    HttpResponseMessage editHelper = await client.PutAsJsonAsync($"{URL}/Helpers/{helperID}", tempHelper);

                    if (editTicket.IsSuccessStatusCode && editHelper.IsSuccessStatusCode)
                    {
                        if (Application.Current != null)
                        {
                            Application.Current.MainPage = new NavigationPage(new HelperView()); // go back to main Helper view page.
                        }
                        else { await DisplayAlert("NoCurrentPage", "Error! There is no set current page.", "Ok."); }
                    }
                    else
                    { await DisplayAlert("PutFailure", $"Error! EditTicketStatus: {editTicket.StatusCode}.\nEditHelperStatus: {editHelper.StatusCode}.", "Ok."); }
                }
                else { await DisplayAlert("DBAccessFailure", "Error! Could not connect to the database.", "Ok."); }
            }
            catch(Exception except)
            {
                await DisplayAlert("Exception", $"An exception occurred ... {except}", "Ok.");
            }
        }

        ticketSummaryText = "";
    }

    private void HelperTicketSummaryEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        ticketSummaryText = e.NewTextValue;
    }
}