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



        private double? _balance = 0.00;

        private int _momID;
        private String _accountID;
        private int _chatID;
        private int _childrenID;
        private int _helpersID;
        private int _ticketsID;



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

                    HttpResponseMessage response1 = await client.GetAsync(accounts_url);
                    // DisplayAlert("Testing:", $"{ client.BaseAddress + accounts_url }", "OK");
                  //  response1.EnsureSuccessStatusCode();

                    HttpResponseMessage response1 = await client.GetAsync(URL + accounts_url);
                    string json1 = await response1.Content.ReadAsStringAsync();
                    List<Account> accountsList = JsonConvert.DeserializeObject<List<Account>>(json1);


                    HttpResponseMessage response2 = await client.GetAsync(mothers_url);
                    // DisplayAlert("Testing:", $"{ client.BaseAddress + mothers_url }", "OK");
                 //   response2.EnsureSuccessStatusCode();

                    HttpResponseMessage response2 = await client.GetAsync(URL + mothers_url);
                    string json2 = await response2.Content.ReadAsStringAsync();
                    List<Mother> motherList = JsonConvert.DeserializeObject<List<Mother>>(json1);



                    Boolean found = false;

                    foreach (Mother index in motherList)
                    {
                        DisplayAlert("Testing:", "Got here at least", "OK");
                        if (index.Id == _momID)
                        {
                            _balance = (double)index.Tokens;

                           // float balanceNumber = (float)_balance;

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
            // Add functionality for Open Chats button
            // DisplayAlert("Open Chats", "Open Chats button clicked", "OK");
            await Navigation.PushAsync(new MomChatsPage());
        }

        async private void OnCreateTicketClicked(object sender, EventArgs e)
        {
            // Add functionality for Create a Help Ticket button
            // DisplayAlert("Create Ticket", "Create a Help Ticket button clicked", "OK");

            await Navigation.PushAsync(new MomTicketPage(_balance, _momID));
        }

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

        /*
        public class BalanceResponse
        {
            public double Balance { get; set; }
        }
        */

    }

}
