
using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI;

public partial class ChildAccountCreation : ContentPage
{
	public ChildAccountCreation()
	{
		InitializeComponent();
	}
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    protected override async void OnAppearing()
    {
        /*
         * Title: 35
        Header: 25
        Normal: 15
        Buttons:
        Small: 20
        Med: 30
        Large: 35
        */
        Accessibility a = Accessibility.getAccessibilitySettings();
        UserL.FontSize = a.fontsize;
        UsernameEntry.FontSize = a.fontsize;
        LNAMEL.FontSize = a.fontsize;   
        FNAMEL.FontSize = a.fontsize;
        PasswordEntry.FontSize = a.fontsize;
        PasswordL.FontSize = a.fontsize;
        EmailL.FontSize = a.fontsize;
        EmailEntry.FontSize = a.fontsize;
        FirstNameEntry.FontSize = a.fontsize;
        LastNameEntry.FontSize = a.fontsize;
        CreateAccountButton.FontSize = a.fontsize + 5;
        ErrorLabel.FontSize = a.fontsize;


    }
        async private void CreateAccountButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Children"}");
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Connection Error has occured";
                return;
            }
            String json = await response2.Content.ReadAsStringAsync();

            List<Child> mList = JsonConvert.DeserializeObject<List<Child>>(json);

            Child child = new Child();
            Account account = new Account();

            child.Id = (mList[mList.Count - 1].Id + 1);
            child.FName = FirstNameEntry.Text;
            child.LName = LastNameEntry.Text;
            String E = EmailEntry.Text.Split("\n")[0];
            child.Email = E;


            account.ChildId = child.Id;
            account.Username = UsernameEntry.Text;
            account.Password = PasswordEntry.Text;

            if (account.Username == null)
            {
                ErrorLabel.Text = "Please enter a username";
                return;
            }
            else if (child.FName == null)
            {
                ErrorLabel.Text = "Please enter a first name";
                return;
            }
            else if (child.LName == null)
            {
                ErrorLabel.Text = "Please enter a last name";
                return;
            }
            else if (child.Email == null)
            {
                ErrorLabel.Text = "Please enter an email";
                return;
            }
            response2 = await client.PostAsJsonAsync($"{URL}/{"Children"}", child);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in child creation";
                return;

            }
            response2 = await client.PostAsJsonAsync($"{URL}/{"Accounts"}", account);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in account creation";
                response2 = await client.DeleteAsync($"{URL}/{"Children"}/{child.Id}");
                return;

            }
            ErrorLabel.Text = "Success";
            await Navigation.PushAsync(new ChildToMomRelationshipPage(child));

        }

    }
}