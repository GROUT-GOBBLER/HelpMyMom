using momUI.models;
using Newtonsoft.Json;
using System;
using System.Net.Http.Json;

namespace momUI;

public partial class ChildToMomRelationshipPage : ContentPage
{
	public ChildToMomRelationshipPage(Child c)
	{
		InitializeComponent();
        child = c;
   
	}
    Child? child;
    Account? account;
    Mother? mother;
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    async private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"{URL}/{"Accounts"}/{Search.Text}");
            String json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                account = null;
                FNameLabel.Text = "????";
                LNameLabel.Text = "????";
                return;
            }
            account = JsonConvert.DeserializeObject<Account>(json);

            response = await client.GetAsync($"{URL}/{"Mothers"}/{account.MomId}");
            json = await response.Content.ReadAsStringAsync();
            mother = JsonConvert.DeserializeObject<Mother>(json);
            FNameLabel.Text = mother.FName;
            LNameLabel.Text = mother.LName;




        }

    }

    async private void RelationShipButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {

            if (account != null && mother != null && child != null)
            {
                Relationship relationship = new Relationship();
                relationship.ChildId = child.Id;
                relationship.MomId = mother.Id;
                HttpResponseMessage response = await client.GetAsync($"{URL}/{"Relationships"}");
                String json = await response.Content.ReadAsStringAsync();
                List<Relationship> rList = JsonConvert.DeserializeObject< List<Relationship>>(json);
                relationship.Id = ((rList[rList.Count - 1].Id) + 1);
                response = await client.PostAsJsonAsync($"{URL}/{"Relationships"}", relationship);
                ErrorLabel.Text = "Success";
                return;
            }
        }

    }
}