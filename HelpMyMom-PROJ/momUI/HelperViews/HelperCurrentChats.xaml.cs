using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class HelperCurrentChats : ContentPage
{
	List<ChatLogView> CHATLOG_LIST;
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String MASTERusername = "UncleBensBiggestFan";

    public HelperCurrentChats()
	{
        InitializeComponent();
		CHATLOG_LIST = new List<ChatLogView>();
    }

	private class ChatLogView
	{
		public long chatLogID { get; set; }
		public String? momName { get; set; }
		public String? lastMessageSent { get; set; }
		public DateTime? lastMessageTime { get; set; }
	}

	protected override async void OnAppearing() // determines what the page does when it opens.
	{
		using (HttpClient client = new HttpClient())
		{
			// Get account, helper, mom, chatlog, ticket.
			HttpResponseMessage accountResponse = await client.GetAsync($"{URL}/{"Accounts"}");
				String accountJson = await accountResponse.Content.ReadAsStringAsync();
				List<Account> accountsList = JsonConvert.DeserializeObject<List<Account>>(accountJson); // accountsList.
			HttpResponseMessage helperResponse = await client.GetAsync($"{URL}/{"Helpers"}");
				String helperJson = await helperResponse.Content.ReadAsStringAsync();
				List<Helper> helpersList = JsonConvert.DeserializeObject<List<Helper>>(helperJson); // helpersList.
			HttpResponseMessage motherResponse = await client.GetAsync($"{URL}/{"Mothers"}");
				String motherJson = await motherResponse.Content.ReadAsStringAsync();
				List<Mother> mothersList = JsonConvert.DeserializeObject<List<Mother>>(motherJson); // mothersList.
			HttpResponseMessage chatlogResponse = await client.GetAsync($"{URL}/{"Chatlogs"}");
				String chatlogJson = await chatlogResponse.Content.ReadAsStringAsync();
				List<ChatLog> chatLogsList = JsonConvert.DeserializeObject<List<ChatLog>>(chatlogJson); // chatLogsList.
			HttpResponseMessage ticketResponse = await client.GetAsync($"{URL}/{"Tickets"}");
				String ticketJson = await ticketResponse.Content.ReadAsStringAsync();
				List<Ticket> ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(ticketJson); // ticketsList.

			// Create temporary variables.
			Helper tempHelper = new Helper();
			int? helperID = -1;
			bool found = false;
			String chatLogMessage = "";
			String motherName = "";

			foreach (Account a in accountsList) // Find helper ID.
			{
				if (a.Username == MASTERusername)
				{
					helperID = a.HelperId;
					break;
				}
			}

			if (helperID == -1) 
			{
				await DisplayAlert("AccountNotFound", "Error! Account not found.", "Ok.");
				return;
			}

			foreach(Helper h in helpersList) // Find helper.
			{
				if(h.Id == helperID)
				{
					tempHelper = h;
					found = true;
					break;
				}
			}

			if(!found)
			{
				await DisplayAlert("HelperNotFound", "Error! Helper not found.", "Ok.");
				return;
			}

			foreach(Ticket t in ticketsList) // For each of the helper's tickets...
            {
				if (t.HelperId == helperID && t.Status != "APPROVED")
				{
					Mother tempMother = new Mother(); // find the mom.
                    foreach (Mother m in mothersList) 
					{
						if(t.MomId == m.Id)
						{
                            tempMother = m;
							break;
						}
					}

                    ChatLogView tempChatLogView = new ChatLogView();
					tempChatLogView.momName = tempMother.FName + " " + tempMother.LName;

                    foreach (ChatLog c in chatLogsList) // For each chatLog from that ticket...
                    {
						if(c.TicketId == t.Id)
						{
                            tempChatLogView.chatLogID = c.Id;
							tempChatLogView.lastMessageTime = c.Time;
                            tempChatLogView.lastMessageSent = c.Text;
                        }
					}

					CHATLOG_LIST.Add(tempChatLogView);
				}
			}

			CurrentChatsListView.ItemsSource = CHATLOG_LIST;
        }
    }

    async private void CurrentChatsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        await Navigation.PushAsync(new MessagingView());
    }
}