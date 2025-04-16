namespace momUI;

using models;
using Newtonsoft.Json;
using System.Net.Http.Json;

public partial class UnassignedTickets : ContentPage
{
    List<Ticket> tickets = new List<Ticket>();
    Child account;
	Helper helper;

    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

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
                            ticketList.ItemsSource = tickets;
                        }
                        else
                        {
                            Ticket t = new Ticket();

                            t.Id = -1;
                            t.Description = "You have no unassigned tickets";

                            tickets.Add(t);
                        }
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
        }
	}

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        using (HttpClient client = new HttpClient())
        {
            Ticket selected = ticketList.SelectedItem as Ticket;
            selected.HelperId = helper.Id;
            selected.Status = "ASSIGNED";

            try
            {
                HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Tickets/{selected.Id}", selected);
                if (response3.IsSuccessStatusCode) TopText.Text = "Success";
                else TopText.Text = "Error";

                Application.Current.MainPage = new NavigationPage(new ChildMenu(account));
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