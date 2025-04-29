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
    Child? c;
    Mother? m;
    Helper? h;

    Ticket masterTicket;
    Accessibility fontSizes;

    // CLASS DEFINITION.
    public AcceptDenyTicket(Ticket t, String motherFullName)
    {
        InitializeComponent();

        masterTicket = t;

        fontSizes = Accessibility.getAccessibilitySettings();

        if (t.Description != null) { ticketDescription = t.Description; }
        else { ticketDescription = "Could not find ticket description."; }

        getRelatedPpl();

        TicketDescriptionLabel.Text = ticketDescription;
        MomNameLabel.Text = motherFullName;

        AcceptTicketLabel.FontSize = fontSizes.fontsize + 20;
        MomNameLabel.FontSize = fontSizes.fontsize + 10;
        TicketDescriptionLabel.FontSize = fontSizes.fontsize;
        AcceptTicketButton.FontSize = fontSizes.fontsize + 5;
        DeclineTicketButton.FontSize = fontSizes.fontsize + 5;
    }

    // get child + mom
    async private void getRelatedPpl()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage momResponse = await client.GetAsync(URL + "/Mothers/" + masterTicket.MomId);
                HttpResponseMessage childResponse = await client.GetAsync(URL + "/Children/" + masterTicket.ChildId);
                HttpResponseMessage helperResponse = await client.GetAsync(URL + "/Helpers/" + masterTicket.HelperId);

                if (momResponse.IsSuccessStatusCode && childResponse.IsSuccessStatusCode)
                {
                    string json1 = await momResponse.Content.ReadAsStringAsync();
                    m = JsonConvert.DeserializeObject<Mother>(json1);

                    string json2 = await childResponse.Content.ReadAsStringAsync();
                    c = JsonConvert.DeserializeObject<Child>(json2);

                    string json3 = await helperResponse.Content.ReadAsStringAsync();
                    h = JsonConvert.DeserializeObject<Helper>(json3);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
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
        if (isSubmitButton)
        {
            Console.WriteLine("\n========================================================");
            Console.WriteLine("yes");
            Console.WriteLine("========================================================\n");

            string[]? notifSettings = null;
            if (c != null && c.Notifs != null) notifSettings = c.Notifs.Split(",");

            EmailServices.SendNotifcation(h.Email, $"{h.FName} {h.LName}", "INPROGRESS", masterTicket);
            EmailServices.SendNotifcation(m.Email, $"{m.FName} {m.LName}", "INPROGRESS", masterTicket);

            if (notifSettings != null && notifSettings.Length == 5)
            {
                bool shouldSendChild = bool.Parse(notifSettings[1].ToLower());

                if (shouldSendChild) EmailServices.SendNotifcation(c.Email, $"{c.FName} {c.LName}", "INPROGRESS", masterTicket);
            }
            else //If there are no settings, assume "true"
            {
                EmailServices.SendNotifcation(c.Email, $"{c.FName} {c.LName}", "INPROGRESS", masterTicket);
            }
        }
        else
        {
            Console.WriteLine("\n========================================================");
            Console.WriteLine("no");
            Console.WriteLine("========================================================/n");

            EmailServices.SendDenyMessage(c.Email, m.Email, $"{c.FName} {c.LName}", masterTicket);
        }
    }
}