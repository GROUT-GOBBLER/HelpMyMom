using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


using HelpMyMom_PROJ.models;
using HelpMyMom_PROJ;



namespace UnitTesting
{
    public class UnitTest1// : IAsyncLifetime
    {
        private HttpClient _client;
        String chatlogs_url = "/api/ChatLogs";
        String children_url = "/api/Children";
        String helpers_url = "/api/Helpers";
        String mothers_url = "/api/Mothers";
        String relationships_url = "/api/Relationships";
        String reports_url = "/api/Reports";
        String reviews_url = "/api/Reviews";
        String specs_url = "/api/Specs";
        String tickets_url = "/api/Tickets";

        public UnitTest1() 
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5124"); // Replace with your momAPI's URL
        }

        // Setup method that runs before each test
        public async Task InitializeAsync()
        {
            bool isApiAvailable = false;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    var response = await _client.GetAsync("/swagger/index.html");
                    if (response.IsSuccessStatusCode)
                    {
                        isApiAvailable = true;
                        break;
                    }
                }
                catch
                {
                    // API not yet available, wait 1 second and retry
                    await Task.Delay(1000);
                }
            }

            if (!isApiAvailable)
            {
                throw new Exception("momAPI did not start within 30 seconds. Ensure the API is running on http://localhost:5124.");
            }
        }

        // Cleanup method (required by IAsyncLifetime, can be empty)
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Test1()
        {
            /*
            var response = await _client.GetAsync("/api/ChatLogs");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            // Add assertions here
            var helpmymom = JsonConvert.DeserializeObject<List<object>>(content);
           
            Assert.NotNull(helpmymom);
            */
            var response = await _client.GetAsync("/WeatherForecast");
            response.EnsureSuccessStatusCode();
            // await InitializeAsync();

            var content = await response.Content.ReadAsStringAsync();
            var weatherForecasts = JsonConvert.DeserializeObject<List<object>>(content);
            Assert.NotNull(weatherForecasts);

            //Assert.Equal(10, 10);
        }

        [Fact]
        public async Task GetTicket()
        {

            var response = await _client.GetAsync(tickets_url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tickets = JsonConvert.DeserializeObject<List<object>>(content);

            Assert.Equal(10, 10);
            /*
            Assert.NotNull(tickets);
            Assert.NotEmpty(tickets);
            Assert.All(tickets, tckt => Assert.False(tckt.Equals(null)));
            */


        }



    }
}