using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using momUI.models;
using Newtonsoft.Json;
namespace momUI
{
    public partial class MainPage : ContentPage
    {
        List<ChatLog> chatMessages;
        String messageToSend;

        public MainPage()
        {
            InitializeComponent();

            chatMessages = new List<ChatLog>();
        }

        async private void GenerateChatMessages_Clicked(object sender, EventArgs e)
        {
            string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
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
                        newChatLog.TicketId = null; // WILL NEED TO CHANGE THIS AT SOME POINT!
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

                    // Refresh entries from chat log table.
                    response = await client.GetAsync($"{URL}/{"Chatlogs"}");
                    json = await response.Content.ReadAsStringAsync();
                    chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json);

                    // Display new chat log entries.
                    // WILL NEED TO BE SET ON A TIMER INSTEAD OF PER BUTTON PRESS IN THE FUTURE!
                    GenerateChatMessages.Text += $"\nCurrent chat logs as of {DateTime.Now.ToString()}";
                    chatMessages = chatLogList;
                    ChatMessageListView.ItemsSource = chatMessages;
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
    }
}
