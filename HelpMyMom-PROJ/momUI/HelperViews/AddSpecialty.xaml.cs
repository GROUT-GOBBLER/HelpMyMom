using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI.HelperViews;

public partial class AddSpecialty : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    String newSpecName;

    Accessibility fontSizes;

    public AddSpecialty()
	{
		InitializeComponent();
        newSpecName = "";
        fontSizes = Accessibility.getAccessibilitySettings();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SetFontSizes();
    }

    private void SetFontSizes()
    {
        AddSpecialtyLabel.FontSize = fontSizes.fontsize + 20;
        EnterSpecialtyEntry.FontSize = fontSizes.fontsize;
        SubmitNewSpecialtyButton.FontSize = fontSizes.fontsize + 15;
    }

    private void EnterSpecialtyEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newSpecName = e.NewTextValue;
    }

    async private void SubmitNewSpecialtyButton_Clicked(object sender, EventArgs e)
    {
        SubmitNewSpecialtyButton.IsEnabled = false;

        if (newSpecName == "") // ensure that user has entered a value.
        {
            await DisplayAlert("NoNameEntered", "Please enter a name for the new specialty.", "Ok.");
        }
        else
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage specsResponse = await client.GetAsync($"{URL}/{"Specs"}");
                    String specsJSON = await specsResponse.Content.ReadAsStringAsync();
                    List<Spec>? specsList = JsonConvert.DeserializeObject<List<Spec>>(specsJSON); // specsList.

                    // Create some temporary variables.
                    Spec tempSpec = new Spec();
                    bool alreadyExists = false;

                    // Check to see if spec with this name already exists.
                    if (specsList != null)
                    {
                        foreach (Spec s in specsList)
                        {
                            if (s.Name == newSpecName)
                            {
                                alreadyExists = true;
                                break;
                            }
                        }

                        if (!alreadyExists)
                        {
                            // Set up new specialty.
                            tempSpec.Id += (short)(specsList.Last().Id + 1);
                            tempSpec.Name = newSpecName;

                            HttpResponseMessage addSpecResponse = await client.PostAsJsonAsync($"{URL}/Specs", tempSpec); // add new spec.

                            if (addSpecResponse.IsSuccessStatusCode)
                            {
                                SubmitNewSpecialtyButton.Text = "Success!";
                            }
                            else { await DisplayAlert("AddFailedError", "Error! Failed to add the new specialty.", "Ok."); }
                        }
                        else
                        {
                            await DisplayAlert("AlreadyExists", "The given specialty already exists.", "Ok.");
                            newSpecName = "";
                            EnterSpecialtyEntry.Text = null;
                        }
                    }
                    else
                    {
                        await DisplayAlert("NoSpecsFound", "Error! Failed to find any specs.", "Ok.");
                        return;
                    }
                }
                catch (Exception except)
                {
                    await DisplayAlert("Exception", $"An exception occurred ... {except}", "Ok.");
                }
            }

            // Return to edit page.
            if(Navigation != null) { await Navigation.PopModalAsync(); }
            else { await DisplayAlert("NavigationError", "Error! Something happened when returning to the previous page.", "Ok."); }
        }

        SubmitNewSpecialtyButton.IsEnabled = true;
    }
}