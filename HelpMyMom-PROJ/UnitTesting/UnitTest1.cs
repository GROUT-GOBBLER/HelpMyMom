using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using HelpMyMom_PROJ.models;
using HelpMyMom_PROJ;



namespace UnitTesting
{
    public class UnitTest1
    {
        private HttpClient _client;

        String accounts_url = "/api/Accounts";
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
           // _client.BaseAddress = new Uri("http://localhost:5124");
            _client.BaseAddress = new Uri("https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net");
        }

        [Fact]
        public async Task GetAccounts()
        {

            var response = await _client.GetAsync(accounts_url);
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

            var response = await _client.GetAsync(tickets_url);
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

            var response = await _client.GetAsync(mothers_url);
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

            var response = await _client.GetAsync(helpers_url);
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

            var response = await _client.GetAsync(chatlogs_url);
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

            var response = await _client.GetAsync(specs_url);
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

            var response = await _client.GetAsync(reviews_url);
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

            var response = await _client.GetAsync(reports_url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            //    Assert.NotNull(objectChecking);
            //   Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetChildren()
        {

            var response = await _client.GetAsync(children_url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            //   Assert.NotNull(objectChecking);
            //   Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

        [Fact]
        public async Task GetRelationships()
        {

            var response = await _client.GetAsync(relationships_url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var objectChecking = JsonConvert.DeserializeObject<List<object>>(content);

            // Assert.NotNull(objectChecking);
            //  Assert.NotEmpty(objectChecking);
            Assert.All(objectChecking, thing => Assert.False(thing.Equals(null)));

        }

    }
}