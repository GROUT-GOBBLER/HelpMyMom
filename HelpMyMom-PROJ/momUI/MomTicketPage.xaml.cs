using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;


namespace momUI
{
    public partial class MomTicketPage : ContentPage
    {

        private const double _ticketCost = 19.99; // Cost to create a ticket
        
        public string IssueText { get; set; }

       // string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";


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

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Tickets");
                    HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");
                    HttpResponseMessage response3 = await client.GetAsync(URL + "/Relationships");
                    HttpResponseMessage response4 = await client.GetAsync(URL + "/Children");


                    if (response3.IsSuccessStatusCode)
                    {
                        String json1 = await response1.Content.ReadAsStringAsync();
                        String json2 = await response2.Content.ReadAsStringAsync();
                        String json3 = await response3.Content.ReadAsStringAsync();
                        String json4 = await response4.Content.ReadAsStringAsync();

                        List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json1);
                        List<Mother> mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                        List<Relationship> relationshipList = JsonConvert.DeserializeObject<List<Relationship>>(json3);
                        List<Child> childrenList = JsonConvert.DeserializeObject<List<Child>>(json4);

                        int childrenID = 0;
                        Boolean found_childrenID = false;

                        foreach (Relationship index in relationshipList) {
                            if (index.MomId == _momAccountID)
                            {
                                childrenID = (int)index.ChildId;
                                found_childrenID = true;
                            }
                        }

                        int howManyChildrenDoYouHave = 0;
                        foreach (Child index in childrenList)
                        {
                            if (index.Id == childrenID)
                            {
                                howManyChildrenDoYouHave++;
                            }
                        }

                        if (!found_childrenID || !(howManyChildrenDoYouHave > 0))
                        {
                            await DisplayAlert("Error", "No children relationships found.", "OK");
                            return;
                        }

                        int length1 = ticketsList.Count;

                        int momIndexInList = 0;
                        foreach (Mother index in mothersList)
                        {
                            if (index.Id == _momAccountID)
                            {
                                momIndexInList = index.Id;
                            }
                        }

                        // Step 4: Create the ticket and add it to the database
                        Ticket newTicket = new Ticket
                        {
                            Id = ticketsList[length1 - 1].Id + 1,
                            MomId = _momAccountID,
                            ChildId = childrenID,
                            HelperId = null,
                            Status = "NEW",
                            Description = IssueText,
                            LogForm = null,
                            ReviewId = null,
                            Reports = [],
                            ChatLogs = [],
                            Helper = null,
                            Mom = mothersList[_momAccountID]
                        };

                        HttpResponseMessage createTicketResponse = await client.PostAsJsonAsync(
                            $"{URL}/{"Tickets"}",
                            newTicket);

                        
                        Mother updateMom = new Mother
                        {
                            Id = mothersList[momIndexInList].Id,
                            FName = mothersList[momIndexInList].FName,
                            LName = mothersList[momIndexInList].LName,
                            Email = mothersList[momIndexInList].Email,
                            Tokens = newBalance
                        };

                        HttpResponseMessage changeTokenAmountInMomResponse = await client.PutAsJsonAsync(
                            $"{URL}/{"Mothers"}/{momIndexInList}", 
                            updateMom);
                        

                        if (createTicketResponse.IsSuccessStatusCode)
                        {
                            // Step 5: Show success pop-up and navigate back
                            await DisplayAlert("Success", "Your ticket has been successfully sent!", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Error", $"Failed to create ticket: {createTicketResponse.StatusCode}", "OK");
                            return;
                        }

                    }
                    else
                    {
                        throw new Exception("No relationships");
                    }
                    
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to connect to Ticket Request: {ex.Message}", "OK");
                   // SubmitTicketButton.IsEnabled = false;
                 //   SubmitTicketButton.Text = ex.Message;

                   // Console.WriteLine("\n\n");
                   // Console.WriteLine(ex.ToString());
                }

            }
        }
    }
}