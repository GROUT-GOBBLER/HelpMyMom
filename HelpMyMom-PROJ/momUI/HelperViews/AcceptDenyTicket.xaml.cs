using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class AcceptDenyTicket : ContentPage
{
    // GLOBAL VARIABLES.
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String ticketDescription;
    String motherName;
    String helperName;
    String childName;

    Ticket masterTicket;
    Accessibility fontSizes;
    
    // CLASS DEFINITION.
    public AcceptDenyTicket(Ticket t, String motherFullName)
	{
        InitializeComponent();

        masterTicket = t;
        fontSizes = Accessibility.getAccessibilitySettings();

        if(t.Description != null) { ticketDescription = t.Description; }
        else { ticketDescription = "Could not find ticket description."; }

        motherName = "";
        helperName = "";
        childName = "";
        
        TicketDescriptionLabel.Text = ticketDescription;
        MomNameLabel.Text = motherFullName;

        AcceptTicketLabel.FontSize = fontSizes.fontsize + 20;
        MomNameLabel.FontSize = fontSizes.fontsize + 10;
        TicketDescriptionLabel.FontSize = fontSizes.fontsize;
        AcceptTicketButton.FontSize = fontSizes.fontsize + 5;
        DeclineTicketButton.FontSize = fontSizes.fontsize + 5;
    }

    // BUTTON PRESSES.
    async private void AcceptTicketButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                if (masterTicket != null)
                {
                    masterTicket.Status = "INPROGRESS";
                    HttpResponseMessage ticketPutResponse = await client.PutAsJsonAsync($"{URL}/Tickets/{masterTicket.Id}", masterTicket); // TICKET.

                    if (ticketPutResponse.IsSuccessStatusCode) { SendEmailNotifications(masterTicket, true); }
                    else { await DisplayAlert("EditStatus", "Error! Failed to edit the ticket status.", "Ok."); }
                }
                else { await DisplayAlert("TicketSelectionFailure", "Error! Failed to find the selected ticket.", "Ok."); }
            }
            catch (Exception except)
            {
                await DisplayAlert("Exception", $"Exception occurred ... {except}", "Ok");
            }
        }

        if (Navigation != null) { await Navigation.PopModalAsync(); }
        else { await DisplayAlert("NavigationError", "Error! Something happened when returning to the previous page.", "Ok."); }
    }

    async private void DeclineTicketButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                if (masterTicket != null)
                {
                    masterTicket.Status = "NEW";
                    masterTicket.HelperId = null;
                    masterTicket.Helper = null;
                    HttpResponseMessage ticketPutResponse = await client.PutAsJsonAsync($"{URL}/Tickets/{masterTicket.Id}", masterTicket); // TICKET.
                    SendEmailNotifications(masterTicket, false);
                    if (ticketPutResponse.IsSuccessStatusCode) { SendEmailNotifications(masterTicket, false); }
                    else { await DisplayAlert("EditStatus", "Error! Failed to edit the ticket status.", "Ok."); }
                }
                else { await DisplayAlert("TicketSelectionFailure", "Error! Failed to find the selected ticket.", "Ok."); }
            }
            catch (Exception except)
            {
                await DisplayAlert("Exception", $"Exception occurred ... {except}", "Ok");
            }
        }

        if (Navigation != null) { await Navigation.PopModalAsync(); }
        else { await DisplayAlert("NavigationError", "Error! Something happened when returning to the previous page.", "Ok."); }
    }

    async private void ClosePageButton_Clicked(object sender, EventArgs e)
    {
        if (Navigation != null) { await Navigation.PopModalAsync(); }
        else { await DisplayAlert("NavigationError", "Error! Something happened when returning to the previous page.", "Ok."); }
    }

    // METHODS.
    async private void SendEmailNotifications(Ticket t, bool isSubmitButton)
    {
        // Temporary user objects.
        Child tempChild = new Child();
        Mother tempMother = new Mother();
        Helper tempHelper = new Helper();

        // Get mom and child and helper.
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
                HttpResponseMessage helperResponse = await client.GetAsync($"{URL}/{"Helpers"}");
                    String helperJSON = await helperResponse.Content.ReadAsStringAsync();
                    List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(helperJSON); // helpersList.

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
                    else { await DisplayAlert("MothersNotFound", "Error! Failed to find any mothers.", "Ok."); }
                }
                else { await DisplayAlert("DatabaseConnectionFailure", "Error! Could not connect to the database.", "Ok."); }

                // Get helper.
                if (helperResponse.IsSuccessStatusCode)
                {
                    if (helpersList != null)
                    {
                        foreach (Helper h in helpersList)
                        {
                            if(h.Id == t.HelperId)
                            {
                                tempHelper = h;
                                helperName = h.FName + " " + h.LName;
                                break;
                            }
                        }

                        if (helperName == "") { await DisplayAlert("HelperNotFound", $"Error! Helper with ID {t.HelperId} could not be found.", "Ok."); }
                    }
                    else { await DisplayAlert("HelpersNotFound", "Error! Failed to find any helpers.", "Ok."); }
                }
                else { await DisplayAlert("DatabaseConnectionFailure", "Error! Could not connect to the database.", "Ok."); }
            }
            catch (Exception e)
            {
                await DisplayAlert("Exception", $"Exception occurred ... {e}", "Ok.");
            }
        }

        // Email.
        if (tempMother.Email != null)
        {
            if (isSubmitButton) { EmailServices.SendNotifcation(tempMother.Email, motherName, "INPROGRESS", t); }
        }
        else { await DisplayAlert("MotherEmailMissing", "Error! Could not find the mother's email.", "Ok."); }

        if (tempHelper.Email != null)
        {
            if (isSubmitButton) { EmailServices.SendNotifcation(tempHelper.Email, helperName, "INPROGRESS", t); }
        }
        else { await DisplayAlert("NoHelperEmail", "Error! Could not find Helper's email.", "Ok."); }

        // Determining child account setup.
        string[]? notifSettings = null;
        if (tempChild.Notifs != null) { notifSettings = tempChild.Notifs.Split(","); }
        
        if (notifSettings != null && notifSettings.Length == 5)
        {
            bool shouldSendChild = bool.Parse(notifSettings[3].ToLower());

            if (shouldSendChild)
            {
                if (tempChild.Email != null && tempMother.Email != null)
                {
                    if (isSubmitButton)
                    {
                        EmailServices.SendNotifcation(tempChild.Email, childName, "INPROGRESS", t);
                    }
                    else
                    {
                        EmailServices.SendDenyMessage(tempChild.Email, tempMother.Email, motherName, t);
                    }
                }
                else { await DisplayAlert("EmailsNotFound", "Error! Could not find required emails.", "Ok."); }
            }
        }
        else //If there are no settings, assume "true"
        {
            if (tempChild.Email != null && tempMother.Email != null)
            {
                if (isSubmitButton)
                {
                    EmailServices.SendNotifcation(tempChild.Email, childName, "INPROGRESS", t);
                }
                else
                {
                    EmailServices.SendDenyMessage(tempChild.Email, tempMother.Email, motherName, t);
                }
            }
            else { await DisplayAlert("EmailsNotFound", "Error! Could not find required emails.", "Ok."); }
        }
    }
}