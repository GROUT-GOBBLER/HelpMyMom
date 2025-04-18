using System;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Threading.Tasks;
using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
        ObservableCollection<MessageView> chatMessagesList;
        String messageToSend;
        int ticketID = 0; // hard-coded value for TICKET id.

        public MainPage()
        {
            InitializeComponent();
            ChatMessageListView.ItemsSource = chatMessagesList;
            chatMessagesList = new ObservableCollection<MessageView>();
        }

        private class MessageView
        {
            public String? sender { get; set; }
            public String? messageTextContent { get; set; }
        }

        protected override async void OnAppearing() // determines what the page does when it opens.
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
                    List<String> temporaryChatLogList = new List<String>();
                    foreach (ChatLog cl in chatLogList)
                    {
                        if (cl.TicketId == ticketID)
                        {
                            if (cl.IsMom == "true      ") // mom's chat.
                            {
                                MessageView tempMessageView = new MessageView();
                                tempMessageView.sender = momName;
                                tempMessageView.messageTextContent = cl.Text;
                                chatMessagesList.Add(tempMessageView);
                            }
                            else // helper's chat.
                            {
                                MessageView tempMessageView = new MessageView();
                                tempMessageView.sender = helperName;
                                tempMessageView.messageTextContent = cl.Text;
                                chatMessagesList.Add(tempMessageView);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    RefreshChatMessages.Text = $"EXCEPTION OCCURED {ex}";
                }
            }

            ChatMessageListView.ItemsSource = chatMessagesList;
        }

        private void MessageTextEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            messageToSend = e.NewTextValue;
        }

        async private void RefreshChatMessages_Clicked(object sender, EventArgs e) 
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
                    List<String> temporaryChatLogList = new List<String>();
                    foreach (ChatLog cl in chatLogList)
                    {
                        if (cl.TicketId == ticketID)
                        {
                            if (cl.IsMom == "true      ") // mom's chat.
                            {
                                MessageView tempMessageView = new MessageView();
                                tempMessageView.sender = momName;
                                tempMessageView.messageTextContent = cl.Text;
                                chatMessagesList.Add(tempMessageView);
                            }
                            else // helper's chat.
                            {
                                MessageView tempMessageView = new MessageView();
                                tempMessageView.sender = helperName;
                                tempMessageView.messageTextContent = cl.Text;
                                chatMessagesList.Add(tempMessageView);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    RefreshChatMessages.Text = $"EXCEPTION OCCURED {ex}";
                }
            }

            ChatMessageListView.ItemsSource = chatMessagesList;
            RefreshChatMessages.Text = "Refreshed!";
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
                    newChatLog.IsMom = null; // WILL NEED TO CHANGE THIS AT SOME POINT!
                    newChatLog.Text = messageToSend;
                    newChatLog.Image = null; // WILL NEED TO CHANGE THIS AT SOME POINT!
                    newChatLog.Ticket = null; // might NEED TO CHANGE THIS AT SOME POINT!

                    HttpResponseMessage createResponse = await client.PostAsJsonAsync($"{URL}/{"Chatlogs"}", newChatLog); // post new message.
                    if (createResponse.IsSuccessStatusCode)
                    {
                        SendChatMessage.Text = "Message added successfully.";
                    }
                    else
                    {
                        SendChatMessage.Text = "Message was not added.";
                    }

                    // Get entries from chat log table from DB.
                    HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Chatlogs"}");
                    String json1 = await response1.Content.ReadAsStringAsync();
                    chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json1);

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
                    List<String> temporaryChatLogList = new List<String>();
                    foreach (ChatLog cl in chatLogList)
                    {
                        if (cl.TicketId == ticketID)
                        {
                            if (cl.IsMom == "true      ") // mom's chat.
                            {
                                MessageView tempMessageView = new MessageView();
                                tempMessageView.sender = momName;
                                tempMessageView.messageTextContent = cl.Text;
                                chatMessagesList.Add(tempMessageView);
                            }
                            else // helper's chat.
                            {
                                MessageView tempMessageView = new MessageView();
                                tempMessageView.sender = helperName;
                                tempMessageView.messageTextContent = cl.Text;
                                chatMessagesList.Add(tempMessageView);
                            }
                        }
                    }

                    SendChatMessage.Text = $"\nCurrent chat logs as of {DateTime.Now.ToString()}";
                }
                catch (Exception ex)
                {
                    SendChatMessage.Text = $"EXCEPTION OCCURED {ex}";
                }
            }

            ChatMessageListView.ItemsSource = chatMessagesList;
            MessageTextEntry.Text = null;
            messageToSend = null;
        }
    }
}
