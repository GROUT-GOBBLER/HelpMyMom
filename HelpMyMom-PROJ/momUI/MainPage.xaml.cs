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

        public MainPage()
        {
            InitializeComponent();

            chatMessages = new List<ChatLog>();
        }

        async private void GenerateChatMessages_Clicked(object sender, EventArgs e)
        {
            string URL = $"http://localhost:5124/api/ChatLogs";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"{URL}");
                    String json = await response.Content.ReadAsStringAsync();
                    List<ChatLog> chatLogList = JsonConvert.DeserializeObject<List<ChatLog>>(json);

                    GenerateChatMessages.Text = $"Current chat logs as of {DateTime.Now.ToString()}";
                    chatMessages = chatLogList;
                    ChatMessageListView.ItemsSource = chatMessages;
                }
                catch (Exception ex)
                {
                    GenerateChatMessages.Text = $"EXCEPTION OCCURED {ex}";
                }
            }
        }

        //async private void OnCounterClicked(object sender, EventArgs e)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Specs"}/{1}");

        //            string json = await response2.Content.ReadAsStringAsync();

        //            Spec Specs = JsonConvert.DeserializeObject<Spec>(json);

        //            CounterBtn.Text = $" id: {Specs.Id} String: {Specs.Name}";
        //        }
        //        catch (Exception ex)
        //        {
        //            CounterBtn.Text = $" {ex}";
        //        }
        //    }
        //}
    }
}
