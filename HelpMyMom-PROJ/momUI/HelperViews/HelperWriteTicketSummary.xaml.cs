using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperWriteReport : ContentPage
{
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String ticketSummaryText = "";
    int? ticketID;

    Accessibility fontSizes;
    Helper masterHelper;
    Account masterAccount;

    String helperName;
    String motherName;
    String childName;

	public HelperWriteReport(int? t_id, Helper h, Account a)
	{
		InitializeComponent();
        ticketID = t_id;

        fontSizes = Accessibility.getAccessibilitySettings();
        masterHelper = h;
        masterAccount = a;

        helperName = masterHelper.FName + " " + masterHelper.LName;
        motherName = "";
        childName = "";
    }

    protected override void OnAppearing()
    {
        SetFontSizes();
    }

    private void SetFontSizes()
    {
        TicketSummaryLabel.FontSize = fontSizes.fontsize + 20;
        TicketDescriptionLabel.FontSize = fontSizes.fontsize;
        HelperTicketSummaryEditor.FontSize = fontSizes.fontsize;
        SubmitTicketSummaryButton.FontSize = fontSizes.fontsize + 15;
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
                    if(masterHelper.Tokens != null)
                    {
                        masterHelper.Tokens = Double.Round((double)(masterHelper.Tokens + 19.99), 2); ;
                    }
                    else { await DisplayAlert("NoTokensFound", "Error! Helper doesn't have tokens.", "Ok."); }

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

                SendEmailNotification(tempTicket);
            }
            catch(Exception except)
            {
                await DisplayAlert("Exception", $"An exception occurred ... {except}", "Ok.");
            }
        }

        ticketSummaryText = "";
    }

    async private void SendEmailNotification(Ticket t)
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

        if (tempMother.Email != null)
        {
            EmailServices.SendNotifcation(tempMother.Email, motherName, "APPROVED", t);
        }
        else { await DisplayAlert("NoMotherEmail", "Error! Could not find Mother's email.", "Ok."); }

        if (masterHelper.Email != null)
        {
            EmailServices.SendNotifcation(masterHelper.Email, helperName, "APPROVED", t);
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
                    EmailServices.SendNotifcation("hmmprojectchild@hotmail.com", childName, "APPROVED", t);
                }
            }
            else //If there are no settings, assume "true"
            {
                EmailServices.SendNotifcation("hmmprojectchild@hotmail.com", childName, "APPROVED", t);
            }
        }
    }

    private void HelperTicketSummaryEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        ticketSummaryText = e.NewTextValue;
    }
}