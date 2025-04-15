using momUI.models;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperEditProfile : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String MASTERusername = "UncleBensBiggestFan";
    String newUsername = null, newFirstName = null, newLastName = null, newDescription = null, newSpecsList = null, newDay, newMonth, newYear;

	public HelperEditProfile()
	{
		InitializeComponent();
	}

    // USERNAME.
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

    // FIRST NAME.
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

    // LAST NAME.
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
    
    // DATE OF BIRTH.
    private void DateOfBirthMONTHEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newMonth = e.NewTextValue;
    }

    private void DateOfBirthDAYEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newDay = e.NewTextValue;
    }

    private void DateOfBirthYEAREntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newYear = e.NewTextValue;
    }

    async private void DateOfBirthButton_Clicked(object sender, EventArgs e)
    {
        using(HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}");
                    String json1 = await response1.Content.ReadAsStringAsync();
                    List<Account> listAccounts = JsonConvert.DeserializeObject<List<Account>>(json1); // listAccounts.
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
                    String json2 = await response2.Content.ReadAsStringAsync();
                    List<Helper> listHelpers = JsonConvert.DeserializeObject<List<Helper>>(json2); // listHelpers.

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    string accountUsername = MASTERusername;
                    Helper tempHelper = new Helper();
                    DateOnly fullDateOfBirth = new DateOnly();

                    int? helperID = -1;
                    foreach(Account a in listAccounts) // get helper ID from account.
                    {
                        if(a.Username == accountUsername)
                        {
                            helperID = a.HelperId;
                            break;
                        }
                    }

                    if(helperID == -1)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Account not found.", "OK");
                        return;
                    }

                    bool found = false;
                    foreach(Helper h in listHelpers) // get helper object from helper ID.
                    {
                        if(h.Id == helperID)
                        {
                            tempHelper = h;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        await DisplayAlert("HelperNotFoundError", "ERROR! Helper not found.", "OK");
                        return;
                    }

                    fullDateOfBirth = new DateOnly(Int32.Parse(newYear), Int32.Parse(newMonth), Int32.Parse(newDay)); // Concatanate date of birth into one string.
                    tempHelper.Dob = fullDateOfBirth;

                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{helperID}", tempHelper); // replace helper.

                    if (response3.IsSuccessStatusCode)
                    {
                        DateOfBirthButton.Text = "Success.";
                    }
                    else
                    {
                        DateOfBirthButton.Text = "Failure.";
                    }
                }
                else
                {
                    DateOfBirthButton.Text = "Failed to access database.";
                }
            }
            catch(Exception except)
            {
                DateOfBirthButton.Text = $"Exception occured ... {except}";
            }
        }

        newDay = null;
        newMonth = null;
        newYear = null;
    }

    // SPECS.
    private void SpecsEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newSpecsList = e.NewTextValue;
    }

    async private void SpecsButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}");
                    String json1 = await response1.Content.ReadAsStringAsync();
                    List<Account> listAccounts = JsonConvert.DeserializeObject<List<Account>>(json1); // listAccounts.
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
                    String json2 = await response2.Content.ReadAsStringAsync();
                    List<Helper> listHelpers = JsonConvert.DeserializeObject<List<Helper>>(json2); // listHelpers.

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    String accountUsername = MASTERusername;
                    Helper tempHelper = new Helper();

                    int? helperID = -1;
                    foreach (Account a in listAccounts) // find helper ID from Account.
                    {
                        if (a.Username == accountUsername)
                        {
                            helperID = a.HelperId;
                            break;
                        }
                    }

                    if (helperID == -1)
                    {
                        await DisplayAlert("AccountNotFoundError", "ERROR! Account not found.", "OK");
                        return;
                    }

                    bool found = false;
                    foreach (Helper h in listHelpers) // find Helper object from Helper ID.
                    {
                        if (h.Id == helperID)
                        {
                            found = true;
                            tempHelper = h;
                            break;
                        }
                    }

                    if (!found)
                    {
                        await DisplayAlert("HelperNotFoundError", "ERROR! Helper not found.", "OK");
                        return;
                    }

                    tempHelper.Specs = newSpecsList;
                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{helperID}", tempHelper); // replace helper.

                    if (response3.IsSuccessStatusCode)
                    {
                        SpecsButton.Text = "Success!";
                    }
                    else
                    {
                        SpecsButton.Text = "Failure!";
                    }
                }
                else
                {
                    SpecsButton.Text = "Failed to access one of the database tables.";
                }
            }
            catch (Exception except)
            {
                SpecsButton.Text = $"Exception occured ... {except}";
            }
        }

        newSpecsList = null;
    }

    // DESCRIPTION.
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