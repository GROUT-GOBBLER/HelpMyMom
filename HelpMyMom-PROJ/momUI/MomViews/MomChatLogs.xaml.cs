using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Threading.Tasks;
using System.Data;

namespace momUI
{
    public partial class MomChatLogs : ContentPage
    {

        string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
        ObservableCollection<MessageView> chatMessagesList;
        String messageToSend;
        int ticketID = 0; // hard-coded value for TICKET id.

        public MomChatLogs(int onlyUseTicketID)
        {
            InitializeComponent();

            ticketID = onlyUseTicketID;


            // BindingContext = this;
            ChatMessageListView.ItemsSource = chatMessagesList;
            chatMessagesList = new ObservableCollection<MessageView>();

           // RefreshChat();
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
                            MessageView tempMessageView = new MessageView();
                            tempMessageView.messageTextContent = cl.Text;
                            if (cl.IsMom == "true      ") // mom's chat.
                            {
                                tempMessageView.sender = momName;
                            }
                            else // helper's chat.
                            {
                                tempMessageView.sender = helperName;
                            }
                            chatMessagesList.Add(tempMessageView);
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
            if (string.IsNullOrWhiteSpace(messageToSend))
            {
                return;
            }

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
    

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private void OnSettingsClicked(object sender, EventArgs e)
        {
            // Add settings functionality here
        }

        private async void OnStatusCompleteClickedMom(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                 
                    // Get entries from Ticket table in DB.
                    HttpResponseMessage response = await client.GetAsync($"{URL}/{"Tickets"}");
                    String json = await response.Content.ReadAsStringAsync();
                    List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json);

                    Ticket tempTicket = new Ticket(); // Find ticket.
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Id == ticketID)
                        {
                            tempTicket = t;
                            break;
                        }
                    }
                    // Valid statuses: NEW, ASSIGNED, INPROGRESS, COMPLETED, APPROVED
                    if (tempTicket.Status == "COMPLETED")
                    {
                        tempTicket.Status = "APPROVED";
                    }
                    else
                    {
                        await DisplayAlert("ERROR!", "Ticket is not complete, cannot be approved!", "OK");
                        return;
                    }

                        string buttonStatus = (string)tempTicket.Status;

                    HttpResponseMessage updateTicketStatusResponse = await client.PutAsJsonAsync(
                           $"{URL}/{"Tickets"}/{tempTicket.Id}",
                           tempTicket);


                    if (updateTicketStatusResponse.IsSuccessStatusCode)
                    {
                        UpdateTicketButtonStatus();
                        await DisplayAlert("Ticket Completed!", "Wooo", "OK");
                        // int momID, int helperID, int ticketID
                        int ticket_momID = (int)tempTicket.MomId;
                        int ticket_helperID = (int)tempTicket.HelperId;
                        await Navigation.PushAsync(new MomReviewPage(ticket_momID, ticket_helperID, tempTicket.Id));
                        // await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", $"Failed to update ticket status: {updateTicketStatusResponse.StatusCode}", "OK");
                        return;
                    }


                }
                catch (Exception ex)
                {
                    await DisplayAlert("ERROR:", $"{ex.Message}", "OK");
                    UpdateTicketButtonStatus();
                    
                }
            }

        }

        async private void UpdateTicketButtonStatus()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Get entries from Ticket table in DB.
                    HttpResponseMessage response = await client.GetAsync($"{URL}/{"Tickets"}");
                    String json = await response.Content.ReadAsStringAsync();
                    List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json);

                    Ticket tempTicket = new Ticket(); // Find ticket.
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Id == ticketID)
                        {
                            tempTicket = t;
                            break;
                        }
                    }
                 
                    // Valid statuses: NEW, ASSIGNED, INPROGRESS, COMPLETED, APPROVED
                    if (tempTicket.Status == "INPROGRESS")
                    {
                        TicketStatusButton.Text = "ONGOING";
                        TicketStatusButton.BackgroundColor = Color.FromArgb("Red");
                    }
                    else if (tempTicket.Status == "COMPLETED")
                    {
                        TicketStatusButton.Text = "DONE 1/2";
                        TicketStatusButton.BackgroundColor = Color.FromArgb("Yellow");
                    }
                    else if (tempTicket.Status == "APPROVED")
                    {
                        TicketStatusButton.Text = "DONE 2/2";
                        TicketStatusButton.BackgroundColor = Color.FromArgb("Green");
                    }
                    else
                    {
                        TicketStatusButton.Text = "N/A";
                        TicketStatusButton.BackgroundColor = Color.FromArgb("White");
                    }
                    return;
                }
                catch (Exception ex)
                {
                   await DisplayAlert("ERROR:", $"{ex.Message}", "OK");
                   return;
                }
            }
        }


        public class ChatItem
        {
            public string Name { get; set; }
            public string Time { get; set; }
            public string Text { get; set; }

            public int TicketId { get; set; }
        }


    }
}