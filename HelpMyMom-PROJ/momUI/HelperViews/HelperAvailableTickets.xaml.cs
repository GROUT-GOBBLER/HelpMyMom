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

    String motherName;
    String helperName;
    String childName;

    public HelperAvailableTickets(Account a, Helper h)
	{
		InitializeComponent();
		
        allTickets = new List<TicketView>();

        fontSize = Accessibility.getAccessibilitySettings();
        masterAccount = a;
        masterHelper = h;

        motherName = "";
        helperName = masterHelper.FName + " " + masterHelper.LName;
        childName = "";
    }

	private class TicketView
	{
		public int? ticketID { get; set; }
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

        String? description = ticketSelected.ticketDescription;

        bool answer = await DisplayAlert("Accept this ticket?", $"{motherName}\n{description}", "No", "Yes");

        if (!answer)
		{
            using (HttpClient client = new HttpClient())
			{
				try
				{
                    HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
						String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
						List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON); // ticketsList.

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

            SendEmailNotifications(tempTicket);
        }

        RefreshPage();
    }

    async private void SendEmailNotifications(Ticket t)
    {
        // Temporary user objects.
        Child tempChild = new Child();
        Mother tempMother = new Mother();

        // Get mom and child.
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get data from database.
                HttpResponseMessage childResponse = await client.GetAsync($"{URL}/{"Children"}");
                    String childJSON = await childResponse.Content.ReadAsStringAsync();
                    List<Child>? childrenList = JsonConvert.DeserializeObject<List<Child>>(childJSON); // childrenList.
                HttpResponseMessage motherResponse = await client.GetAsync($"{URL}/{"Mothers"}");
                    String motherJSON = await motherResponse.Content.ReadAsStringAsync();
                    List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(motherJSON); // mothersList.

                // Get child.
                if (childResponse.IsSuccessStatusCode)
                {
                    if (childrenList != null)
                    {
                        foreach (Child c in childrenList)
                        {
                            if (c.Id == t.ChildId)
                            {
                                tempChild = c;
                                childName = tempChild.FName + " " + tempChild.LName;
                                break;
                            }
                        }

                        if (childName == "") { await DisplayAlert("ChildNotFound", $"Error! Child with ID {t.ChildId} could not be found.", "Ok."); }
                    }
                    else { await DisplayAlert("ChildrenNotFound", "Error! Failed to find any children.", "Ok."); }
                }
                else { await DisplayAlert("DatabaseConnectionFailure", "Error! Could not connect to the database.", "Ok."); }

                // Get mom.
                if (motherResponse.IsSuccessStatusCode)
                {
                    if (mothersList != null)
                    {
                        foreach (Mother m in mothersList)
                        {
                            if (m.Id == t.MomId)
                            {
                                tempMother = m;
                                motherName = tempMother.FName + " " + tempMother.LName;
                                break;
                            }
                        }

                        if (motherName == "") { await DisplayAlert("MotherNotFound", $"Error! Mother with ID {t.MomId} could not be found.", "Ok."); }
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Exception", $"Exception occurred ... {e}", "Ok.");
            }
        }

        if(tempMother.Email != null)
        {
            EmailServices.SendNotifcation(tempMother.Email, motherName, "INPROGRESS", t);
        }
        else { await DisplayAlert("NoMotherEmail", "Error! Could not find Mother's email.", "Ok."); }

        if (masterHelper.Email != null)
        {
            EmailServices.SendNotifcation(masterHelper.Email, helperName, "INPROGRESS", t);
        }
        else { await DisplayAlert("NoHelperEmail", "Error! Could not find Helper's email.", "Ok."); }

        // Determining child account setup.
        string[]? notifSettings = null;
        if (tempChild.Notifs != null) { notifSettings = tempChild.Notifs.Split(","); }

        using (HttpClient client = new HttpClient())
        {
            if (notifSettings != null && notifSettings.Length == 5)
            {
                bool shouldSendChild = bool.Parse(notifSettings[3].ToLower());

                if (shouldSendChild)
                {
                    EmailServices.SendNotifcation("hmmprojectchild@hotmail.com", childName, "INPROGRESS", t);
                }
            }
            else //If there are no settings, assume "true"
            {
                EmailServices.SendNotifcation("hmmprojectchild@hotmail.com", childName, "INPROGRESS", t);
            }
        }  
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
                            tempTicketView.ticketID = t.Id;
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