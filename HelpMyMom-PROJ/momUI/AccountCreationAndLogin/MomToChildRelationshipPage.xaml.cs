
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI;

public partial class MomToChildReltionshipPage : ContentPage
{
    Child? child;
    Account? account;
    Mother? mother;
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    public MomToChildReltionshipPage(Mother m)
	{
		InitializeComponent();
        mother = m;
	}
    protected override async void OnAppearing()
    {
        /*
        Title: 35
        Header: 25
        Normal: 15
        Buttons:
        Small: 20
        Med: 30
        Large: 35
        */
        Accessibility a = Accessibility.getAccessibilitySettings();
        FNameLabel.FontSize = a.fontsize;
        LNameLabel.FontSize = a.fontsize;
        ErrorLabel.FontSize = a.fontsize;
        FNameL.FontSize = a.fontsize;
        LNameL.FontSize = a.fontsize;
        Search.FontSize = a.fontsize;
        RelationShipButton.FontSize = a.fontsize + 5;

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
                List<Relationship> rList = JsonConvert.DeserializeObject<List<Relationship>>(json);
                relationship.Id = ((rList[rList.Count - 1].Id) + 1);
                response = await client.PostAsJsonAsync($"{URL}/{"Relationships"}", relationship);
                ErrorLabel.Text = "Success";
                return;
            }
        }
    }

    async private void Search_SearchButtonPressed(object sender, EventArgs e)
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

            response = await client.GetAsync($"{URL}/{"Children"}/{account.ChildId}");

            json = await response.Content.ReadAsStringAsync();
            child = JsonConvert.DeserializeObject<Child>(json);
            FNameLabel.Text = child.FName;
            LNameLabel.Text = child.LName;



        }

}
}