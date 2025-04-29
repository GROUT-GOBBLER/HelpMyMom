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
using System.Timers;
using ScrollView = Microsoft.Maui.Controls.ScrollView;

namespace momUI
{
    public partial class MomChatLogs : ContentPage
    {
        string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
        
        
        ObservableCollection<MessageView> chatMessagesList;
        
        
        DateTime? latestMessageTime;

        String messageToSend;
        int ticketID = 0; // hard-coded value for TICKET id.
        private System.Timers.Timer aTimer;



        public MomChatLogs(int onlyUseTicketId)
        {
            InitializeComponent();
            chatMessagesList = new ObservableCollection<MessageView>();
            GetLatestMessageTime();

            ticketID = onlyUseTicketId;

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
                    List<ChatLog>? chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json1);

                    latestMessageTime = chatLogList.First<ChatLog>().Time;
                }
                catch (Exception ebert)
                {
                    return;
                }
            }
        }

        private class MessageView
        {
            public String? sender { get; set; }
            public FormattedString? messageTextContent { get; set; }
            public DateTime? timeOfSent { get; set; }
            public double SenderFontSize { get; set; } // Property for sender label font size
            public double MessageFontSize { get; set; } // Property for message label font size

        }

        protected override async void OnAppearing() // determines what the page does when it opens.
        {
            RefreshListView();
            ChatMessageListView.ItemsSource = chatMessagesList;
            aTimer.Enabled = true;


            Accessibility a = Accessibility.getAccessibilitySettings();

            TicketStatusButton.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);

            HelperChatName.FontSize = Math.Min(Math.Max(20, a.fontsize + 10), 40);

            MessageTextEntry.FontSize = Math.Min(Math.Max(10, a.fontsize), 30);

            SendChatMessage.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);
            GoBack.FontSize = Math.Min(Math.Max(15, a.fontsize + 5), 35);



            /*
            using (HttpClient client = new HttpClient())
            {
                try
                {

                    // Get entries from Ticket table in DB.
                    HttpResponseMessage response = await client.GetAsync($"{URL}/{"Tickets"}");
                    String json = await response.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json);

                    Ticket tempTicket = new Ticket(); // Find ticket.
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Id == ticketID)
                        {
                            tempTicket = t;
                            break;
                        }
                    }

                    // int momID, int helperID, int ticketID
                    int ticket_momID = (int)tempTicket.MomId;
                    int ticket_helperID = (int)tempTicket.HelperId;

                    // Goto the review page.
                    await Navigation.PushAsync(new MomReviewPage(ticket_momID, ticket_helperID, tempTicket.Id));

                }
                catch (Exception ex)
                {
                    await DisplayAlert("ERROR:", $"{ex.Message}", "OK");

                }
            }
            */

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            aTimer.Enabled = false;
        }

        async private void CheckIfReviewApproved()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Get entries from Ticket table in DB.
                    HttpResponseMessage response = await client.GetAsync($"{URL}/{"Tickets"}");

                    String json = await response.Content.ReadAsStringAsync();

                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json);

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
                    if (tempTicket.Status == "APPROVED")
                    {
                        HttpResponseMessage response2 = await client.GetAsync(URL + "/Mothers");
                        HttpResponseMessage response3 = await client.GetAsync(URL + "/Helpers");
                        HttpResponseMessage response4 = await client.GetAsync(URL + "/Children");
                        HttpResponseMessage response5 = await client.GetAsync(URL + "/Accounts");

                        String json2 = await response2.Content.ReadAsStringAsync();
                        String json3 = await response3.Content.ReadAsStringAsync();
                        String json4 = await response4.Content.ReadAsStringAsync();
                        String json5 = await response5.Content.ReadAsStringAsync();


                        List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(json2);
                        List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(json3);
                        List<Child>? childrenList = JsonConvert.DeserializeObject<List<Child>>(json4);
                        List<Account>? accountList = JsonConvert.DeserializeObject<List<Account>>(json5);

                        Account account = new Account();
                        foreach (Account index in accountList)
                        {
                            if (index.MomId == tempTicket.MomId)
                            {
                                account = index;
                                break;
                            }
                        }

                        // Send email to mom.
                        foreach (Mother index in mothersList)
                        {
                            if (index.Id == tempTicket.MomId)
                            {
                                EmailServices.SendNotifcation(index.Email,
                                    $"{index.FName} {index.LName}",
                                    $"{tempTicket.Status}",
                                    tempTicket);

                            }
                        }

                        // Only if they opt in.
                        foreach (Child index in childrenList)
                        {
                            if (index.Id == tempTicket.ChildId)
                            {
                                string[]? notifSettings = null;
                                if (index.Notifs != null)
                                {
                                    notifSettings = index.Notifs.Split(",");
                                }

                                if (notifSettings != null && notifSettings.Length == 5)
                                {
                                    bool shouldSendChild = bool.Parse(notifSettings[4].ToLower());
                                    if (shouldSendChild)
                                    {
                                        EmailServices.SendNotifcation(index.Email, $"{index.FName} {index.LName}", tempTicket.Status, tempTicket);
                                    }
                                }
                                else //If there are no settings, assume "true"
                                {
                                    EmailServices.SendNotifcation(index.Email, $"{index.FName} {index.LName}", tempTicket.Status, tempTicket);
                                }

                            }
                        }
                        // For the helper
                        foreach (Helper index in helpersList)
                        {
                            if (index.Id == tempTicket.HelperId)
                            {
                                EmailServices.SendNotifcation(index.Email,
                                    $"{index.FName} {index.LName}",
                                    $"{tempTicket.Status}",
                                    tempTicket);
                            }
                        }

                        // int momID, int helperID, int ticketID
                        int ticket_momID = (int)tempTicket.MomId;
                        int ticket_helperID = (int)tempTicket.HelperId;

                        // Goto the review page.
                        await Navigation.PushAsync(new MomReviewPage(ticket_momID, ticket_helperID, tempTicket.Id));
                    }
                    else
                    {
                        return;
                    }

                }
                catch (Exception ex)
                {
                    await DisplayAlert("ERROR:", $"{ex.Message}", "OK");

                }
            }
        }

        async private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            RefreshListView();

            CheckIfReviewApproved();

            UpdateTicketButtonStatus();

        }

        private FormattedString CreateFormattedMessage(string messageText)
        {
            var formattedString = new FormattedString();

            var urlRegex = new System.Text.RegularExpressions.Regex(@"(https?://[^\s]+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            var matches = urlRegex.Matches(messageText);
            int lastIndex = 0;

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                // text before the URL
                if (match.Index > lastIndex)
                {
                    var textSpan = new Span
                    {
                        Text = messageText.Substring(lastIndex, match.Index - lastIndex)
                    };
                    formattedString.Spans.Add(textSpan);
                }

                // Make the URL clickable.
                var urlSpan = new Span
                {
                    Text = match.Value,
                    TextColor = Colors.Blue,
                    TextDecorations = TextDecorations.Underline
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) => await OpenUrl(match.Value);
                urlSpan.GestureRecognizers.Add(tapGesture);
                formattedString.Spans.Add(urlSpan);

                lastIndex = match.Index + match.Length;
            }

            // Add any remaining text after the last URL
            if (lastIndex < messageText.Length)
            {
                var remainingSpan = new Span
                {
                    Text = messageText.Substring(lastIndex)
                };
                formattedString.Spans.Add(remainingSpan);
            }

            // Not sure what this does
            if (formattedString.Spans.Count == 0)
            {
               formattedString.Spans.Add(new Span { Text = messageText });
            }

            return formattedString;
        }


        private async Task OpenUrl(string url)

        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                // Opens the link when clicked. Not sure if we should make this open differently?
                await Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR:", $"Failed to launch URL: {ex.Message}", "OK");
            }
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

                    HelperChatName.Text = helperName + "'s Profile";

                    // Ensure only chats from the current ticket.
                    foreach (ChatLog cl in chatLogList)
                    {
                        if (cl.TicketId == ticketID && (cl.Time > latestMessageTime))
                        {
                            latestMessageTime = cl.Time;
                            Accessibility a = Accessibility.getAccessibilitySettings();



                            if (cl.IsMom == "true      ") // mom's chat.
                            {
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    MessageView tempMessageView = new MessageView();
                                    tempMessageView.sender = momName;
                                    tempMessageView.messageTextContent = CreateFormattedMessage(cl.Text ?? "");
                                    tempMessageView.timeOfSent = cl.Time;
                                    tempMessageView.SenderFontSize = Math.Min(Math.Max(10, a.fontsize), 30);
                                    tempMessageView.MessageFontSize = Math.Min(Math.Max(10, a.fontsize), 30);
                                    chatMessagesList.Add(tempMessageView);
                                });
                                
                            }
                            else if (cl.IsMom == "false     ")// helper's chat.
                            {
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    MessageView tempMessageView = new MessageView();
                                    tempMessageView.sender = helperName;
                                    tempMessageView.messageTextContent = CreateFormattedMessage(cl.Text ?? "");
                                    tempMessageView.timeOfSent = cl.Time;
                                    tempMessageView.SenderFontSize = Math.Min(Math.Max(10, a.fontsize), 30);
                                    tempMessageView.MessageFontSize = Math.Min(Math.Max(10, a.fontsize), 30);
                                    chatMessagesList.Add(tempMessageView);
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendChatMessage.Text = $"EXCEPTION OCCURED {ex.Message}";
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

                    // Create new message.
                    ChatLog newChatLog = new ChatLog();
                    int length = chatLogList.Count();
                    newChatLog.Id = chatLogList[length - 1].Id + 1;
                    newChatLog.TicketId = ticketID;
                    newChatLog.Time = DateTime.Now;
                    newChatLog.IsMom = "true      "; // FOR HELPER. BOHAN, YOU NEED TO CHANGE THIS TO TRUE BECAUSE YOU'RE DOING THE MOM!
                    newChatLog.Text = messageToSend;
                    newChatLog.Ticket = null; // might NEED TO CHANGE THIS AT SOME POINT!


                    HttpResponseMessage createResponse = await client.PostAsJsonAsync($"{URL}/{"Chatlogs"}", newChatLog); // post new message.

                    if (createResponse.IsSuccessStatusCode)
                    {
                        RefreshListView();
                        SendChatMessage.Text = "Message added successfully.";
                        
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (chatMessagesList.Any()) // Scrolls every time a chat is sent
                            {
                                ChatMessageListView.ScrollTo(chatMessagesList.Last(), ScrollToPosition.End, true);
                            }
                        });
                    }
                    else
                    {
                        SendChatMessage.Text = "Message was not added.";
                    }

                    SendChatMessage.Text = $"\nCurrent chat logs as of {DateTime.Now.ToString()}";
                }
                catch (Exception ex)
                {
                    SendChatMessage.Text = $"EXCEPTION OCCURED {ex.Message}";
                }
            }

            MessageTextEntry.Text = null;
            messageToSend = null;
        }

        /*
        bool IsAtBottom() // Checks if the user is already near the bottom. If the user is scrolling up it doesnt do this.
        {
            var scrollView = ChatMessageListView.Parent as ScrollView;
            return scrollView.ScrollY >= scrollView.ContentSize.Height - scrollView.Height - 50; // 50-pixel threshold
        }
        */

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnClickedHelperProfile(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {

                    // Get entries from Ticket table in DB.
                    HttpResponseMessage response = await client.GetAsync($"{URL}/{"Tickets"}");
                    String json = await response.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json);

                    Ticket tempTicket = new Ticket(); // Find ticket.
                    foreach (Ticket t in ticketsList)
                    {
                        if (t.Id == ticketID)
                        {
                            tempTicket = t;
                            break;
                        }
                    }

                    // int momID, int helperID, int ticketID
                    int ticket_momID = (int)tempTicket.MomId;
                    int ticket_helperID = (int)tempTicket.HelperId;

                    // Open Helper Profile
                    await Navigation.PushAsync(
                        new MomLookAtHelperProfile(ticket_momID, ticket_helperID, tempTicket.Id)
                    );
                }
                catch (Exception ex)
                {
                    await DisplayAlert("ERROR:", $"{ex.Message}", "OK");

                }
            }
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
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json);

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
                        tempTicket.Status = "COMPLETED";
                    }
                    else if (tempTicket.Status == "COMPLETED")
                    {
                        await DisplayAlert("UNAVAILABLE", "Ticket has not been approved by Helper, cannot access review!", "OK");
                        return;
                    }
                    else
                    {
                        await DisplayAlert("ERROR!", "Ticket is not in-progress, cannot be set to complete!", "OK");
                        return;
                    }

                    string buttonStatus = (string)tempTicket.Status;

                    HttpResponseMessage updateTicketStatusResponse = await client.PutAsJsonAsync(
                           $"{URL}/{"Tickets"}/{tempTicket.Id}",
                           tempTicket);


                    if (updateTicketStatusResponse.IsSuccessStatusCode)
                    {
                        UpdateTicketButtonStatus();
                        // await DisplayAlert("Ticket Completed!", "Wooo", "OK");
                        return;
                        
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

        /*
        public class ChatItem
        {
            public string? Name { get; set; }
            public string? Time { get; set; }
            public string? Text { get; set; }
            public int? TicketId { get; set; }
        }

        */

    }
}