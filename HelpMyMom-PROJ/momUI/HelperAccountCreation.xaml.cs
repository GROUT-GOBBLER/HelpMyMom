using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI;

public partial class HelperAccountCreation : ContentPage
{
	public HelperAccountCreation()
	{
		InitializeComponent();
	}

    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    async private void CreateAccountButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Connection Error has occured";
                return;
            }
            String json = await response2.Content.ReadAsStringAsync();

            List<Helper> mList = JsonConvert.DeserializeObject<List<Helper>>(json);

            Helper helper = new Helper();
            Account account = new Account();

            helper.Id = (mList[mList.Count - 1].Id + 1);
            helper.FName = FirstNameEntry.Text;
            helper.LName = LastNameEntry.Text;
            helper.Email = EmailEntry.Text;
            String dob = DOBEntry.Text;
            if (!dob.Contains("/"))
            {
                ErrorLabel.Text = "Needs / between yyyy/mm/dd";
                return;
            }
            String[] d_o_b = dob.Split('/');
            if (d_o_b.Length != 3)
            {
                ErrorLabel.Text = "Needs / between yyyy/mm/dd";
                return;
            }
            if (d_o_b[0].Length != 4)
            {
                ErrorLabel.Text = "Needs / between yyyy/mm/dd";
                return;
            }

            int d = Convert.ToInt32(d_o_b[2]);
            int m = Convert.ToInt32(d_o_b[1]);
            int y = Convert.ToInt32(d_o_b[0]);
            DateOnly DateOfBirth = new DateOnly(y,m,d);
            




            helper.Dob = DateOfBirth;


            account.HelperId = helper.Id;
            account.Username = UsernameEntry.Text;
            account.Password = PasswordEntry.Text;

            if (account.Username == null)
            {
                ErrorLabel.Text = "Please enter a username";
                return;
            }
            else if (helper.FName == null)
            {
                ErrorLabel.Text = "Please enter a first name";
                return;
            }
            else if (helper.LName == null)
            {
                ErrorLabel.Text = "Please enter a last name";
                return;
            }
            else if (helper.Email == null)
            {
                ErrorLabel.Text = "Please enter an email";
                return;
            }
            response2 = await client.PostAsJsonAsync($"{URL}/{"Helpers"}", helper);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in helper creation";
                return;

            }
            response2 = await client.PostAsJsonAsync($"{URL}/{"Accounts"}", account);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in account creation";
                response2 = await client.DeleteAsync($"{URL}/{"Helpers"}/{helper.Id}");
                return;

            }
            ErrorLabel.Text = "Success";

        }

    }
}