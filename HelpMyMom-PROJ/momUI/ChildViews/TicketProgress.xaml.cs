using momUI.ChildViews;
using momUI.models;
using Newtonsoft.Json;

namespace momUI;

public partial class TicketProgress : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    
    Child account;
    List<Ticket>? tickets = new List<Ticket>();
    List<SearchTicket> childTickets = new List<SearchTicket>();

    int titleFont = 35;
    int headerFont = 25;
    int normalFont = 18;

    int fontOffset = 0;

    public TicketProgress(Child acc)
	{
		InitializeComponent();

        account = acc;
	}

    protected override async void OnAppearing()
    {
        TopText.FontSize = titleFont + fontOffset;
        reportText.FontSize = normalFont + fontOffset;

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
                        //tickets.RemoveAll(t => t.HelperId != null);
                        tickets.RemoveAll(t => t.ChildId != account.Id);
                        tickets.RemoveAll(t => t.Status.ToUpper() == "APPROVED");

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
                            t.Details = "You have no tickets";
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
        try
        {
            SearchTicket? s = ticketList.SelectedItem as SearchTicket;

            Ticket? selected = tickets.SingleOrDefault(t => t.Id == s.Id);

            if (selected != null && selected.HelperId != null)
            {
                await Navigation.PushAsync(new ChildHelperReport(account, selected));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "ok");

            Console.WriteLine("\n-------------------------------------------------------------");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("-------------------------------------------------------------\n");
        }
    }
}