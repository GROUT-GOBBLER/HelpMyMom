namespace momUI;

using models;
using Newtonsoft.Json;
using System.Net.Http.Json;

public partial class ConfirmHelper : ContentPage
{
    Child account;
    Ticket? ticket = null;
    Helper helper;

    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

    int titleFont = 35;
    int headerFont = 25;
    int normalFont = 18;

    int fontOffset = 0;

    public ConfirmHelper(Child acc, Helper h, Ticket? t = null)
	{
		InitializeComponent();

        account = acc;
        helper = h;
        if (t != null) ticket = t;
    }

    protected override async void OnAppearing()
    {
        Name.FontSize = titleFont + fontOffset;
        Email.FontSize = normalFont + fontOffset;
        Rating.FontSize = normalFont + fontOffset;
        Description.FontSize = normalFont + fontOffset;
        TextSpecs.FontSize = normalFont + fontOffset;

        confirmBtn.FontSize = titleFont + fontOffset;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                Name.Text = $"{helper.FName} {helper.LName}";

                Email.Text = $"{helper.Email}";


                HttpResponseMessage reviewResponse = await client.GetAsync(URL + "/Reviews");
                if (reviewResponse.IsSuccessStatusCode)
                {
                    // Calc Rating
                    int calc = 0;

                    string json2 = await reviewResponse.Content.ReadAsStringAsync();
                    List<Review>? allReviews = JsonConvert.DeserializeObject<List<Review>>(json2);

                    Double sum = 0;
                    int count = 0;

                    allReviews.RemoveAll(r => helper.Id != r.HelperId);

                    if (allReviews != null)
                    {
                        foreach (Review v in allReviews)
                        {
                            sum += (int)v.Stars;
                            count++;
                        }
                        if (count > 0)
                        {
                            sum = sum / count;
                            calc = (int)Math.Ceiling(sum / 2);
                        }
                    }

                    Rating.Text = $"Rating: {calc}";
                }

                HttpResponseMessage specResponse = await client.GetAsync(URL + "/Specs");
                if (specResponse.IsSuccessStatusCode)
                {
                    // get specs
                    string json = await specResponse.Content.ReadAsStringAsync();
                    List<Spec>? allSpecs = JsonConvert.DeserializeObject<List<Spec>>(json);

                    var specIds = helper.Specs
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => short.TryParse(id.Trim(), out var parsedId) ? parsedId : (short?)null)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToHashSet();

                    List<Spec> filteredSpecs = allSpecs.Where(spec => specIds.Contains(spec.Id)).ToList();

                    List<string> specStrings = new List<string>();
                    foreach (Spec spec in filteredSpecs)
                    {
                        string specString = spec.Name;
                    }

                    TextSpecs.Text = string.Join(", ", specStrings);
                }

                Description.Text = $"{helper.Description}";
            }
            catch (Exception ex)
            {
                Description.Text = ex.Message;

                Console.WriteLine("\n-------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------------\n");
            }
        }
    }

    private async void finishedClicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            if (ticket != null)
            {
                ticket.HelperId = helper.Id;
                ticket.Status = "ASSIGNED";

                string[]? notifSettings = null;
                if (account.Notifs != null) notifSettings = account.Notifs.Split(",");
                Mother? mom = null;

                HttpResponseMessage momResponse = await client.GetAsync($"{URL}/Mothers/{ticket.MomId}");
                if (momResponse.IsSuccessStatusCode)
                {
                    string json2 = await momResponse.Content.ReadAsStringAsync();
                    mom = JsonConvert.DeserializeObject<Mother>(json2);
                }

                try
                {
                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Tickets/{ticket.Id}", ticket);

                    if (response3.IsSuccessStatusCode)
                    {
                        EmailServices.SendNotifcation(helper.Email, $"{helper.FName} {helper.LName}", ticket.Status, ticket);

                        if (mom != null)
                        {
                            EmailServices.SendNotifcation(mom.Email, $"{mom.FName} {mom.LName}", ticket.Status, ticket);
                        }

                        if (notifSettings != null && notifSettings.Length == 5)
                        {
                            bool shouldSendChild = bool.Parse(notifSettings[1].ToLower());

                            if (shouldSendChild) EmailServices.SendNotifcation(account.Email, $"{account.FName} {account.LName}", ticket.Status, ticket);
                        }
                        else //If there are no settings, assume "true"
                        {
                            EmailServices.SendNotifcation(account.Email, $"{account.FName} {account.LName}", ticket.Status, ticket);
                        }

                        confirmBtn.Text = "Success";
                    }
                    else confirmBtn.Text = "Error";
                }
                catch (Exception ex)
                {
                    Description.Text = ex.Message;

                    Console.WriteLine("\n-------------------------------------------------------------");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("-------------------------------------------------------------\n");
                }

                Application.Current.MainPage = new NavigationPage(new ChildMenu(account));
            }
            else
            {
                await Navigation.PushAsync(new UnassignedTickets(account, helper));
            }
        }
    }
}