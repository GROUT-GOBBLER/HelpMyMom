using momUI.models;
using Newtonsoft.Json;
using System.Diagnostics;
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
        // code here.
    }
}