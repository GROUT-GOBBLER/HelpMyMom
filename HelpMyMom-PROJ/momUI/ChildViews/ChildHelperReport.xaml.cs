using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI.ChildViews;

public partial class ChildHelperReport : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

    Child account;
    Ticket ticket;
    Helper? helper = null;
    Mother? mom = null;

    string subject = "";
    string body = "";

    int titleFont = 35;
    int headerFont = 25;
    int normalFont = 18;

    public ChildHelperReport(Child acc, Ticket t)
    {
        InitializeComponent();

        account = acc;
        ticket = t;
    }

    protected override async void OnAppearing()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage momResponse = await client.GetAsync($"{URL}/Mothers/{ticket.MomId}");
                if (momResponse.IsSuccessStatusCode)
                {
                    string json2 = await momResponse.Content.ReadAsStringAsync();
                    mom = JsonConvert.DeserializeObject<Mother>(json2);
                }

                HttpResponseMessage HelperResponse = await client.GetAsync($"{URL}/Helpers/{ticket.HelperId}");
                if (momResponse.IsSuccessStatusCode)
                {
                    string json2 = await HelperResponse.Content.ReadAsStringAsync();
                    helper = JsonConvert.DeserializeObject<Helper>(json2);
                }

                if (helper != null && mom != null)
                {
                    ReportingText.Text = $"{helper.FName} {helper.LName}";
                }
            } catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "ok");

                Console.WriteLine("\n-------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------------\n");
            }
        }
    }

    private void SubjectEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        subject = SubjectEntry.Text;
    }

    private void DetailsEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        body = DetailsEntry.Text;
    }

    private async void ReportHelperClicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            List<Report>? allReports;
            Report report = new Report();

            try
            {
                HttpResponseMessage reportResponse = await client.GetAsync(URL + "/Reports");

                if (reportResponse.IsSuccessStatusCode)
                {
                    string json = await reportResponse.Content.ReadAsStringAsync();

                    allReports = JsonConvert.DeserializeObject<List<Report>>(json);

                    int newID = 0;
                    if (allReports != null)
                    {
                        foreach (Report r in allReports)
                        {
                            if (r.Id > newID) newID = r.Id;
                        }
                    }

                    newID++;

                    report.Id = newID;
                    report.HelperId = helper.Id;
                    report.MomId = mom.Id;
                    report.ChildId = account.Id;
                    report.TicketId = ticket.Id;

                    report.Subject = subject;
                    report.Body = body;

                    report.Ticket = null;
                    report.Helper = null;
                    report.Mom = null;
                    report.Child = null;

                    HttpResponseMessage response = await client.PostAsJsonAsync(URL + "/Reports", report);
                    if (response.IsSuccessStatusCode)
                    {
                        ReportBtn.Text = "Done!";

                        Application.Current.MainPage = new NavigationPage(new ChildMenu(account));
                    }
                    else
                    {
                        ReportBtn.Text = "Failed";
                    }

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
}