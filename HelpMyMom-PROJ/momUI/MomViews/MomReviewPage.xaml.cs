using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;


namespace momUI
{
    public partial class MomReviewPage : ContentPage
    {


        public string ReviewText { get; set; }
        public int Rating { get; set; } = 5; // Default rating of 5 stars


       // private double _currentAccountBalance;
        private int _momAccountID;
        private int _helperAccountID;
        private int _ticketID;

        public MomReviewPage(int momID, int helperID, int ticketID)
        {
            InitializeComponent();
            BindingContext = this;


           // _currentAccountBalance = (double)currentBalance;
            _momAccountID = momID;
            _helperAccountID = helperID;
            _ticketID = ticketID;
            UpdatePageVariables();

        }


        private async void UpdatePageVariables()
        {
            /*
            PLACEHOLDER, HARDCODED TO USE A SPECIFIC ACCOUNT RIGHT NOW
            WILL CHANGE LATER BUT RIGHT NOW DONT HAVE LOG IN PAGE SO WE CANT ACCESS 
            LIKE THAT, SO JUST USE THIS FOR NOW, WILL NEED TO CHANGE LATER
            */
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Helpers");

                    string json1 = await response1.Content.ReadAsStringAsync();

                    List<Helper> helpersList = JsonConvert.DeserializeObject<List<Helper>>(json1);

                    foreach (Helper index in helpersList)
                    {
                        // await DisplayAlert("Testing:", "Got here at least", "OK");
                        if (index.Id == _helperAccountID)
                        {
                            MomReviewTemplateText.Text = $"How did {index.FName} {index.LName} do in assisting you?";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors (e.g., show a default value or error message)
                    MomReviewTemplateText.Text = $"ERROR: ({ex.Message}), could not load the helper's account.";
                }

            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Tickets");
                    HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");;
                    HttpResponseMessage response3 = await client.GetAsync(URL + "/Helpers");
                    HttpResponseMessage response5 = await client.GetAsync(URL + "/Reviews");

                    String json1 = await response1.Content.ReadAsStringAsync();
                    String json2 = await response2.Content.ReadAsStringAsync();
                    String json3 = await response3.Content.ReadAsStringAsync();

                    List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json1);
                    List<Mother> mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                    List<Helper> helpersList = JsonConvert.DeserializeObject<List<Helper>>(json1);

                    foreach (Helper index in helpersList)
                    {
                        // await DisplayAlert("Testing:", "Got here at least", "OK");
                        if (index.Id == _helperAccountID)
                        {
                            MomReviewTemplateText.Text = $"How did {index.FName} {index.LName} do in assisting you?";
                        }
                    }

                    int momIndexInList = 0;
                    foreach(Mother index in mothersList)
                    {
                        if (index.Id == _momAccountID)
                        {
                            momIndexInList = index.Id;
                        }
                    }

                    // Create a review object with the rating and review text
                    Review newReview = new Review
                    {
                        Id = _ticketID,
                        HelperId = _helperAccountID,
                        MomId = _momAccountID,
                        Stars = (short?)Rating,
                        Text = ReviewText
                    };


                    HttpResponseMessage createReviewResponse = await client.PostAsJsonAsync(
                        $"{URL}/{"Reviews"}", 
                        newReview);

                    HttpResponseMessage getHelperResponse = await client.GetAsync($"{URL}/{"Helpers"}/{_helperAccountID}");
                    string helperJson = await getHelperResponse.Content.ReadAsStringAsync();

                    Helper _helper = JsonConvert.DeserializeObject<Helper>(helperJson);

                    double newBalance = (double)_helper.Tokens + 20.00;

                    Helper updateHelper = new Helper
                    {
                        Id = _helperAccountID,
                        FName = _helper.FName,
                        LName = _helper.LName,
                        Email = _helper.Email,
                        Specs = _helper.Specs,
                        Description = _helper.Description,
                        Dob = _helper.Dob,
                        Banned = _helper.Banned,
                        Tokens = newBalance
                    };

                    HttpResponseMessage changeTokenAmountInHelperResponse = await client.PutAsJsonAsync(
                        $"{URL}/{"Helpers"}/{_helperAccountID}", 
                        updateHelper);

                    if (createReviewResponse.IsSuccessStatusCode && changeTokenAmountInHelperResponse.IsSuccessStatusCode)
                    {
                        // Step 5: Show success pop-up and navigate back
                        await DisplayAlert("Success", "Your review and payment has been successfully sent!", "OK");
                        await Navigation.PopAsync();
                    }
                    else if (!createReviewResponse.IsSuccessStatusCode && !changeTokenAmountInHelperResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", $"Failed to create review and transfer payment: {createReviewResponse.StatusCode} & {changeTokenAmountInHelperResponse.StatusCode}", 
                            "OK");
                        return;
                    }
                    else if (!changeTokenAmountInHelperResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", $"Failed to transfer payment balance: {changeTokenAmountInHelperResponse.StatusCode}", "OK");
                        return;
                    }
                    else
                    {
                        await DisplayAlert("Error", $"Failed to create review: {createReviewResponse.StatusCode}", "OK");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to connect to Review Request: {ex.Message}", "OK");
                }

            }
        }
    }
}