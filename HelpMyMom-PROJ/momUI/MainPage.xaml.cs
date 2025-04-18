using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Timers;
using momUI.models;
using Newtonsoft.Json;


namespace momUI
{
    public partial class MainPage : ContentPage
    {
        string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
        ObservableCollection<MessageView> chatMessagesList;
        DateTime? latestMessageTime;

        String messageToSend;
        int ticketID = 0; // hard-coded value for TICKET id.
        private  System.Timers.Timer aTimer;

        public MainPage()
        {
            InitializeComponent();
            chatMessagesList = new ObservableCollection<MessageView>();
            GetLatestMessageTime();

            aTimer = new System.Timers.Timer(5000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
        }

        async private void GetLatestMessageTime()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Get entries from chat log table from DB.
                    HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Chatlogs"}");
                    String json1 = await response1.Content.ReadAsStringAsync();
                    List<ChatLog> chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json1);

                    latestMessageTime = chatLogList.First<ChatLog>().Time;
                }
                catch(Exception ebert)
                {
                    return;
                }
            }
        }

        private class MessageView
        { 
            public String? sender { get; set; }
            public String? messageTextContent { get; set; }
            public DateTime? timeOfSent { get; set; }
        }

        protected override async void OnAppearing() // determines what the page does when it opens.
        {
            RefreshListView();
            ChatMessageListView.ItemsSource = chatMessagesList;
            aTimer.Enabled = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            aTimer.Enabled = false;
        }

        async private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            RefreshListView();
        }

        async private void RefreshListView()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Get entries from chat log table from DB.
                    HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Chatlogs"}");
                    String json1 = await response1.Content.ReadAsStringAsync();
                    List<ChatLog> chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json1);

                    // Get entries from Mother table in DB.
                    HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Mothers"}");
                    String json2 = await response2.Content.ReadAsStringAsync();
                    List<Mother> mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);

                    // Get entries from Helper table in DB.
                    HttpResponseMessage response3 = await client.GetAsync($"{URL}/{"Helpers"}");
                    String json3 = await response3.Content.ReadAsStringAsync();
                    List<Helper> helpersList = JsonConvert.DeserializeObject<List<Helper>>(json3);

                    // Get entries from Ticket table in DB.
                    HttpResponseMessage response4 = await client.GetAsync($"{URL}/{"Tickets"}");
                    String json4 = await response4.Content.ReadAsStringAsync();
                    List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json4);

                    // Get Mom name and Helper name.
                    String momName = "", helperName = "";
                    int? momID = 0, helperID = 0;

                    Ticket tempTicket = new Ticket(); // Find ticket.
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Id == ticketID)
                        {
                            tempTicket = t;
                            break;
                        }
                    }

                    momID = tempTicket.MomId; // Get momID and helperID.
                    helperID = tempTicket.HelperId;

                    Mother tempMother = new Mother(); // get mom and helper instance.
                    Helper tempHelper = new Helper();

                    foreach (Mother m in mothersList)
                    {
                        if (m.Id == momID)
                        {
                            tempMother = m;
                            break;
                        }
                    }

                    foreach (Helper h in helpersList)
                    {
                        if (h.Id == helperID)
                        {
                            tempHelper = h;
                            break;
                        }
                    }

                    momName = tempMother.FName + " " + tempMother.LName;
                    helperName = tempHelper.FName + " " + tempHelper.LName;


                    // Ensure only chats from the current ticket.
                    foreach (ChatLog cl in chatLogList)
                    {
                        if (cl.TicketId == ticketID && (cl.Time > latestMessageTime))
                        {
                            latestMessageTime = cl.Time;

                            if (cl.IsMom == "true      ") // mom's chat.
                            {
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    MessageView tempMessageView = new MessageView();
                                    tempMessageView.sender = momName;
                                    tempMessageView.messageTextContent = cl.Text;
                                    tempMessageView.timeOfSent = cl.Time;
                                    chatMessagesList.Add(tempMessageView);
                                });
                            }
                            else if (cl.IsMom == "false     ")// helper's chat.
                            {
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    MessageView tempMessageView = new MessageView();
                                    tempMessageView.sender = helperName;
                                    tempMessageView.messageTextContent = cl.Text;
                                    tempMessageView.timeOfSent = cl.Time;
                                    chatMessagesList.Add(tempMessageView);
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendChatMessage.Text = $"EXCEPTION OCCURED {ex}";
                }
            }
        }

        private void MessageTextEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            messageToSend = e.NewTextValue;
        }

        async private void SendChatMessage_Clicked(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Get entries from chat log table from DB.
                    HttpResponseMessage response = await client.GetAsync($"{URL}/{"Chatlogs"}");
                    String json = await response.Content.ReadAsStringAsync();
                    List<ChatLog> chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json);

                    // Create new message.
                    ChatLog newChatLog = new ChatLog();
                    int length = chatLogList.Count();
                    newChatLog.Id = chatLogList[length - 1].Id + 1;
                    newChatLog.TicketId = ticketID;
                    newChatLog.Time = DateTime.Now;
                    newChatLog.IsMom = "false      "; // FOR HELPER. BOHAN, YOU NEED TO CHANGE THIS TO TRUE BECAUSE YOU'RE DOING THE MOM!
                    newChatLog.Text = messageToSend;
                    newChatLog.Image = null; 
                    newChatLog.Ticket = null; 

                    HttpResponseMessage createResponse = await client.PostAsJsonAsync($"{URL}/{"Chatlogs"}", newChatLog); // post new message.

                    if (createResponse.IsSuccessStatusCode)
                    {
                        RefreshListView();
                        SendChatMessage.Text = "Message added successfully.";
                    }
                    else
                    {
                        SendChatMessage.Text = "Message was not added.";
                    }

                    SendChatMessage.Text = $"\nCurrent chat logs as of {DateTime.Now.ToString()}";
                }
                catch (Exception ex)
                {
                    SendChatMessage.Text = $"EXCEPTION OCCURED {ex}";
                }
            }

            MessageTextEntry.Text = null;
            messageToSend = null;
        }
    }
}
