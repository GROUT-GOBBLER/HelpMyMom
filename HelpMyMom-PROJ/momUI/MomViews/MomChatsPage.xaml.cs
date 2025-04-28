using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Data;

namespace momUI
{
    public partial class MomChatsPage : ContentPage
    {
        
        public ObservableCollection<ChatItem> Chats { get; set; }

        // public List<ChatItem> Chats { get; set; }

        private double _currentAccountBalance;

        private int _momAccountID;

        public MomChatsPage(int momID)
        {
            InitializeComponent();

            _momAccountID = momID;

            Chats = new ObservableCollection<ChatItem>();
            
            BindingContext = this;

            OnlyLoadChatsForTicketsWithCorrectMomID();


            
        }


        protected override async void OnAppearing()
        {
            Accessibility a = Accessibility.getAccessibilitySettings();
            PageTitle.FontSize = Math.Min(Math.Max(30, a.fontsize + 20), 50);
           
            GoBack.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);
        }


        private async void OnlyLoadChatsForTicketsWithCorrectMomID()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

                    HttpResponseMessage response1 = await client.GetAsync(URL + "/Tickets");
                    HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");
                    HttpResponseMessage response3 = await client.GetAsync(URL + "/ChatLogs");
                    HttpResponseMessage response4 = await client.GetAsync(URL + "/Helpers");

