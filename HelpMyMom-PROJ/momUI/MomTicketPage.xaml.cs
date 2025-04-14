using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;


namespace momUI
{
    public partial class MomTicketPage : ContentPage
    {

        private const double _ticketCost = 20.00; // Cost to create a ticket
        
        public string IssueText { get; set; }

       // string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

        String accounts_url = "/Accounts";
        String chatlogs_url = "/ChatLogs";
        String children_url = "/Children";
        String helpers_url = "/Helpers";
        String mothers_url = "/Mothers";
        String relationships_url = "/Relationships";
        String reports_url = "/Reports";
        String reviews_url = "/Reviews";
        String specs_url = "/Specs";
        String tickets_url = "/Tickets";



        private double _currentAccountBalance;

        private int _momAccountID;

        public MomTicketPage(double? currentBalance, int momID)
        {
            InitializeComponent();
            BindingContext = this;


            _currentAccountBalance = (double)currentBalance;
            _momAccountID = momID;

        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(IssueText))
            {
                await DisplayAlert("Error", "Please enter an issue.", "OK");
                return;
            }

            // Step 1: Fetch the current balance from the database
            double currentBalance = _currentAccountBalance;

            // Step 2: Check if the balance is sufficient
            if (currentBalance < _ticketCost)
            {
                await DisplayAlert("Insufficient Balance",
                    $"You need at least ${_ticketCost} to create a ticket. Your current balance is ${currentBalance}.",
                    "OK");
                return;
            }


            
            // Step 3: Deduct the ticket cost from the balance
            double newBalance = currentBalance - _ticketCost;
            //MockDatabase.UpdateBalance(newBalance);
            

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + tickets_url);
                    //response1.EnsureSuccessStatusCode();
                    String json1 = await response1.Content.ReadAsStringAsync();

                    HttpResponseMessage response2 = await client.GetAsync(URL + mothers_url);
                  //  response2.EnsureSuccessStatusCode();
                    String json2 = await response2.Content.ReadAsStringAsync();

                    HttpResponseMessage response3 = await client.GetAsync(URL + relationships_url);
                   // response3.EnsureSuccessStatusCode();
                    String json3 = await response3.Content.ReadAsStringAsync();


                    List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json1);
                    List<Mother> mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                    List<Relationship> relationshipList = JsonConvert.DeserializeObject<List<Relationship>>(json3);

                    int childrenID = 4;

                    foreach (Relationship index in relationshipList) {
                        if (index.MomId == _momAccountID)
                        {
                            childrenID = (int)index.ChildId;
                        }
                    }

                    int length1 = ticketsList.Count;

                    int momIndexInList = 0;
                    foreach(Mother index in mothersList)
                    {
                        if (index.Id == _momAccountID)
                        {
                            momIndexInList = index.Id;
                        }
                    }

                    // Step 4: Create the ticket and add it to the database
                    Ticket newTicket = new Ticket
                    {
                        Id = ticketsList[length1 - 1].Id + 1, // LastTicketID + 1
                        MomId = _momAccountID, 
                        ChildId = childrenID,
                        HelperId = null,
                        Status = "Open",
                        Description = IssueText,
                        LogForm = null,
                        ReviewId = null,
                        // CreatedAt = DateTime.Now
                    };

                    HttpResponseMessage createTicketResponse = await client.PostAsJsonAsync(URL + tickets_url, newTicket);

                    Mother updateMom = new Mother
                    {
                        Id = mothersList[momIndexInList].Id,
                        FName = mothersList[momIndexInList].FName,
                        LName = mothersList[momIndexInList].LName,
                        Email = mothersList[momIndexInList].Email,
                        Tokens = newBalance
                    };

                    HttpResponseMessage changeTokenAmountInMomResponse = await client.PutAsJsonAsync(URL + mothers_url, updateMom);

                    if (createTicketResponse.IsSuccessStatusCode && changeTokenAmountInMomResponse.IsSuccessStatusCode)
                    {
                        // Step 5: Show success pop-up and navigate back
                        await DisplayAlert("Success", "Your ticket has been successfully sent!", "OK");
                        await Navigation.PopAsync();
                    }
                    else if (!createTicketResponse.IsSuccessStatusCode && !changeTokenAmountInMomResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", $"Failed to create ticket and update mom's balance: {createTicketResponse.StatusCode} & {changeTokenAmountInMomResponse.StatusCode}", 
                            "OK");
                        return;
                    }
                    else if (!changeTokenAmountInMomResponse.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", $"Failed to create update balance: {changeTokenAmountInMomResponse.StatusCode}", "OK");
                        return;
                    }
                    else
                    {
                        await DisplayAlert("Error", $"Failed to create ticket: {createTicketResponse.StatusCode}", "OK");
                        return;
                    }


                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to connect to Ticket Request: {ex.Message}", "OK");
                }

            }
        }
    }
}