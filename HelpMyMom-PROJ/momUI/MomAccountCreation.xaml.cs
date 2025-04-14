
using momUI.models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace momUI;

public partial class MomAccountCreation : ContentPage
{
	public MomAccountCreation()
	{
		InitializeComponent();
	}
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    async private void CreateAccountButton_Clicked(object sender, EventArgs e)
    {
		using (HttpClient client = new HttpClient())
		{
            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Mothers"}");
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Connection Error has occured";
                return;
            }
            String json = await response2.Content.ReadAsStringAsync();

            List<Mother> mList = JsonConvert.DeserializeObject< List<Mother>>(json);

            Mother mother = new Mother();
			Account account = new Account();

			mother.Id = (mList[mList.Count - 1].Id + 1);
            mother.FName = FirstNameEntry.Text;
			mother.LName = LastNameEntry.Text;
			mother.Email = EmailEntry.Text;
			mother.Tokens = 0;

			account.MomId = mother.Id;
			account.Username = UsernameEntry.Text;
			account.Password = PasswordEntry.Text;

			if (account.Username == null)
			{
				ErrorLabel.Text = "Please enter a username";
				return;
			}
			else if (mother.FName == null) {
                ErrorLabel.Text = "Please enter a first name";
                return;
            }
            else if (mother.LName == null)
            {
                ErrorLabel.Text = "Please enter a last name";
                return;
            }
            else if (mother.Email == null)
            {
                ErrorLabel.Text = "Please enter an email";
                return;
            }
            response2 = await client.PostAsJsonAsync($"{URL}/{"Mothers"}", mother);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in mother creation";
                return;

            }
            response2 = await client.PostAsJsonAsync($"{URL}/{"Accounts"}", account);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in account creation";
                response2 = await client.DeleteAsync($"{URL}/{"Mothers"}/{mother.Id}");
                return;

            }
            ErrorLabel.Text = "Success";

        }

    }
}