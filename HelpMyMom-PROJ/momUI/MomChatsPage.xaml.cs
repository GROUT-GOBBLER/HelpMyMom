using System;
using System.Net.Http.Json;
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http;

namespace momUI
{
    public partial class MomChatsPage : ContentPage
    {

        string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

        private String accounts_url = "/Accounts";
        private String chatlogs_url = "/ChatLogs";
        private String children_url = "/Children";
        private String helpers_url = "/Helpers";
        private String mothers_url = "/Mothers";
        private String relationships_url = "/Relationships";
        private String reports_url = "/Reports";
        private String reviews_url = "/Reviews";
        private String specs_url = "/Specs";
        private String tickets_url = "/Tickets";

        public List<ChatItem> Chats { get; set; }

        public MomChatsPage()
        {
            InitializeComponent();

            // Sample chat data
            Chats = new List<ChatItem>
            {
                new ChatItem { Name = "Jennie P.", Time = "8:00am" },
                new ChatItem { Name = "Josie C.", Time = "7:30am" },
                new ChatItem { Name = "Jerry V.", Time = "7:00am" },
                new ChatItem { Name = "Jimmy G.", Time = "6:30am" },
                new ChatItem { Name = "Richard Johnson", Time = "5:00am" }
            };

            BindingContext = this;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }

    public class ChatItem
    {
        public string Name { get; set; }
        public string Time { get; set; }
    }
}