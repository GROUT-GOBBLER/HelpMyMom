using momUI.models;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class HelperEditProfile : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String newUsername = "", newFirstName = "", newLastName = "", newDescription = "", newSpecsList = "";

    DateOnly fullDateOfBirth;

    Account masterAccount;
    Helper masterHelper;

    public HelperEditProfile(Account a, Helper h)
    {
        InitializeComponent();

        masterAccount = a;
        masterHelper = h;

        DateOfBirthDatePicker.MaximumDate = DateTime.Today;

        DateOnly tempDateOnly;
        if (masterHelper.Dob != null) 
        { 
            tempDateOnly = (DateOnly) masterHelper.Dob;
            DateOfBirthDatePicker.Date = tempDateOnly.ToDateTime(TimeOnly.Parse("10:00 PM"));
        }
        else { DateOfBirthDatePicker.Date = DateTime.MinValue; }
            
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
                String oldUsername = masterAccount.Username;
                masterAccount.Username = newUsername; // Update username.
                
                HttpResponseMessage response3 = await client.PostAsJsonAsync($"{URL}/Accounts", masterAccount); // add new account.
                HttpResponseMessage response4 = await client.DeleteAsync($"{URL}/Accounts/{oldUsername}"); // Delete old account.

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
            catch (Exception except)
            {
                UsernameEditButton.Text = $"Exception Occured: {except}";
            }
        }

        newUsername = "";
        UsernameEntry.Text = null;
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
                masterHelper.FName = newFirstName;
                
                HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);
                
                if (response3.IsSuccessStatusCode) { FirstNameButton.Text = $"Success!"; }
                else { FirstNameButton.Text = $"Failure."; }
            }
            catch(Exception except)
            {
                FirstNameButton.Text = $"Exception Occurred: {except}";
            }
        }

        newFirstName = "";
        FirstNameEntry.Text = null;
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
                masterHelper.LName = newLastName;
                
                HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);

                if (response3.IsSuccessStatusCode) { LastNameButton.Text = $"Success!"; }
                else { LastNameButton.Text = $"Failure."; }
            }
            catch (Exception except)
            {
                LastNameButton.Text = $"Exception Occurred: {except}";
            }
        }

        newLastName = "";
        LastNameEntry.Text = null;
    }

    // DATE OF BIRTH.
    async private void DateOfBirthButton_Clicked(object sender, EventArgs e)
    {
        using(HttpClient client = new HttpClient())
        {
            try
            {
                masterHelper.Dob = fullDateOfBirth;

                HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);
                
                if (response3.IsSuccessStatusCode) { DateOfBirthButton.Text = "Success."; }
                else { DateOfBirthButton.Text = "Failure."; }
            }
            catch(Exception except)
            {
                DateOfBirthButton.Text = $"Exception occured ... {except}";
            }
        }

        DateOfBirthDatePicker.MaximumDate = DateTime.Today;
        DateOfBirthDatePicker.Date = fullDateOfBirth.ToDateTime(TimeOnly.Parse("10:00 PM"));
    }

    private void DateOfBirthDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        fullDateOfBirth = new DateOnly(e.NewDate.Year, e.NewDate.Month, e.NewDate.Day);
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
                masterHelper.Specs = newSpecsList;
                HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);

                if (response3.IsSuccessStatusCode) { SpecsButton.Text = "Success!"; }
                else { SpecsButton.Text = "Failure!"; }
            }
            catch (Exception except)
            {
                SpecsButton.Text = $"Exception occured ... {except}";
            }
        }

        newSpecsList = "";
        SpecsEntry.Text = null;
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
                masterHelper.Description = newDescription;
                
                HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);
                
                if (response3.IsSuccessStatusCode) { DescriptionButton.Text = $"Success!"; }
                else { DescriptionButton.Text = $"Failure."; }
            }
            catch (Exception except)
            {
                DescriptionButton.Text = $"Exception Occurred: {except}";
            }
        }

        newDescription = "";
        DescriptionEntry.Text = null;
    }
}