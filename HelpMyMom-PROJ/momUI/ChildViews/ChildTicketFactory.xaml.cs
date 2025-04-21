using Microsoft.Extensions.Logging.Abstractions;
using momUI.models;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace momUI;

public partial class ChildTicketFactory : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    List<Mother>? moms = new List<Mother>();
    List<string> momsString = new List<string>();
    string details = "";

    Child account;
	public ChildTicketFactory(Child acc)
	{
		InitializeComponent();
        account = acc;
	}

    protected override async void OnAppearing()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                List<Relationship>? relationships = new List<Relationship>();

                HttpResponseMessage momResponse = await client.GetAsync(URL + "/Mothers");
                HttpResponseMessage relationResponse = await client.GetAsync(URL + "/Relationships");

                if (momResponse.IsSuccessStatusCode && relationResponse.IsSuccessStatusCode)
                {
                    string json = await momResponse.Content.ReadAsStringAsync();
                    string json2 = await relationResponse.Content.ReadAsStringAsync();

                    moms = JsonConvert.DeserializeObject<List<Mother>>(json);
                    relationships = JsonConvert.DeserializeObject<List<Relationship>>(json2);

                    if (moms != null && relationships != null)
                    {
                        relationships.RemoveAll(r => account.Id != r.ChildId);

                        var relatedMomIds = relationships
                            .Where(r => r.MomId.HasValue)
                            .Select(r => r.MomId.Value)
                            .ToHashSet();

                        moms.RemoveAll(m => !relatedMomIds.Contains(m.Id));

                        if (moms.Count < 1 || moms == null)
                        {
                            throw new Exception("You have no mom :(");
                        }
                        else
                        {
                            foreach (Mother m in moms)
                            {
                                momsString.Add($"{m.Id} {m.FName} {m.LName}");
                            }

                            momSelect.ItemsSource = momsString;
                        }
                    }
                    else
                    {
                        throw new Exception("No moms or relationships");
                    }
                } 
            }
            catch (Exception ex)
            {
                settingBtn.IsEnabled = false;
                settingBtn.Text = ex.Message;

                Console.WriteLine("\n\n");
                Console.WriteLine(ex.ToString());
            }
        }
    }


    private void DetailsEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        details = DetailsEntry.Text;
    }

    private async void createClicked(object sender, EventArgs e)
    {
        Ticket newTicket = new Ticket();
        List<Ticket>? tickets = new List<Ticket>();

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage ticketResponse = await client.GetAsync(URL + "/Tickets");

                if (ticketResponse.IsSuccessStatusCode == true)
                {
                    string json = await ticketResponse.Content.ReadAsStringAsync();

                    tickets = JsonConvert.DeserializeObject<List<Ticket>>(json);

                    if (tickets != null)
                    {
                        int newID = 0;
                        foreach (Ticket t in tickets)
                        {
                            if (t.Id > newID) newID = t.Id;
                        }

                        newTicket.Id = newID + 1;
                    }
                    else newTicket.Id = 1;

                    if (momSelect.SelectedItem == null) throw new Exception("No mom selected");
                    else
                    {
                        string[] selectedMom = momSelect.SelectedItem.ToString().Split(" ");

                        Mother mom = moms.Single(m => m.Id == Int32.Parse(selectedMom[0]));
                        newTicket.MomId = mom.Id;

                        if (mom.Tokens < 19.99) throw new Exception("That mom does not have enough money");

                        newTicket.ChildId = account.Id;
                        newTicket.Status = "NEW";
                        newTicket.Description = details;

                        // NULL BULLSHIT
                        newTicket.HelperId = null;
                        newTicket.LogForm = null;
                        newTicket.ReviewId = null;
                        newTicket.Reports = [];
                        newTicket.ChatLogs = [];
                        newTicket.Helper = null;
                        newTicket.Review = null;
                        newTicket.Child = null;
                        newTicket.Mom = null;

                        mom.Relationships = [];
                        mom.Reports = [];
                        mom.Reviews = [];
                        mom.Tickets = [];

                        HttpResponseMessage response3 = await client.PostAsJsonAsync(URL + "/Tickets", newTicket);
                        if (response3.IsSuccessStatusCode)
                        {
                            double newToken = (double)mom.Tokens - 19.99;

                            mom.Tokens = Math.Round(newToken, 2);

                            Console.WriteLine("-----------------------------------------------------------------------------------");
                            Console.WriteLine(JsonConvert.SerializeObject(mom));
                            Console.WriteLine("-----------------------------------------------------------------------------------");

                            HttpResponseMessage response4 = await client.PutAsJsonAsync($"{URL}/Mothers/{mom.Id}", mom);
                            if (response4.IsSuccessStatusCode)
                            {
                                settingBtn.Text = "good";
                            }
                            else settingBtn.Text = "bad money request";
                        }
                        else settingBtn.Text = "bad request";
                    }

                    await Navigation.PushAsync(new AssignHelperPage(account, newTicket));
                }
            }
            catch (Exception ex)
            {
                settingBtn.Text = ex.Message;

                Console.WriteLine("\n\n");
                Console.WriteLine(ex.ToString());
            }
        }
    }

    private void NothingClicked(object sender, EventArgs e)
    {

    }
}