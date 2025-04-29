using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Timers;
using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class MessagingView : ContentPage
{
    // VARIABLE CREATION.
    string URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String messageToSend = "";
    String motherName;
    String helperName;
    int? ticketID;
    
    Helper masterHelper;
    Account masterAccount;
    Ticket masterTicket;

    ObservableCollection<MessageView> chatMessagesList;
    Accessibility fontSizes;

    DateTime? latestMessageTime;
    private System.Timers.Timer aTimer;

    // CLASS DEFINITIONS.
    public MessagingView(int? t_id, Helper h, Account a)
	{
        InitializeComponent();
        chatMessagesList = new ObservableCollection<MessageView>();
        GetLatestMessageTime();

        ticketID = t_id;
        motherName = "";
        helperName = h.FName + " " + h.LName;

        masterHelper = h;
        masterAccount = a;
        masterTicket = new Ticket();

        fontSizes = Accessibility.getAccessibilitySettings();

        aTimer = new System.Timers.Timer(5000);
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;

        TicketApprovedButton.IsEnabled = false;
    }

    private class MessageView
    {
        public String? sender { get; set; }
        public FormattedString? messageTextContent { get; set; }
        public DateTime? timeOfSent { get; set; }
        public double MessageSenderFontSize { get; set; }
        public double MessageOtherFontSize { get; set; }
    }

    // PAGE OPEN / CLOSE.
    protected override void OnAppearing() // determines what the page does when it opens.
    {
        base.OnAppearing();

        GetMomName();
        SetFontSizes();
        TicketApprovedButtonStateDetermination(masterTicket);
        RefreshListView();

        ChatMessageListView.ItemsSource = chatMessagesList;

        aTimer.Enabled = true;
    }

    protected override void OnDisappearing() // determines what the page does when it closes.
    {
        base.OnDisappearing();
        aTimer.Enabled = false;
    }

    // LOCAL METHODS.
    async private void RefreshListView()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TicketApprovedButtonStateDetermination(masterTicket);
        });

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get entries from chat log table from DB.
                HttpResponseMessage chatlogResponse = await client.GetAsync($"{URL}/{"Chatlogs"}");
                String chatlogJSON = await chatlogResponse.Content.ReadAsStringAsync();
                List<ChatLog>? chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(chatlogJSON);

                // Get entries from Ticket table in DB.
                HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
                String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
                List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON);

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

                                    tempMessageView.sender = motherName;
                                    tempMessageView.messageTextContent = CreateFormattedMessage(cl.Text ?? "");
                                    tempMessageView.timeOfSent = cl.Time;
                                    tempMessageView.MessageSenderFontSize = fontSizes.fontsize + 10;
                                    tempMessageView.MessageOtherFontSize = fontSizes.fontsize;

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

                                    tempMessageView.MessageSenderFontSize = fontSizes.fontsize + 10;
                                    tempMessageView.MessageOtherFontSize = fontSizes.fontsize;

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
                await DisplayAlert("Exception", $"Exception occurred ... {ex}", "Ok.");
            }
        }
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

                if (chatLogList != null)
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

    private void OnTimedEvent(Object? source, ElapsedEventArgs e) // what the timer does ever x seconds.
    {
        RefreshListView();
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

    private void SetFontSizes()
    {
        MomNameTextBox.FontSize = fontSizes.fontsize + 20;
        TicketApprovedButton.FontSize = fontSizes.fontsize + 5;
        MessageTextEntry.FontSize = fontSizes.fontsize;
        SendChatMessage.FontSize = fontSizes.fontsize + 5;
    }    

    private async void GetMomName()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get entries from Ticket table in DB.
                HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
                    String ticketJSON = await ticketResponse.Content.ReadAsStringAsync();
                    List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJSON);

                // Get entries from Mother table in DB.
                HttpResponseMessage motherResponse = await client.GetAsync($"{URL}/{"Mothers"}");
                    String motherJSON = await motherResponse.Content.ReadAsStringAsync();
                    List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(motherJSON);

                // Make some temporary variables.
                Mother tempMother = new Mother();
                
                if(ticketsList != null) // get Ticket object from ticket id.
                {
                    foreach(Ticket t in ticketsList)
                    {
                        if(t.Id == ticketID)
                        {
                            masterTicket = t;
                            break;
                        }
                    }

                    if(masterTicket.Id != ticketID) { await DisplayAlert("TicketNotFound", $"Error! Could not find ticket with ID {ticketID}.", "Ok."); }
                }
                else { await DisplayAlert("NoTicketsFound", "Error! Failed to find any tickets.", "Ok."); }

                
                if(mothersList != null) // get Mother object from ticket.
                {
                    foreach(Mother m in mothersList)
                    {
                        if(m.Id == masterTicket.MomId)
                        {
                            tempMother = m;
                            break;
                        }
                    }

                    if(tempMother.Id != masterTicket.MomId) { await DisplayAlert("MotherNotFound", $"Error! Could not find mother with ID {masterTicket.MomId}.", "Ok."); }
                }
                else { await DisplayAlert("NoMothersFound", "Error! Failed to find any mothers.", "Ok."); }

                motherName = tempMother.FName + " " + tempMother.LName;
            }
            catch (Exception e)
            {
                await DisplayAlert("Exception", $"Exception occurred ... {e}", "Ok.");
            }
        }

        MomNameTextBox.Text = motherName;
    }

    private FormattedString CreateFormattedMessage(String messageText)
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

    // XAML ACTIONS.
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
                HttpResponseMessage chatlogResponse = await client.GetAsync($"{URL}/{"Chatlogs"}");
                String chatlogJSON = await chatlogResponse.Content.ReadAsStringAsync();
                List<ChatLog>? chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(chatlogJSON);

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

                if (createResponse.IsSuccessStatusCode) { RefreshListView(); }
                else { await DisplayAlert("MessageFailedToSend", "Error! The message could not be sent.", "Ok."); }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", $"An exception occurred ... {ex}", "Ok.");
            }
        }

        MessageTextEntry.Text = null;
        messageToSend = "";
    }

    async private void TicketApprovedButton_Clicked(object sender, EventArgs e)
    {
        TicketApprovedButton.IsEnabled = false;

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
                        Application.Current.MainPage = new NavigationPage(new HelperWriteReport(ticketID, masterHelper, masterAccount));
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

        TicketApprovedButton.IsEnabled = true;
    }
}