using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.Security;

namespace momUI.HelperViews;

public partial class HelperAvailableTickets : ContentPage
{
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    
    List<TicketView>? allTickets;
    Accessibility fontSize;

    Account masterAccount;
    Helper masterHelper;



    public HelperAvailableTickets(Account a, Helper h)
	{
		InitializeComponent();
		
        allTickets = new List<TicketView>();

        fontSize = Accessibility.getAccessibilitySettings();
        
        masterAccount = a;
        masterHelper = h;
    }

	private class TicketView
	{
		public Ticket? ticket { get; set; }
		public String? momName { get; set; }
		public String? ticketDescription { get; set; }
        public double MomNameFontSize { get; set; }
        public double TicketDescriptionFontSize { get; set; }
	}

	protected override void OnAppearing() // determines what the page does when it opens.
	{
        base.OnAppearing();
        RefreshPage();
        AvailableTicketsExceptionLabel.FontSize = fontSize.fontsize;
    }

    async private void AvailableTicketsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
		TicketView ticketSelected = (TicketView) AvailableTicketsListView.SelectedItem;
        Ticket tempTicket = new Ticket();

        if(ticketSelected.momName != null && ticketSelected.ticketDescription != null && ticketSelected.ticket != null)
        { 
            await Navigation.PushModalAsync(new AcceptDenyTicket(ticketSelected.ticket, ticketSelected.momName));
        }
        else 
        { 
            await DisplayAlert("TicketSelectionFailure", "Error! Could not find ticket information.", "Ok.");
            return;
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
                HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
                    String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON); // ticketsList.
                HttpResponseMessage motherResponse = await client.GetAsync($"{URL}/{"Mothers"}");
                    String motherJSON = await motherResponse.Content.ReadAsStringAsync();
                    List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(motherJSON); // mothersList.


                // Populate allTICKETS with values from all tickets from a given helper with status "ASSIGNED".
                if (ticketsList != null)
                {
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Status == "ASSIGNED" && t.HelperId == masterHelper.Id)
                        {
                            TicketView tempTicketView = new TicketView();
                            tempTicketView.ticketDescription = t.Description;
                            tempTicketView.ticket = t;
                            // Set font sizes.
                                tempTicketView.TicketDescriptionFontSize = fontSize.fontsize;
                                tempTicketView.MomNameFontSize = fontSize.fontsize + 10;

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