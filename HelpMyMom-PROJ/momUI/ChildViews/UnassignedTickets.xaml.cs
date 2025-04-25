namespace momUI;

using models;
using Newtonsoft.Json;
using System.Net.Http.Json;

public partial class UnassignedTickets : ContentPage
{
    List<Ticket>? tickets = new List<Ticket>();
    List<SearchTicket> childTickets = new List<SearchTicket>();
    Child account;
	Helper helper;

    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

    int titleFont = 35;
    int headerFont = 25;
    int normalFont = 18;

    public UnassignedTickets(Child acc, Helper h)
	{
		InitializeComponent();

		account = acc;
		helper = h;
	}

	protected override async void OnAppearing()
	{
		using (HttpClient client = new HttpClient())
		{
            try
            {
                ticketList.IsRefreshing = true;
                HttpResponseMessage ticketResponse = await client.GetAsync(URL + "/Tickets");

                if (ticketResponse.IsSuccessStatusCode)
                {
                    string json = await ticketResponse.Content.ReadAsStringAsync();
                    tickets = JsonConvert.DeserializeObject<List<Ticket>>(json);

                    if (tickets != null)
                    {
                        tickets.RemoveAll(t => t.HelperId != null);
                        tickets.RemoveAll(t => t.ChildId != account.Id);

                        if (tickets.Count > 0)
                        {
                            foreach (Ticket t in tickets)
                            {
                                SearchTicket st = new SearchTicket();

                                st.Id = t.Id;
                                st.Details = t.Description;
                                st.Status = t.Status;

                                HttpResponseMessage momResponse = await client.GetAsync(URL + "/Mothers/" + t.MomId);
                                if (momResponse.IsSuccessStatusCode)
                                {
                                    string json2 = await momResponse.Content.ReadAsStringAsync();
                                    Mother? m = JsonConvert.DeserializeObject<Mother>(json2);

                                    if (m != null) st.MomName = $"{m.FName} {m.LName}";
                                    else st.MomName = "None";
                                }
                                else st.MomName = "None";

                                if (t.HelperId != null)
                                {
                                    HttpResponseMessage helperResponse = await client.GetAsync(URL + "/Helpers/" + t.HelperId);
                                    if (momResponse.IsSuccessStatusCode)
                                    {
                                        string json3 = await helperResponse.Content.ReadAsStringAsync();
                                        Helper? h = JsonConvert.DeserializeObject<Helper>(json3);

                                        if (h != null) st.HelperName = $"{h.FName} {h.LName}";
                                        else st.HelperName = "None";
                                    }
                                }
                                else st.HelperName = "None";

                                childTickets.Add(st);
                            }
                        }
                        else
                        {
                            SearchTicket t = new SearchTicket();

                            t.Id = -1;
                            t.Details = "You have no unassigned tickets";
                            t.MomName = "n/a";
                            t.HelperName = "n/a";

                            childTickets.Add(t);
                        }

                        ticketList.ItemsSource = childTickets;
                    }
                }
            }
            catch (Exception ex)
            {
                TopText.Text = ex.Message;

                Console.WriteLine("\n-------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------------\n");
            }
            finally
            {
                ticketList.IsRefreshing = false;
            }
        }
	}

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        using (HttpClient client = new HttpClient())
        {
            SearchTicket? s = ticketList.SelectedItem as SearchTicket;

            Ticket? selected = tickets.SingleOrDefault(t => t.Id == s.Id);

            if (selected != null)
            {
                selected.HelperId = helper.Id;
                selected.Status = "ASSIGNED";

                string[]? notifSettings = null;
                if (account.Notifs != null) notifSettings = account.Notifs.Split(",");

                try
                {
                    Mother? mom = null;

                    HttpResponseMessage momResponse = await client.GetAsync($"{URL}/Mothers/{selected.MomId}");
                    if (momResponse.IsSuccessStatusCode)
                    {
                        string json2 = await momResponse.Content.ReadAsStringAsync();
                        mom = JsonConvert.DeserializeObject<Mother>(json2);
                    }

                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Tickets/{selected.Id}", selected);
                    if (response3.IsSuccessStatusCode) 
                    {
                        EmailServices.SendNotifcation(helper.Email, $"{helper.FName} {helper.LName}", selected.Status, selected);

                        if (mom != null)
                        {
                            EmailServices.SendNotifcation(mom.Email, $"{mom.FName} {mom.LName}", selected.Status, selected);
                        }

                        if (notifSettings != null && notifSettings.Length == 5)
                        {
                            bool shouldSendChild = bool.Parse(notifSettings[1].ToLower());

                            if (shouldSendChild) EmailServices.SendNotifcation(account.Email, $"{account.FName} {account.LName}", selected.Status, selected);
                        }
                        else //If there are no settings, assume "true"
                        {
                            EmailServices.SendNotifcation(account.Email, $"{account.FName} {account.LName}", selected.Status, selected);
                        }

                        TopText.Text = "Success"; 
                    }
                    else TopText.Text = "Error";

                    if (Application.Current != null) Application.Current.MainPage = new NavigationPage(new ChildMenu(account));
                }
                catch (Exception ex)
                {
                    TopText.Text = ex.Message;

                    Console.WriteLine("\n-------------------------------------------------------------");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("-------------------------------------------------------------\n");
                }
            }
        }
    }
}