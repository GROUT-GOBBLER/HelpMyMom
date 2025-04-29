using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;


namespace momUI
{
    public partial class MomReviewPage : ContentPage
    {


        public string ReviewText { get; set; }
        public int Rating { get; set; } = 0; 
        // Default rating of 0. Maximum of 10. (Out of 5 stars)

        private int _momAccountID;
        private int _helperAccountID;
        private int _ticketID;

        public MomReviewPage(int momID, int helperID, int ticketID)
        {
            InitializeComponent();
            BindingContext = this;

            _momAccountID = momID;
            _helperAccountID = helperID;
            _ticketID = ticketID;
            UpdatePageVariables();

        }
        protected override void OnAppearing()
        {
            Accessibility a = Accessibility.getAccessibilitySettings();
            PageTitle.FontSize = Math.Min(Math.Max(30, a.fontsize + 20), 50);

            MomReviewTemplateText.FontSize = Math.Min(Math.Max(10, a.fontsize), 30);

            RatingDialogue.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);
            RatingPicker.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);

            PleaseWriteReview.FontSize = Math.Min(Math.Max(10, a.fontsize), 30);
            IssueDescriptionBox1.FontSize = Math.Min(Math.Max(10, a.fontsize), 30);

            SubmitTicketButton1.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);

            GoBack.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);

        }


        private async void OnRatingSelected(object sender, EventArgs e)
        {
            if (RatingPicker.SelectedItem != null)
            {
                int selectedRating = (int)RatingPicker.SelectedItem;
                // SelectedRatingLabel.Text = $"Selected Rating: {selectedRating}/10";
                Rating = selectedRating;
            }
        }

        private async void UpdatePageVariables()
        {          
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Helpers");

                    string json1 = await response1.Content.ReadAsStringAsync();

                    List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(json1);

                    foreach (Helper index in helpersList)
                    {
                        // await DisplayAlert("Testing:", "Got here at least", "OK");
                        if (index.Id == _helperAccountID)
                        {
                            MomReviewTemplateText.Text = $"How did {index.FName} {index.LName} do in assisting you?";
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Cannot find helper ID
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
            if (Rating == 0)
            {
                await DisplayAlert("ERROR:", "Please rate this helper before submitting!", "OK");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Tickets");
                    HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");
                    HttpResponseMessage response3 = await client.GetAsync(URL + "/Helpers");
                    HttpResponseMessage response4 = await client.GetAsync(URL + "/Children");
                    HttpResponseMessage response5 = await client.GetAsync(URL + "/Reviews");
                    HttpResponseMessage response6 = await client.GetAsync(URL + "/Accounts");

                    String json1 = await response1.Content.ReadAsStringAsync();
                    String json2 = await response2.Content.ReadAsStringAsync();
                    String json3 = await response3.Content.ReadAsStringAsync();
                    String json4 = await response4.Content.ReadAsStringAsync();
                    String json6 = await response6.Content.ReadAsStringAsync();

                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json1);
                    List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                    List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(json3);
                    List<Child>? childrenList = JsonConvert.DeserializeObject<List<Child>>(json4);
                    List<Account>? accountList = JsonConvert.DeserializeObject<List<Account>>(json6);

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

                    if (createReviewResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "Your review has been successfully sent!", "OK");

                        String accountUserName = "";
                        foreach (Account index in accountList)
                        {
                            if (index.MomId == _momAccountID)
                            {
                                accountUserName = index.Username;
                                break;
                            }
                        }
                        await Navigation.PushAsync(new MomMenu(accountUserName, _momAccountID));
                      //  await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", $"Failed to create ticket: {createReviewResponse.StatusCode}", "OK");
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