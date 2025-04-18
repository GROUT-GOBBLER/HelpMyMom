using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;

namespace momUI
{
    public partial class MomMenu : ContentPage
    {

        //string URL = $"http://localhost:5124/api";

        string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";



        private string _balanceLabel;
        public string BalanceLabel
        {
            get => _balanceLabel;
            set
            {
                _balanceLabel = value;
                OnPropertyChanged(nameof(BalanceLabel)); // Notify the UI of the change
            }
        }
       
        private double? _balance = 0.00;

        private int _momID;
        private String _accountID;
        private int _chatID;
        private int _childrenID;
        private int _helpersID;
        private int _ticketsID;


        public string balanceAddText { get; set; }

        public MomMenu()
        {
            InitializeComponent();
            BindingContext = this; // Set the binding context to this page
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FetchImportantVariables();
        }

        private async void ModifyBalance()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Tickets");
                    HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");
                    HttpResponseMessage response3 = await client.GetAsync(URL + "/Relationships");

                    String json1 = await response1.Content.ReadAsStringAsync();
                    String json2 = await response2.Content.ReadAsStringAsync();
                    String json3 = await response3.Content.ReadAsStringAsync();


                    List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json1);
                    List<Mother> mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                    List<Relationship> relationshipList = JsonConvert.DeserializeObject<List<Relationship>>(json3);

                    int momIndexInList = 0;

                    foreach (Mother index in mothersList)
                    {
                        if (index.Id == _momID)
                        {
                            momIndexInList = index.Id;
                        }
                    }

                    double newBalance = (double)_balance;

                    Mother updateMom = new Mother
                    {
                        Id = mothersList[momIndexInList].Id,
                        FName = mothersList[momIndexInList].FName,
                        LName = mothersList[momIndexInList].LName,
                        Email = mothersList[momIndexInList].Email,
                        Tokens = newBalance
                    };

                    HttpResponseMessage changeTokenAmountInMomResponse = await client.PutAsJsonAsync($"{URL}/{"Mothers"}/{momIndexInList}", updateMom);

                    if (changeTokenAmountInMomResponse.IsSuccessStatusCode)
                    {
                        // Step 5: Show success pop-up and navigate back
                        BalanceLabel = $"Current Balance: ${newBalance:F2}";
                        await DisplayAlert("Success", "Your balance has been successfully updated!", "OK");
                        return;
                    }
                    else
                    {
                        await DisplayAlert("Error", $"Failed to create ticket: {changeTokenAmountInMomResponse.StatusCode}", "OK");
                        return;
                    }

                }
                catch (Exception ex)
                {
                    // Handle errors (e.g., show a default value or error message)
                    await DisplayAlert("Error", $"Failed to connect to add  to Balance: {ex.Message}", "OK");
                }
            }
        }

        private void OnAddBalanceClicked(object sender, EventArgs e)
        {
            // Show the popup
            AddBalancePopup.IsVisible = true;
            BalanceEntry.Text = string.Empty; // Clear the entry field
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            // Show the popup
            AddBalancePopup.IsVisible = false;
            BalanceEntry.Text = string.Empty; // Clear the entry field
        }

        private async void OnSubmitBalanceClicked(object sender, EventArgs e)
        {
            if (decimal.TryParse(BalanceEntry.Text, out decimal amount))
            {
                if (amount < 0)
                {
                    // Treat negative numbers as 0
                    amount = 0;
                } 
                // Round 2 decimal places
                amount = Math.Round(amount, 2);
                // Update balance
                _balance += (double)amount;
                // Hide the popup
                AddBalancePopup.IsVisible = false;
                ModifyBalance();
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number.", "OK");
            }
        }


        private async void FetchImportantVariables()
        {
            /*
            PLACEHOLDER, HARDCODED TO USE A SPECIFIC ACCOUNT RIGHT NOW
            WILL CHANGE LATER BUT RIGHT NOW DONT HAVE LOG IN PAGE SO WE CANT ACCESS 
            LIKE THAT, SO JUST USE THIS FOR NOW, WILL NEED TO CHANGE LATER
            */
            BalanceLabel = "Current Balance: N/A ()";
            using (HttpClient client = new HttpClient())
            {
                _accountID = "LoveMyRan";
                _momID = 0;

                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Accounts");
                    string json1 = await response1.Content.ReadAsStringAsync();
                    List<Account> accountsList = JsonConvert.DeserializeObject<List<Account>>(json1);

                    HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");
                    string json2 = await response2.Content.ReadAsStringAsync();
                    List<Mother> motherList = JsonConvert.DeserializeObject<List<Mother>>(json2);

                    Boolean found = false;

                    foreach (Mother index in motherList)
                    {
                       
                        if (index.Id == _momID)
                        {
                            _balance = (double)index.Tokens;
                            MomNameHeader.Text = $"{index.FName} {index.LName}";

                            // Update the BalanceText property with the fetched value
                           BalanceLabel = $"Current Balance: ${_balance:F2}";
                           found = true;
                                
                            
                        }
                    }
                    if (found == false)
                    {
                        // Handle errors (e.g., show a default value or error message)
                        BalanceLabel = $"Current Balance: N/A (Could Not Find An Account with that username)";
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors (e.g., show a default value or error message)
                    BalanceLabel = $"Current Balance: N/A ({ex.Message})";
                }

            }
        }

        /*
        private async void FetchBalance()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(mothers_url);
                    response.EnsureSuccessStatusCode(); // Throw if not successful

                    string json = await response.Content.ReadAsStringAsync();
                    // Assuming the API returns a JSON object like { "balance": 123.45 }
                    var balanceData = JsonConvert.DeserializeObject<Mother>(json);
                    
                    if (balanceData != null)
                    {
                        _balance = balanceData.Tokens;
                    }

                    // Update the BalanceText property with the fetched value
                    BalanceLabel = $"Current Balance: ${balanceData.Tokens:F2}";
                  
                }
                catch (Exception ex)
                {
                    // Handle errors (e.g., show a default value or error message)
                    BalanceLabel = $"Current Balance: N/A ({ex.Message})";
                }
            }
        }
        */

        async private void OnOpenChatsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MomChatsPage(_momID));
        }

        async private void OnCreateTicketClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MomTicketPage(_balance, _momID));
        }

        /*
        async private void OnCounterClicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Specs"}/{1}");
                    
                    string json = await response2.Content.ReadAsStringAsync();

                    Spec Specs = JsonConvert.DeserializeObject<Spec>(json);

                    CounterBtn.Text = $" id: {Specs.Id} String: {Specs.Name}";

                }
                    catch (Exception ex)
                {
                    CounterBtn.Text = $" {ex}";
                }

                EmailServices.SendNotifcation("hmmprojectmom@hotmail.com", "completed", 1);

            }
        }

        async private void post_Clicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {

                    Spec s = new Spec();
                    s.Id = 2;
                    s.Name = "Microsoft Word";
                    HttpResponseMessage response = await client.PostAsJsonAsync($"{URL}/{"Specs"}", s); 

                    post.Text = $" success";
                }
                catch (Exception ex)
                {
                    post.Text = $" {ex}";
                }
            }
        }
        */


    }

}
