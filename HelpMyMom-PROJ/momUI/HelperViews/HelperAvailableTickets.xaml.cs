using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperAvailableTickets : ContentPage
{
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    List<TicketView>? allTickets;
	String MASTER_USERNAME = "UncleBensBiggestFan";

    public HelperAvailableTickets(Account a, Helper h)
	{
		InitializeComponent();
		allTickets = new List<TicketView>();
    }

	private class TicketView
	{
		public int? ticketID { get; set; }
		public String? momName { get; set; }
		public String? ticketDescription { get; set; }
	}

	protected override void OnAppearing() // determines what the page does when it opens.
	{
        RefreshPage();
    }

    async private void AvailableTicketsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
		TicketView ticketSelected = (TicketView) AvailableTicketsListView.SelectedItem;
		String? name = ticketSelected.momName;
		String? description = ticketSelected.ticketDescription;

        bool answer = await DisplayAlert("Accept this ticket?", $"{name}\n{description}", "No", "Yes");

        if (!answer)
		{
            using (HttpClient client = new HttpClient())
			{
				try
				{
                    HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
						String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
						List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON); // ticketsList.
					
					Ticket tempTicket = new Ticket();

					bool found = false;
					if(ticketsList != null)
					{
                        foreach (Ticket t in ticketsList)
                        {
                            if (t.Id == ticketSelected.ticketID)
                            {
                                tempTicket = t;
                                found = true;
                                break;
                            }
                        }
						if(!found) { await DisplayAlert("TicketNotFound", "Error! Ticket not found.", "Ok."); }
                    }
					else { await DisplayAlert("NoTicketsFound", "Error! Failed to find any tickets.", "Ok."); }

					tempTicket.Status = "INPROGRESS";
                    HttpResponseMessage ticketPutResponse = await client.PutAsJsonAsync($"{URL}/Tickets/{ticketSelected.ticketID}", tempTicket); // TICKET.

					if (!ticketPutResponse.IsSuccessStatusCode)
					{
                        await DisplayAlert("EditStatus", "Failed to edit the ticket status.", "Ok.");
                    }
                }
				catch(Exception except)
				{
					AvailableTicketsExceptionLabel.Text = $"Exception occurred ... {except}";

                }
            }
        }

        RefreshPage();
    }

	async private void RefreshPage()
	{
        allTickets = new List<TicketView>(); // refresh 'er.

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Access all needed database tables.
                HttpResponseMessage accountResponse = await client.GetAsync($"{URL}/{"Accounts"}");
                String accountJSON = await accountResponse.Content.ReadAsStringAsync();
                List<Account>? accountsList = JsonConvert.DeserializeObject<List<Account>>(accountJSON); // accountsList.
                HttpResponseMessage helperResponse = await client.GetAsync($"{URL}/{"Helpers"}");
                String helperJSON = await helperResponse.Content.ReadAsStringAsync();
                List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(helperJSON); // helpersList.
                HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
                String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
                List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON); // ticketsList.
                HttpResponseMessage motherResponse = await client.GetAsync($"{URL}/{"Mothers"}");
                String motherJSON = await motherResponse.Content.ReadAsStringAsync();
                List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(motherJSON); // mothersList.

                if (!(accountResponse.IsSuccessStatusCode && helperResponse.IsSuccessStatusCode
                    && ticketResponse.IsSuccessStatusCode && motherResponse.IsSuccessStatusCode))
                {
                    await DisplayAlert("DatabaseConnectionFailure", "Error! Failed to connect to the database.", "Ok.");
                    return;
                }

                // Create temporary variables.
                int? helperID = -1;
                Helper tempHelper = new Helper();

                // Find account and helper.
                if (accountsList != null)
                {
                    foreach (Account a in accountsList) // get Helper's ID.
                    {
                        if (a.Username == MASTER_USERNAME)
                        {
                            helperID = a.HelperId;
                            break;
                        }
                    }

                    if (helperID == -1)
                    {
                        await DisplayAlert("AccoutNotFound", "Error! Account not found.", "Ok.");
                        return;
                    }
                }
                else { await DisplayAlert("NoAccountsFound", "Error! Failed to find any accounts.", "Ok."); }

                if (helpersList != null) // get Helper object.
                {
                    foreach (Helper h in helpersList)
                    {
                        if (h.Id == helperID)
                        {
                            tempHelper = h;
                            break;
                        }
                    }
                }
                else { await DisplayAlert("NoHelpersFound", "Error! Failed to find any helpers.", "Ok."); }

                // Populate allTICKETS with values from all tickets from a given helper with status "ASSIGNED".
                if (ticketsList != null)
                {
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Status == "ASSIGNED" && t.HelperId == tempHelper.Id)
                        {
                            TicketView tempTicketView = new TicketView();
                            tempTicketView.ticketDescription = t.Description;
                            tempTicketView.ticketID = t.Id;

                            if (mothersList != null)
                            {
                                foreach (Mother m in mothersList)
                                {
                                    if (m.Id == t.MomId)
                                    {
                                        tempTicketView.momName = m.FName + " " + m.LName;
                                        break;
                                    }
                                }
                            }
                            else { await DisplayAlert("NoMothersFound", "Error! Failed to find any mothers.", "Ok."); }

                            if (allTickets != null)
                            {
                                allTickets.Add(tempTicketView);
                            }
                        }
                    }
                }
                else { await DisplayAlert("NoTicketsFound", "Error! Failed to find any tickets.", "Ok."); }
            }
            catch (Exception e)
            {
                AvailableTicketsExceptionLabel.Text = $"Exception occured ... {e}";
            }
        }

        AvailableTicketsListView.ItemsSource = allTickets;
    }
}