using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperWriteReport : ContentPage
{
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    
    String ticketSummaryText = "";

    int? ticketID;
    Helper masterHelper;
    Account masterAccount;

	public HelperWriteReport(int? t_id, Helper h, Account a)
	{
		InitializeComponent();
        ticketID = t_id;
        masterHelper = h;
        masterAccount = a;
    }

    async private void SubmitTicketSummaryButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get entries from TICKET table.
                HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
                    String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON);

                // Temporary variable creation.
                Ticket tempTicket = new Ticket();

                // Populate tempTicket.
                if (ticketResponse.IsSuccessStatusCode)
                {
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

                    // increase balance by $19.99
                    masterHelper.Tokens += 19.99;

                    HttpResponseMessage editTicket = await client.PutAsJsonAsync($"{URL}/Tickets/{ticketID}", tempTicket);
                    HttpResponseMessage editHelper = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);

                    if (editTicket.IsSuccessStatusCode && editHelper.IsSuccessStatusCode)
                    {
                        if (Application.Current != null)
                        {
                            Application.Current.MainPage = new NavigationPage(new HelperView(masterAccount)); // go back to main Helper view page.
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