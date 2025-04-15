namespace momUI;

using models;
using Newtonsoft.Json;

public partial class ConfirmHelper : ContentPage
{
    Child account;
    Ticket ticket;
    Helper helper;

    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

    public ConfirmHelper(Child acc, Helper h, Ticket t = null)
	{
		InitializeComponent();

        account = acc;
        helper = h;
        if (t != null) ticket = t;
    }

    protected override async void OnAppearing()
    {
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
                    List<Review> allReviews = JsonConvert.DeserializeObject<List<Review>>(json2);

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
                    List<Spec> allSpecs = JsonConvert.DeserializeObject<List<Spec>>(json);
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

    private void NothingClicked(object sender, EventArgs e)
    {

    }
}