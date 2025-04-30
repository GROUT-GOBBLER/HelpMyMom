using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net.Http.Json;

using HelpMyMom_PROJ.models;
using HelpMyMom_PROJ;

using static System.Net.WebRequestMethods;



namespace UnitTesting
{
    public class UnitTest1
    {
        private HttpClient _client;

        String accounts_url = "/Accounts";
        String chatlogs_url = "/ChatLogs";
        String children_url = "/Children";
        String helpers_url = "/Helpers";
        String mothers_url = "/Mothers";
        String relationships_url = "/Relationships";
        String reports_url = "/Reports";
        String reviews_url = "/Reviews";
        String specs_url = "/Specs";
        String tickets_url = "/Tickets";


        String URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";

        public UnitTest1() 
        {
            _client = new HttpClient();
         
        }

        [Fact]
        public async Task GetAccounts()
        {

            var response = await _client.GetAsync(URL + "/Accounts");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            //     Assert.NotNull(objectChecking);
            //    Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetTicket()
        {

            var response = await _client.GetAsync(URL + "/Tickets");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            //     Assert.NotNull(objectChecking);
            //    Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));
           
        }

        [Fact]
        public async Task GetMom()
        {
            var response = await _client.GetAsync(URL + "/Mothers");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            // Assert.NotNull(objectChecking);
            //  Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));
        }

        [Fact]
        public async Task GetHelper()
        {
            var response = await _client.GetAsync(URL + "/Helpers");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            //   Assert.NotNull(objectChecking);
            //     Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetChatLogs()
        {

            var response = await _client.GetAsync(URL + "/ChatLogs");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

           // Assert.NotNull(objectChecking);
           //  Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

       [Fact]
        public async Task GetSpecs()
        {

            var response = await _client.GetAsync(URL + "/Specs");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            //  Assert.NotNull(objectChecking);
            // Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetReviews()
        {

            var response = await _client.GetAsync(URL + "/Reviews");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            // Assert.NotNull(objectChecking);
            // Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetReports()
        {

            var response = await _client.GetAsync(URL + "/Reports");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetChildren()
        {

            var response = await _client.GetAsync(URL + "/Children");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetRelationships()
        {

            var response = await _client.GetAsync(URL + "/Relationships");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

    }
}