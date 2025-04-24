using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;


namespace momUI
{
    public partial class MomReportPage : ContentPage
    {


        public string ReviewText { get; set; }
        public string SubjectText { get; set; }


        private int _momAccountID;
        private int _helperAccountID;
        private int _ticketID;

        public MomReportPage(int momID, int helperID, int ticketID)
        {
            InitializeComponent();
            BindingContext = this;

            _momAccountID = momID;
            _helperAccountID = helperID;
            _ticketID = ticketID;
            UpdatePageVariables();

        }

        protected override async void OnAppearing()
        {
            Accessibility a = Accessibility.getAccessibilitySettings();
            PageTitle.FontSize = Math.Min(Math.Max(20, a.fontsize + 11), 30);
            MomReportTemplateText.FontSize = Math.Min(Math.Max(10, a.fontsize), 20);
            SubjectLabel.FontSize = Math.Min(Math.Max(12, a.fontsize + 3), 18);
            IssueSubjectBox.FontSize = Math.Min(Math.Max(12, a.fontsize), 15);
            IssueDescriptionBox1.FontSize = Math.Min(Math.Max(12, a.fontsize), 15);
            GoBack.FontSize = Math.Min(Math.Max(10, a.fontsize), 20);
            SubmitReportButton.FontSize = Math.Min(Math.Max(10, a.fontsize), 20);
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
                            MomReportTemplateText.Text = $"What problems are you having with {index.FName} {index.LName}?";
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Could not find helper's name
                    MomReportTemplateText.Text = $"ERROR: ({ex.Message}), could not load the helper's account.";
                }

            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // Need to fill both fields.
            if (string.IsNullOrEmpty(ReviewText) || string.IsNullOrEmpty(SubjectText))
            {
                await DisplayAlert("ERROR:", "Fields for Subject and Explanation need to be filled!", "OK");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // await DisplayAlert("1:", "Successfully Loaded Into the try", "OK");

                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Tickets");
                    HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");
                    HttpResponseMessage response3 = await client.GetAsync(URL + "/Helpers");
                    HttpResponseMessage response4 = await client.GetAsync(URL + "/Children");
                    HttpResponseMessage response5 = await client.GetAsync(URL + "/Reports");
                    HttpResponseMessage response6 = await client.GetAsync(URL + "/Accounts");

                    String json1 = await response1.Content.ReadAsStringAsync();
                    String json2 = await response2.Content.ReadAsStringAsync();
                    String json3 = await response3.Content.ReadAsStringAsync();
                    String json4 = await response4.Content.ReadAsStringAsync();
                    String json5 = await response5.Content.ReadAsStringAsync();
                    String json6 = await response6.Content.ReadAsStringAsync();

                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json1);
                    List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                    List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(json3);
                    List<Child>? childrenList = JsonConvert.DeserializeObject<List<Child>>(json4);
                    List<Report>? reportsList = JsonConvert.DeserializeObject<List<Report>>(json5);
                    List<Account>? accountList = JsonConvert.DeserializeObject<List<Account>>(json6);


                    foreach (Helper index in helpersList)
                    {
                        // await DisplayAlert("Testing:", "Got here at least", "OK");
                        if (index.Id == _helperAccountID)
                        {
                            MomReportTemplateText.Text = $"How did {index.FName} {index.LName} do in assisting you?";
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

                    HttpResponseMessage relationshipResponse = await client.GetAsync(URL + "/Relationships");
                    String relationsString = await relationshipResponse.Content.ReadAsStringAsync();
                    int childrenID = 0;

                    List<Relationship>? relationshipList = JsonConvert.DeserializeObject<List<Relationship>>(relationsString);


                    foreach (Relationship index in relationshipList)
                    {
                        if (index.MomId == _momAccountID)
                        {
                            childrenID = (int)index.ChildId;
                            break;
                        }
                    }

                    int length1 = reportsList.Count;
                    int newReportID;
                    if (length1 <= 0)
                    {
                        newReportID = 1;
                    }
                    else
                    {
                        newReportID = reportsList[length1 - 1].Id + 1;
                    }
                    // Create a report object with the reason text
                    Report newReport = new Report
                    {
                        Id = newReportID,
                        HelperId = _helperAccountID,
                        MomId = _momAccountID,
                        TicketId = _ticketID,
                        Subject = SubjectText,
                        Body = ReviewText
                    };


                    HttpResponseMessage createReportResponse = await client.PostAsJsonAsync(
                        $"{URL}/{"Reports"}", 
                        newReport);

                    if (createReportResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "You have succesfully reported this user!", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", $"Failed to create report: {createReportResponse.StatusCode}", "OK");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to connect to Report Request: {ex.Message}", "OK");
                }

            }
        }
    }
}