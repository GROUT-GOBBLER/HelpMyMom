using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace momUI.HelperViews;

public partial class HelperEditProfile : ContentPage
{   
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String newUsername = "", newFirstName = "", newLastName = "", newDescription = "", newSpecsList = "";

    List<SpecialtiesView> specsListFull;
    IList<object> selectedSpecs = []; // used for default selection.
    DateOnly fullDateOfBirth;

    Accessibility fontSizes;
    Account masterAccount;
    Helper masterHelper;

    public HelperEditProfile(Account a, Helper h)
    {
        InitializeComponent();

        masterAccount = a;
        masterHelper = h;
        fontSizes = Accessibility.getAccessibilitySettings();
        specsListFull = new List<SpecialtiesView>();

        DateOfBirthDatePicker.MaximumDate = DateTime.Today;

        DateOnly tempDateOnly;
        if (masterHelper.Dob != null) 
        { 
            tempDateOnly = (DateOnly) masterHelper.Dob;
            DateOfBirthDatePicker.Date = tempDateOnly.ToDateTime(TimeOnly.Parse("10:00 PM"));
        }
        else { DateOfBirthDatePicker.Date = DateTime.MinValue; }
    }
    
    public class SpecialtiesView
    {
        public short IdValue { get; set; }
        public String? Name { get; set; }
        public double SpecialtyFontSize { get; set; }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        specsListFull = new List<SpecialtiesView>();
        selectedSpecs = [];
        
        SetSpecsList();
        SetFontSizes();
    }

    private async void SetSpecsList()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage specsResponse = await client.GetAsync($"{URL}/{"Specs"}");
                String specsJSON = await specsResponse.Content.ReadAsStringAsync();
                List<Spec>? specsList = JsonConvert.DeserializeObject<List<Spec>>(specsJSON); // specsList.

                // Get previously selected specialties.
                String helperSpecs;
                if (masterHelper.Specs != null)
                {
                    helperSpecs = masterHelper.Specs;
                }
                else { helperSpecs = ""; }

                // Populate specsListFull.
                if (specsList != null)
                {
                    foreach (Spec s in specsList)
                    {
                        SpecialtiesView tempSpecialtiesView = new SpecialtiesView();
                        tempSpecialtiesView.IdValue = s.Id;
                        tempSpecialtiesView.Name = s.Name;
                        tempSpecialtiesView.SpecialtyFontSize = fontSizes.fontsize;

                        specsListFull.Add(tempSpecialtiesView);

                        if(helperSpecs.Contains(tempSpecialtiesView.IdValue + ""))
                        {
                            selectedSpecs.Add(tempSpecialtiesView);
                        }
                    }
                }
                else { await DisplayAlert("NoSpecsFound", "Error! Failed to find any specs.", "Ok."); }
            }
            catch (Exception e)
            {
                await DisplayAlert("Exception", $"An exception occurred in SetSpecList() ... {e}", "Ok.");
                return;
            }
        }

        SpecsCollectionView.ItemsSource = specsListFull;
         SpecsCollectionView.SelectedItems = selectedSpecs;
    }

    private void SetFontSizes()
    {
        ProfileSettingsLabel.FontSize = fontSizes.fontsize + 20;
        UsernameLabel.FontSize = fontSizes.fontsize;
        UsernameEntry.FontSize = fontSizes.fontsize;
        UsernameEditButton.FontSize = fontSizes.fontsize + 5;
        FirstNameLabel.FontSize = fontSizes.fontsize;
        FirstNameEntry.FontSize = fontSizes.fontsize;
        LastNameEntry.FontSize = fontSizes.fontsize;
        ChangeNameButton.FontSize = fontSizes.fontsize + 5;
        DateOfBirthLabel.FontSize = fontSizes.fontsize;
        DateOfBirthDatePicker.FontSize = fontSizes.fontsize;
        DateOfBirthButton.FontSize = fontSizes.fontsize + 5;
        DescriptionLabel.FontSize = fontSizes.fontsize;
        DescriptionEditor.FontSize = fontSizes.fontsize;
        DescriptionButton.FontSize = fontSizes.fontsize + 5;
        SpecialtiesLabel.FontSize = fontSizes.fontsize;
        AddNewSpecButton.FontSize = fontSizes.fontsize;
        SpecsButton.FontSize = fontSizes.fontsize;
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
        UsernameEntry.Placeholder = "...";
    }

    // NAME.
    private void FirstNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newFirstName = e.NewTextValue;
    }

    private void LastNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newLastName = e.NewTextValue;
    }

    async private void ChangeNameButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                if(newFirstName != "")
                {
                    masterHelper.FName = newFirstName;
                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);

                    if (response3.IsSuccessStatusCode) { ChangeNameButton.Text = "Success!"; }
                    else { ChangeNameButton.Text = "Failure 1."; }
                }
            }
            catch (Exception except)
            {
                ChangeNameButton.Text = $"Exception Occurred: {except}";
            }

            try
            {
                if(newLastName != "")
                {
                    masterHelper.LName = newLastName;
                    HttpResponseMessage response3 = await client.PutAsJsonAsync($"{URL}/Helpers/{masterHelper.Id}", masterHelper);

                    if (response3.IsSuccessStatusCode) { ChangeNameButton.Text = " Success."; }
                    else { ChangeNameButton.Text = "Failure 2."; }
                }
                
            }
            catch (Exception except)
            {
                ChangeNameButton.Text = $"Exception Occurred: {except}";
            }
        }

        newFirstName = "";
        newLastName = "";
        FirstNameEntry.Placeholder = "First name";
        LastNameEntry.Placeholder = "Last name";
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
    async private void SpecsButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                foreach(SpecialtiesView sv in specsListFull)
                {
                    if(sv.IdValue != specsListFull.Last<SpecialtiesView>().IdValue) // not the last element in the list.
                    {
                        newSpecsList += $"{sv.IdValue}, ";
                    }
                    else // last element in the list.
                    {
                        newSpecsList += $"{sv.IdValue}";
                    }
                }

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
    }

    private void SpecsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        IReadOnlyList<object> temp = e.CurrentSelection;

        specsListFull = new List<SpecialtiesView>();

        foreach(object o in temp)
        {
            SpecialtiesView tempSpec = (SpecialtiesView) o;
            specsListFull.Add(tempSpec);
        }
    }
    async private void AddNewSpecButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddSpecialty());
    }

    // DESCRIPTION.
    private void DescriptionEditor_TextChanged(object sender, TextChangedEventArgs e)
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
        DescriptionEditor.Text = "";
    }
}