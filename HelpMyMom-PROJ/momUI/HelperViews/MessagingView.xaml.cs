using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Timers;
using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class MessagingView : ContentPage
{
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    ObservableCollection<MessageView> chatMessagesList;
    DateTime? latestMessageTime;

    String messageToSend = "";
    int ticketID = 0; // hard-coded value for TICKET id.
    private System.Timers.Timer aTimer;

    public MessagingView()
	{
        InitializeComponent();
        chatMessagesList = new ObservableCollection<MessageView>();
        GetLatestMessageTime();

        aTimer = new System.Timers.Timer(5000);
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;

        TicketApprovedButton.IsEnabled = false;
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
                List<ChatLog>? chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json1);

                if(chatLogList != null)
                {
                    latestMessageTime = chatLogList.First<ChatLog>().Time;
                }
            }
            catch (Exception ebert)
            {
                await DisplayAlert("ExceptionOccured", $"Exception occurred ... {ebert}", "Ok.");
            }
        }
    }

    private class MessageView
    {
        public String? sender { get; set; }
        public String? messageTextContent { get; set; }
        public DateTime? timeOfSent { get; set; }
    }

    protected override void OnAppearing() // determines what the page does when it opens.
    {
        RefreshListView();
        ChatMessageListView.ItemsSource = chatMessagesList;
        aTimer.Enabled = true;
    }

    protected override void OnDisappearing() // determines what the page does when it closes.
    {
        base.OnDisappearing();
        aTimer.Enabled = false;
    } 

    private void OnTimedEvent(Object? source, ElapsedEventArgs e) // what the timer does ever x seconds.
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
                    List<ChatLog>? chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json1);

                // Get entries from Mother table in DB.
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Mothers"}");
                    String json2 = await response2.Content.ReadAsStringAsync();
                    List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);

                // Get entries from Helper table in DB.
                HttpResponseMessage response3 = await client.GetAsync($"{URL}/{"Helpers"}");
                    String json3 = await response3.Content.ReadAsStringAsync();
                    List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(json3);

                // Get entries from Ticket table in DB.
                HttpResponseMessage response4 = await client.GetAsync($"{URL}/{"Tickets"}");
                    String json4 = await response4.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json4);

                // Get Mom name and Helper name.
                String momName = "", helperName = "";
                int? momID = 0, helperID = 0;

                Ticket tempTicket = new Ticket(); // Find ticket.
                if(ticketsList != null)
                {
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Id == ticketID)
                        {
                            tempTicket = t;
                            break;
                        }
                    }
                }
                else { await DisplayAlert("TicketsNotFound", "Failed to find any tickets.", "Ok.");  }

                if (tempTicket != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TicketApprovedButtonStateDetermination(tempTicket);
                        momID = tempTicket.MomId; // Get momID and helperID.
                        helperID = tempTicket.HelperId;
                    });
                }

                Mother tempMother = new Mother(); // get mom and helper instance.
                Helper tempHelper = new Helper();

                if(mothersList != null)
                {
                    foreach (Mother m in mothersList)
                    {
                        if (m.Id == momID)
                        {
                            tempMother = m;
                            break;
                        }
                    }
                }
                else { await DisplayAlert("MothersNotFound", "Failed to find any Mothers.", "Ok."); }

                if(helpersList != null)
                {
                    foreach (Helper h in helpersList)
                    {
                        if (h.Id == helperID)
                        {
                            tempHelper = h;
                            break;
                        }
                    }
                }
                else { await DisplayAlert("HelpersNotFound", "Failed to find any Helpers.", "Ok."); }

                momName = tempMother.FName + " " + tempMother.LName;
                helperName = tempHelper.FName + " " + tempHelper.LName;
                MomNameTextBox.Text = momName;

                // Ensure only chats from the current ticket.
                if (chatLogList != null)
                {
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
                else { await DisplayAlert("ChatLogsNotFound", "Failed to find any ChatLogs.", "Ok."); }

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
                List<ChatLog>? chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json);

                ChatLog newChatLog = new ChatLog();

                // Create new message.
                if (chatLogList != null)
                {
                    int length = chatLogList.Count;
                    newChatLog.Id = chatLogList[length - 1].Id + 1;
                    newChatLog.TicketId = ticketID;
                    newChatLog.Time = DateTime.Now;
                    newChatLog.IsMom = "false      ";
                    newChatLog.Text = messageToSend;
                    newChatLog.Ticket = null;
                }
                else { await DisplayAlert("ChatLogsNotFound", "Failed to find any ChatLogs.", "Ok."); }


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
        messageToSend = "";
    }

    private void TicketApprovedButtonStateDetermination(Ticket tick)
    {
        string? ticketStatus = tick.Status;
        TicketApprovedButton.Text = $"{ticketStatus}";

        if (ticketStatus != null && ticketStatus.Equals("COMPLETED"))
        {
            TicketApprovedButton.IsEnabled = true;
        }
        else
        {
            TicketApprovedButton.IsEnabled = false;
        }
    }

    async private void TicketApprovedButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get entries from Ticket table in DB.
                HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
                    String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON);

                Ticket tempTicket = new Ticket();
                if(ticketsList != null)
                {
                    foreach(Ticket t in ticketsList)
                    {
                        if(t.Id == ticketID)
                        {
                            tempTicket = t;
                            break;
                        }
                    }

                    if(tempTicket.Id != ticketID)
                    {
                        await DisplayAlert("TicketNotFound", "Error! Ticket not found.", "Ok.");
                    }
                }
                else { await DisplayAlert("TicketsNotFound", "Error! Failed to find any tickets.", "Ok."); }

                tempTicket.Status = "APPROVED";

                HttpResponseMessage editTicket = await client.PutAsJsonAsync($"{URL}/Tickets/{ticketID}", tempTicket);

                if (editTicket.IsSuccessStatusCode)
                {
                    if(Application.Current != null)
                    {
                        Application.Current.MainPage = new NavigationPage(new HelperWriteReport());
                    }
                    else { await DisplayAlert("NoCurrentPage", "Error! There is no current page.\nHow did this happen?", "What?"); }
                }
                else { await DisplayAlert("ResponseFailure", "Error! Failed to edit ticket status!", "Ok."); }
            }
            catch(Exception except)
            {
                await DisplayAlert("Exception", $"Error! An exception occurred ... {except}", "Ok.");
            }
        }

    }
}