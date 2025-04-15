using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
        List<String> chatMessagesAsString;
        String messageToSend;
        int ticketID = 0; // hard-coded value for TICKET id.

        public MainPage()
        {
            InitializeComponent();
            chatMessagesAsString = new List<String>();
        }

        async private void RefreshChat()
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
                                temporaryChatLogList.Add(momName + ": \n" + cl.Text);
                            }
                            else
                            {
                                temporaryChatLogList.Add(helperName + ": \n" + cl.Text);
                            }
                        }
                    }

                    chatMessagesAsString = temporaryChatLogList;
                }
                catch (Exception ex)
                {
                    RefreshChatMessages.Text = $"EXCEPTION OCCURED {ex}";
                    chatMessagesAsString = null;
                }
            }
        }

        async private void GenerateChatMessages_Clicked(object sender, EventArgs e)
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
                        if(createResponse.IsSuccessStatusCode)
                        {
                            GenerateChatMessages.Text = "Message added successfully.";
                        }
                        else
                        {
                            GenerateChatMessages.Text = "Message was not added.";
                        }

                    RefreshChat();

                    // Display new chat log entries.
                    // WILL NEED TO BE SET ON A TIMER INSTEAD OF PER BUTTON PRESS IN THE FUTURE!
                    GenerateChatMessages.Text = $"\nCurrent chat logs as of {DateTime.Now.ToString()}";
                    ChatMessageListView.ItemsSource = chatMessagesAsString;
                }
                catch (Exception ex)
                {
                    GenerateChatMessages.Text = $"EXCEPTION OCCURED {ex}";
                }
            }
            messageToSend = null;
        }

        private void MessageTextEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            messageToSend = e.NewTextValue;
        }

        private void RefreshChatMessages_Clicked(object sender, EventArgs e) // need to click the button twice for some reason.
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    RefreshChat();
                    RefreshChatMessages.Text = "Refreshed!";
                    ChatMessageListView.ItemsSource = chatMessagesAsString;
                }
                catch (Exception ex)
                {
                    RefreshChatMessages.Text = $"EXCEPTION OCCURED {ex}";
                }
            }
        }
    }
}