                    String json1 = await response1.Content.ReadAsStringAsync();
                    String json2 = await response2.Content.ReadAsStringAsync();
                    String json3 = await response3.Content.ReadAsStringAsync();
                    String json4 = await response4.Content.ReadAsStringAsync();


                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json1);
                    List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                    List<ChatLog>? chatList = JsonConvert.DeserializeObject<List<ChatLog>>(json3);
                    List<Helper>? helperList = JsonConvert.DeserializeObject<List<Helper>>(json4);

                    
                    // Valid statuses: NEW, ASSIGNED, IN PROGRESS, COMPLETED, APPROVED
                    foreach (Ticket ticketIndex in ticketsList)
                    {

                        String helperName = " ";
                        DateTime displayTime = DateTime.Now;
                        //await DisplayAlert("Default DisplayTime:", $"{displayTime}", "OK");
                        String displayText = " ";
                        int helperId;
                        int ticketId = -1;


                        /* 
                         If ticket's momID = momID
                         If ticket has a helper assigned to it
                         If ticket isnt finished already or has no status
                        */
                        if (ticketIndex.MomId == _momAccountID 
                            && ticketIndex.HelperId != null 
                            && ticketIndex.Status != "APPROVED"
                            && ticketIndex.Status != null)
                        {
                        
                            ticketId = ticketIndex.Id;
                            helperId = (int)ticketIndex.HelperId;
                            //await DisplayAlert("Successfully found a Ticket:", $"Ticket Number: {ticketId}, Helper Number: {helperId}", "OK");

                            foreach (Helper helperIndex in helperList)
                            {
                                if (helperIndex.Id == helperId)
                                {
                                    helperName = $"{helperIndex.FName} {helperIndex.LName}";
                                   // await DisplayAlert("HelperName Acquired:", $"{helperName}", "OK");
                                }
                            }
                        
                            List<ChatLog>? allMessages = new List<ChatLog>();

                            foreach (ChatLog chatIndex in chatList)
                            {
                                List<String> allAddedStuff = new List<String>();

                                if (chatIndex.TicketId == ticketId && chatIndex.Text != null)
                                {
                                    allMessages.Add(chatIndex);
                                    allAddedStuff.Add($"{chatIndex.Id}");
                                }
                                //    await DisplayAlert("Successfully Added Chat:", $"{allAddedStuff.ToString()}", "OK");
                            }

                            //  await DisplayAlert("Got through all the messages of this ticket:", $"Wooo", "OK");

                            // Don't even bother if there are no messages in the list we got for that ticket.
                            if (allMessages.Count() > 0 && allMessages != null)
                            {
                             //   await DisplayAlert("allMessages count is more than 0: ", $"Woo", "OK");

                                ChatLog latestMessage = allMessages.OrderByDescending(x => x.Time).FirstOrDefault();

                             //   await DisplayAlert("Latest Message Time Check", $"{latestMessage.Time:g}", "OK");
                                displayTime = (DateTime)latestMessage.Time;
                                displayText = latestMessage.Text;

                                
                                if (latestMessage.IsMom == "true      ") // mom's chat.
                                {
                                    displayText = "You: " + displayText;
                                  //  await DisplayAlert("Was a message by Mom:", $"True", "OK");
                                }
                            }
                            
                            String timeFixed = displayTime.ToString("g");
                            Accessibility a = Accessibility.getAccessibilitySettings();

                            // await DisplayAlert("Got to the create a new Chat Item thing", $"Yepperonies", "OK");
                            /*
                              UserNameLabels.FontSize = Math.Min(Math.Max(10, a.fontsize + 5), 20);
                              UserMessageLabels.FontSize = Math.Min(Math.Max(10, a.fontsize - 1), 18);
                              UserTimeLabels.FontSize = Math.Min(Math.Max(9, a.fontsize - 2), 16); 
                            */
                            ChatItem newChatItem = new ChatItem {
                                Name = helperName,
                                Time = $"{timeFixed}",
                                Text = $"{displayText}",
                                TicketId = ticketId, // Assign the ticketId
                                SenderFontSize = Math.Min(Math.Max(10, a.fontsize), 30), // Set sender font size
                                MessageFontSize = Math.Min(Math.Max(10, a.fontsize), 30), // Set message font size
                                TimeFontSize = Math.Min(Math.Max(10, a.fontsize), 30) // Set message font size
                            };



                            // await DisplayAlert("Item:", $"Name: {newChatItem.Name}, Time: {newChatItem.Time}, Text: {newChatItem.Text}", "OK");

                            Chats.Add(newChatItem);
                            
                            // Chats.Add(new ChatItem { Name = helperName, Time = displayTime.ToString(), Text = displayText});
                            //   await DisplayAlert("Added New Item:", $"Complete", "OK");
                        }
                    }
                    //await DisplayAlert("Chats Length:", $"{Chats.Count()}", "OK");
                   // await Navigation.PushAsync(new MomChatLogs(_momAccountID));
                    return;
                    

                }
                catch (Exception ex)
                {
                    // Handle errors (e.g., show a default value or error message)
                    await DisplayAlert("Error", $"Failed to load any chats: {ex.Message}", "OK");
                    return;
                }
            }
           
        }

        private async void OnChatSelected(object sender, SelectionChangedEventArgs e)
        {
            
            if (e.CurrentSelection.FirstOrDefault() is ChatItem selectedChat)
            {
                // Navigate to MomChatLogs page, passing the ticketID
                await Navigation.PushAsync(new MomChatLogs(selectedChat.TicketId));

                // Deselect the item to allow re-selection
                ((CollectionView)sender).SelectedItem = null;
            }
            
            /*
            if (e.CurrentSelection.FirstOrDefault() is ChatItem selectedChat)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
                        // Get entries from Ticket table in DB.
                        HttpResponseMessage response = await client.GetAsync($"{URL}/{"Tickets"}");
                        String json = await response.Content.ReadAsStringAsync();
                        List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json);

                        Ticket tempTicket = new Ticket(); // Find ticket.
                        foreach (Ticket t in ticketsList)
                        {
                            if (t.Id == selectedChat.TicketId)
                            {
                                tempTicket = t;
                                break;
                            }
                        }
                        int ticket_helperID = (int)tempTicket.HelperId;

                        await Navigation.PushAsync(new MomReviewPage(_momAccountID, ticket_helperID, tempTicket.Id));


                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("ERROR:", $"{ex.Message}", "OK");
                    }
                }

                // Deselect the item to allow re-selection
                ((CollectionView)sender).SelectedItem = null;
            }
            */
        }
       
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


    }

    public class ChatItem
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public string Text { get; set; }
        public int TicketId { get; set; } // Add TicketId property
        public double SenderFontSize { get; set; } // Property for sender label font size
        public double MessageFontSize { get; set; } // Property for message label font size
        public double TimeFontSize { get; set; } // Property for time label font size
    }
}