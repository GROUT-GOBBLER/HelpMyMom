using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class HelperOpenProfile : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    List<SpecialtiesDisplay> helperSPECIALTIES;
	double fontSizeForListView;

	Accessibility fontSizes;
    Account masterAccount;
	Helper masterHelper;
    
	public class SpecialtiesDisplay
	{
		public String? SpecialtyName { get; set; }
		public double NameFontSize { get; set; }
	}

    public HelperOpenProfile(Account a, Helper h)
	{
		InitializeComponent();

		fontSizes = Accessibility.getAccessibilitySettings();
		masterAccount = a;
		masterHelper = h;

		 helperSPECIALTIES = new List<SpecialtiesDisplay>();
	}

	protected override void OnAppearing() // determines what the page does when it opens.
	{
		base.OnAppearing();
        RefreshPage();
        SetFontSizes();
    }
    
    async private void RefreshPage()
	{
        helperSPECIALTIES = new List<SpecialtiesDisplay>();

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Get lists of Reviews and Specs.
                HttpResponseMessage response3 = await client.GetAsync($"{URL}/{"Specs"}");
                String json3 = await response3.Content.ReadAsStringAsync();
                List<Spec>? specsList = JsonConvert.DeserializeObject<List<Spec>>(json3); // specsList.
                HttpResponseMessage response4 = await client.GetAsync($"{URL}/{"Reviews"}");
                String json4 = await response4.Content.ReadAsStringAsync();
                List<Review>? reviewsList = JsonConvert.DeserializeObject<List<Review>>(json4); // reviewsList.

                // Variable initialization.
                String specs = "";
                String[] specsAsNumbers;
                String textForRatingsLabel = "";
                int reviewAverageValue = 0;
                int numberOfApplicableReviews = 0;

                if (masterHelper.Specs != null)
                {
                    specs = masterHelper.Specs;
                }
                else { await DisplayAlert("HelperSpecsNotFound", $"ERROR! Failed to find {masterHelper.FName}'s specs.", "OK"); }

                // Turn the specs values into their string equivalents.
                specsAsNumbers = specs.Split(','); // split Comma-Separated list.

                if (specsList != null)
                {
                    foreach (Spec s in specsList) // converts all of the int values in listOfSpecs into their String descriptions.
                    {
                        for (int x = 0; x < specsAsNumbers.Length; x++)
                        {
                            short shortIDFromSpec = short.Parse(specsAsNumbers[x].Trim()); // convert string # into short #.

                            if (s.Id == shortIDFromSpec)
                            {
                                SpecialtiesDisplay tempSpecialtiesDisplay = new SpecialtiesDisplay();
                                tempSpecialtiesDisplay.SpecialtyName = s.Name;
                                tempSpecialtiesDisplay.NameFontSize = fontSizes.fontsize;
                                helperSPECIALTIES.Add(tempSpecialtiesDisplay);
                            }
                        }
                    }
                }
                else { await DisplayAlert("SpecsNotFound", $"ERROR! Failed to find any specs.", "OK"); }

                // Find review score.
                if (reviewsList != null)
                {
                    if (reviewsList.Count != 0)
                    {
                        foreach (Review r in reviewsList)
                        {
                            if (r.HelperId == masterHelper.Id)
                            {
                                if (r.Stars != null)
                                {
                                    reviewAverageValue += (int)r.Stars;
                                    numberOfApplicableReviews++;
                                }
                                else { await DisplayAlert("ReviewWithNoStars", $"ERROR! Found a review without a star-rating.", "OK"); }
                            }
                        }

                        if (numberOfApplicableReviews > 0)
                        {
                            reviewAverageValue = reviewAverageValue / numberOfApplicableReviews;
                            reviewAverageValue = (int)Math.Ceiling((double)reviewAverageValue / 2);
                            textForRatingsLabel = reviewAverageValue + " stars.";
                        }
                        else { textForRatingsLabel = "No reviews found."; }
                    }
                    else { textForRatingsLabel = "No reviews found."; }
                }
                else { await DisplayAlert("ReviewsNotFound", $"ERROR! Failed to find any reviews.", "OK"); }

                // Set objects in the XAML file with the newfound values.
                UsernameLabel.Text = masterAccount.Username;

                FirstNameLabel.Text = masterHelper.FName;
                LastNameLabel.Text = masterHelper.LName;
                DescriptionLabel.Text = masterHelper.Description;

                SpecialtiesListView.ItemsSource = helperSPECIALTIES;
                RatingsLabel.Text = textForRatingsLabel;
            }
            catch (Exception except)
            {
                ProfilePictureImage.Source = $"Exception occured {except}";
            }
        }
    }

    private void SetFontSizes()
    {
        RatingTextLabel.FontSize = fontSizes.fontsize;
        RatingsLabel.FontSize = fontSizes.fontsize;
        ShowReviewsButton.FontSize = fontSizes.fontsize + 5;
        UsernameTextLabel.FontSize = fontSizes.fontsize;
        UsernameLabel.FontSize = fontSizes.fontsize;
        FirstNameTextLabel.FontSize = fontSizes.fontsize;
        FirstNameLabel.FontSize = fontSizes.fontsize;
        LastNameTextLabel.FontSize = fontSizes.fontsize;
        LastNameLabel.FontSize = fontSizes.fontsize;
        DescriptionTextLabel.FontSize = fontSizes.fontsize;
        DescriptionLabel.FontSize = fontSizes.fontsize;
        SpecialtiesTextLabel.FontSize = fontSizes.fontsize;
        ProfileEditButton.FontSize = fontSizes.fontsize + 5;
    }

    async private void ProfileEditButton_Clicked(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new HelperEditProfile(masterAccount, masterHelper));
    }

    async private void ShowReviewsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ViewReviews(masterHelper));
    }
}