using momUI.models;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperEditProfile : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String MASTERusername = "UncleBensBiggestFan";
    String newUsername = null, newFirstName = null, newLastName = null, newDescription = null;

	public HelperEditProfile()
	{
		InitializeComponent();
	}

    private void UsernameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newUsername = e.NewTextValue;
    }

    async private void UsernameEditButton_Clicked(object sender, EventArgs e)
    {
        using(HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Helpers"}"); // HELPER.
                    String json1 = await response1.Content.ReadAsStringAsync();
                    List<Helper> listHelpers = JsonConvert.DeserializeObject<List<Helper>>(json1);

                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Accounts"}"); // HELPER.
                    String json2 = await response2.Content.ReadAsStringAsync();
                    List<Account> listAccounts = JsonConvert.DeserializeObject<List<Account>>(json2);

                if(response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    String accountUsername = MASTERusername;
                    int? helperID = -1;
                    Account tempAccount = new Account();

                    foreach (Account a in listAccounts)
                    {
                        if(a.Username == accountUsername)
                        {
                            helperID = a.HelperId;
                            tempAccount = a;
                        }
                    }

                    if(helperID == -1)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Account not found.", "OK");
                        return;
                    }
                    
                    tempAccount.Username = newUsername; // Update username.

                    HttpResponseMessage response3 = await client.PostAsJsonAsync($"{URL}/Accounts", tempAccount); // add new account.
                    HttpResponseMessage response4 = await client.DeleteAsync($"{URL}/Accounts/{accountUsername}"); // Delete old account.

                    if (response3.IsSuccessStatusCode && response4.IsSuccessStatusCode)
                    {
                        UsernameEditButton.Text = "Post and Delete - Success.";
                    }
                    else
                    {
                        if(response3.IsSuccessStatusCode && !response4.IsSuccessStatusCode)
                        {
                            UsernameEditButton.Text = "Post - Success, Delete - Fail.";
                        }
                        else if(!response3.IsSuccessStatusCode && response4.IsSuccessStatusCode)
                        {
                            UsernameEditButton.Text = "Post - Fail, Delete - Success.";
                        }
                        else
                        {
                            UsernameEditButton.Text = "Post and Delete - Fail.";
                        }
                    }
                }
            }
            catch (Exception except)
            {
                UsernameEditButton.Text = $"Exception Occured: {except}";
            }
        }

        newUsername = null;
    }

    private void FirstNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newFirstName = e.NewTextValue;
    }

    async private void FirstNameButton_Clicked(object sender, EventArgs e)
    {
        using(HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}"); // ACCOUNT.
                    String json2 = await response1.Content.ReadAsStringAsync();
                    List<Account> listAccounts = JsonConvert.DeserializeObject<List<Account>>(json2);
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}"); // HELPER.
                    String json1 = await response2.Content.ReadAsStringAsync();
                    List<Helper> listHelpers = JsonConvert.DeserializeObject<List<Helper>>(json1);

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    String accountUsername = MASTERusername;

                    int? helperID = -1;
                    foreach (Account a in listAccounts) // find account.
                    {
                        if(a.Username == accountUsername)
                        {
                            helperID = a.HelperId;
                        }
                    }
                    
                    if (helperID == -1)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Account not found.", "OK");
                        return;
                    }

                    bool found = false;
                    Helper tempHelper = new Helper();
                    foreach (Helper h in listHelpers) // find helper.
                    {
                        if(h.Id == helperID)
                        {
                            tempHelper = h;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Helper not found.", "OK");
                        return;
                    }

                    tempHelper.FName = newFirstName;

                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{helperID}", tempHelper); // add new account.

                    if (response3.IsSuccessStatusCode)
                    {
                        FirstNameButton.Text = $"Success!";
                    }
                    else
                    {
                        FirstNameButton.Text = $"Failure.";
                    }
                }
                else
                {
                    FirstNameButton.Text = $"Failed to access either Helpers or Accounts.";
                }
            }
            catch(Exception except)
            {
                FirstNameButton.Text = $"Exception Occurred: {except}";
            }
        }

        newFirstName = null;
    }


    private void LastNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newLastName = e.NewTextValue;
    }

    async private void LastNameButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}"); // ACCOUNT.
                String json2 = await response1.Content.ReadAsStringAsync();
                List<Account> listAccounts = JsonConvert.DeserializeObject<List<Account>>(json2);
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}"); // HELPER.
                String json1 = await response2.Content.ReadAsStringAsync();
                List<Helper> listHelpers = JsonConvert.DeserializeObject<List<Helper>>(json1);

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    String accountUsername = MASTERusername;

                    int? helperID = -1;
                    foreach (Account a in listAccounts) // find account.
                    {
                        if (a.Username == accountUsername)
                        {
                            helperID = a.HelperId;
                        }
                    }

                    if (helperID == -1)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Account not found.", "OK");
                        return;
                    }

                    bool found = false;
                    Helper tempHelper = new Helper();
                    foreach (Helper h in listHelpers) // find helper.
                    {
                        if (h.Id == helperID)
                        {
                            tempHelper = h;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Helper not found.", "OK");
                        return;
                    }

                    tempHelper.LName = newLastName;

                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{helperID}", tempHelper); // add new account.

                    if (response3.IsSuccessStatusCode)
                    {
                        LastNameButton.Text = $"Success!";
                    }
                    else
                    {
                        LastNameButton.Text = $"Failure.";
                    }
                }
                else
                {
                    LastNameButton.Text = $"Failed to access either Helpers or Accounts.";
                }
            }
            catch (Exception except)
            {
                LastNameButton.Text = $"Exception Occurred: {except}";
            }
        }

        newLastName = null;
    }

    private void DescriptionEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newDescription = e.NewTextValue;
    }

    async private void DescriptionButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}"); // ACCOUNT.
                    String json2 = await response1.Content.ReadAsStringAsync();
                    List<Account> listAccounts = JsonConvert.DeserializeObject<List<Account>>(json2);
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}"); // HELPER.
                    String json1 = await response2.Content.ReadAsStringAsync();
                    List<Helper> listHelpers = JsonConvert.DeserializeObject<List<Helper>>(json1);

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    String accountUsername = MASTERusername;

                    int? helperID = -1;
                    foreach (Account a in listAccounts) // find account.
                    {
                        if (a.Username == accountUsername)
                        {
                            helperID = a.HelperId;
                        }
                    }

                    if (helperID == -1)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Account not found.", "OK");
                        return;
                    }

                    bool found = false;
                    Helper tempHelper = new Helper();
                    foreach (Helper h in listHelpers) // find helper.
                    {
                        if (h.Id == helperID)
                        {
                            tempHelper = h;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Helper not found.", "OK");
                        return;
                    }

                    tempHelper.Description = newDescription;

                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{helperID}", tempHelper); // add new account.

                    if (response3.IsSuccessStatusCode)
                    {
                        DescriptionButton.Text = $"Success!";
                    }
                    else
                    {
                        DescriptionButton.Text = $"Failure.";
                    }
                }
                else
                {
                    DescriptionButton.Text = $"Failed to access either Helpers or Accounts.";
                }
            }
            catch (Exception except)
            {
                DescriptionButton.Text = $"Exception Occurred: {except}";
            }
        }

        newDescription = null;
    }
}