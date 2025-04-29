using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class HelperCurrentChats : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    
    List<ChatLogView> CHATLOG_LIST;
    Accessibility fontSize;
    Helper masterHelper;
    Account masterAccount;

    public HelperCurrentChats(Account a, Helper h)
	{
        InitializeComponent();
        
        masterHelper = h;
        masterAccount = a;

        CHATLOG_LIST = new List<ChatLogView>();
        fontSize = Accessibility.getAccessibilitySettings();
    }

	private class ChatLogView
	{
		public int? ticketID { get; set; }
		public String? momName { get; set; }
		public String? lastMessageSent { get; set; }
		public String? lastMessageTime { get; set; }
        public double MomNameFontSize { get; set; }
        public double LatestMessageTimeFontSize { get; set; }
        public double LatestMessageTextFontSize { get; set; }
	}

	protected override async void OnAppearing() // determines what the page does when it opens.
	{
        
        base.OnAppearing();
        CurrentChatsLabel.FontSize = fontSize.fontsize;

        CHATLOG_LIST = new List<ChatLogView>();

        using (HttpClient client = new HttpClient())
		{
			// Get Mom, chatlog, ticket.
			HttpResponseMessage motherResponse = await client.GetAsync($"{URL}/{"Mothers"}");
				String motherJson = await motherResponse.Content.ReadAsStringAsync();
				List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(motherJson); // mothersList.
			HttpResponseMessage chatlogResponse = await client.GetAsync($"{URL}/{"Chatlogs"}");
				String chatlogJson = await chatlogResponse.Content.ReadAsStringAsync();
				List<ChatLog>? chatLogsList = JsonConvert.DeserializeObject<List<ChatLog>>(chatlogJson); // chatLogsList.
			HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
				String ticketJson = await ticketResponse.Content.ReadAsStringAsync();
				List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJson); // ticketsList.

			if(ticketsList != null)
			{
                foreach (Ticket t in ticketsList) // For each of the helper's tickets...
                {
                    if (t.HelperId == masterHelper.Id && (t.Status == "INPROGRESS" || t.Status == "COMPLETED"))
                    {
                        int? TESTvalue = -1;
                        Mother tempMother = new Mother(); // find the mom.
                        if(mothersList != null)
                        {
                            foreach (Mother m in mothersList)
                            {
                                if (t.MomId == m.Id)
                                {
                                    TESTvalue = t.Id;
                                    tempMother = m;
                                    break;
                                }
                            }
                        }
                        else { await DisplayAlert("MothersNotFound", "Error! Failed to find any Mothers.", "Ok."); }

                        ChatLogView tempChatLogView = new ChatLogView();
                        tempChatLogView.momName = tempMother.FName + " " + tempMother.LName;

                        if(chatLogsList != null)
                        {
                            bool found = false; // if no chats exist yet.
                            foreach (ChatLog c in chatLogsList) // For each chatLog from that ticket...
                            {
                                if (c.TicketId == t.Id)
                                {
                                    found = true;
                                    
                                    tempChatLogView.ticketID = c.TicketId;
                                    tempChatLogView.lastMessageTime = c.Time.ToString();
                                    tempChatLogView.lastMessageSent = c.Text;
                                    
                                    // setting font sizes.
                                    tempChatLogView.MomNameFontSize = fontSize.fontsize + 10;
                                    tempChatLogView.LatestMessageTimeFontSize = fontSize.fontsize;
                                    tempChatLogView.LatestMessageTextFontSize = fontSize.fontsize;
                                }
                            }

                            if (!found)
                            {
                                tempChatLogView.ticketID = TESTvalue;
                                tempChatLogView.lastMessageTime = "";
                                tempChatLogView.lastMessageSent = "";
                                tempChatLogView.MomNameFontSize = fontSize.fontsize + 10; // font size for mother's name.
                            }
                        }
                        else { await DisplayAlert("ChatlogsNotFound", "Error! Failed to find any ChatLogs.", "Ok."); }

                        CHATLOG_LIST.Add(tempChatLogView);
                    }
                }
            }
            else { await DisplayAlert("TicketsNotFound", "Error! Failed to find any tickets.", "Ok."); }

            CurrentChatsListView.ItemsSource = CHATLOG_LIST;
        }
    }

    async private void CurrentChatsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        ChatLogView tempChatLogView = (ChatLogView) CurrentChatsListView.SelectedItem;
        await Navigation.PushAsync(new MessagingView(tempChatLogView.ticketID, masterHelper, masterAccount));
    }
}