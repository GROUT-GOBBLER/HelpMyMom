﻿using momUI.HelperViews;
using momUI.models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace momUI
{
	public partial class MomLookAtHelperProfile : ContentPage
	{
		String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
		String MASTERusername = "UncleBensBiggestFan";

		Helper masterHelper; // Yarrick-added code.

        ObservableCollection<SpecialtyItem> helperSPECIALTIES;


        public int MessageFontSize { get; set; }

        public class SpecialtyItem
        {
            public string? Name { get; set; }
            public double? MessageFontSizeSpecialty { get; set; } // Property for message label font size
        }


       // public int MessageFontSize;


        private int _momAccountID;
        private int _helperAccountID;
        private int _ticketID;

        public MomLookAtHelperProfile(int momID, int helperID, int ticketID)
		{
			InitializeComponent();
          


            _momAccountID = momID;
            _helperAccountID = helperID;
            _ticketID = ticketID;

			masterHelper = new Helper();

        }

		protected override async void OnAppearing() // determines what the page does when it opens.
		{
            Accessibility ac = Accessibility.getAccessibilitySettings();
            helperSPECIALTIES = new ObservableCollection<SpecialtyItem>();

            MessageFontSize = Math.Min(Math.Max(10, ac.fontsize), 30);

            RatingLabelText.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);
            RatingsLabel.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);

            ShowReviewsButton.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);

            UserNameLabelText.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);
            UsernameLabel.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);

            FirstNameLabelText.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);
            FirstNameLabel.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);

            LastNameLabelText.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);
            LastNameLabel.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);

            DescLabelText.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);
            DescriptionLabel.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);

            SpecTextLabel.FontSize = Math.Min(Math.Max(10, ac.fontsize), 30);


            ReportButton.FontSize = Math.Min(Math.Max(15, ac.fontsize + 5), 45);


            using (HttpClient client = new HttpClient())
			{
				try
				{
					// Get lists of Accounts, Helpers, and Specs.
					HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}");
					String json1 = await response1.Content.ReadAsStringAsync();
					List<Account>? accountsList = JsonConvert.DeserializeObject<List<Account>>(json1); // accountsList.
					HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
					String json2 = await response2.Content.ReadAsStringAsync();
					List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(json2); // helpersList.
					HttpResponseMessage response3 = await client.GetAsync($"{URL}/{"Specs"}");
					String json3 = await response3.Content.ReadAsStringAsync();
					List<Spec>? specsList = JsonConvert.DeserializeObject<List<Spec>>(json3); // specsList.
					HttpResponseMessage response4 = await client.GetAsync($"{URL}/{"Reviews"}");
					String json4 = await response4.Content.ReadAsStringAsync();
					List<Review>? reviewsList = JsonConvert.DeserializeObject<List<Review>>(json4); // reviewsList.

					// Variable initialization.
					Account tempAccount = new Account();
					Helper tempHelper = new Helper();
					String specs = "";
					String[] specsAsNumbers, specsAsStringsFull;
					String textForRatingsLabel = "";
					double reviewAverageValue = 0;
					int numberOfApplicableReviews = 0;

					// Find our account.
					foreach (Account a in accountsList) // Get Helper Id from Accounts table.
					{
						if (a.HelperId == _helperAccountID)
						{
							UsernameLabel.Text = a.Username;
							tempAccount = a;
						}
					}

					if (tempAccount.HelperId == -1)
					{
						await DisplayAlert("AccountNotFoundError", "ERROR! Account not found.", "OK");
						return;
					}

					// Find our helper.
					bool found = false;
					foreach (Helper h in helpersList)
					{
						if (h.Id == tempAccount.HelperId)
						{
							tempHelper = h;
							masterHelper = tempHelper;
							found = true;
						}
					}

					if (!found)
					{
						await DisplayAlert("AccountNotFoundError", $"ERROR! Helper not found.", "OK");
						return;
					}

					specs = tempHelper.Specs;

					// Turn the specs values into their string equivalents.
					specsAsNumbers = specs.Split(','); // split Comma-Separated list.
					specsAsStringsFull = new String[specsAsNumbers.Length];

					foreach (Spec s in specsList) // converts all of the int values in listOfSpecs into their String descriptions.
					{
						for (int x = 0; x < specsAsNumbers.Length; x++)
						{
							short shortIDFromSpec = short.Parse(specsAsNumbers[x].Trim()); // convert string # into short #.

                            Accessibility a = Accessibility.getAccessibilitySettings();
                            if (s.Id == shortIDFromSpec)
							{
								SpecialtyItem newSpecialty = new SpecialtyItem();
								newSpecialty.Name = s.Name;
								newSpecialty.MessageFontSizeSpecialty = Math.Min(Math.Max(10, a.fontsize), 30);
								specsAsStringsFull[x] = s.Name;
								helperSPECIALTIES.Add(newSpecialty);
                            }
						}
					}

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
                                reviewAverageValue = Math.Ceiling(reviewAverageValue / numberOfApplicableReviews);
                                reviewAverageValue = reviewAverageValue / 2;

                                // Create star count.
                                for (int x = 0; x < reviewAverageValue; x++)
                                {
                                    if ((reviewAverageValue % 1) == 0.5 && (x + 1) > reviewAverageValue) { textForRatingsLabel += "½ "; }
                                    else { textForRatingsLabel += "☆ "; }
                                }
                            }
                            else { textForRatingsLabel = "No reviews found."; }
                        }
                        else { textForRatingsLabel = "No reviews found."; }
                    }
                    else { await DisplayAlert("ReviewsNotFound", $"ERROR! Failed to find any reviews.", "OK"); }



                    // Set objects in the XAML file with the newfound values.
                    FirstNameLabel.Text = tempHelper.FName;
					LastNameLabel.Text = tempHelper.LName;
					DescriptionLabel.Text = tempHelper.Description;

					// helperSPECIALTIES = helperSPECIALTIES;
					SpecialtiesListView.ItemsSource = helperSPECIALTIES;
					//SpecialtiesListView.ItemsSource.
					RatingsLabel.Text = textForRatingsLabel;
				}
				catch (Exception except)
				{
					ProfilePictureImage.Source = $"Exception occured {except}";
				}
			}
		}

        

        async private void ShowReviewsButton_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ViewReviews(masterHelper));
		}


		private async void OnReportHelperClicked(object sender, EventArgs e)
		{
			using (HttpClient client = new HttpClient())
			{
				try
				{

					HttpResponseMessage response1 = await client.GetAsync(URL + "/Mothers");
					HttpResponseMessage response2 = await client.GetAsync(URL + "/Helpers");
					HttpResponseMessage response3 = await client.GetAsync(URL + "/Tickets");

					String json1 = await response1.Content.ReadAsStringAsync();
					String json2 = await response2.Content.ReadAsStringAsync();
					String json3 = await response3.Content.ReadAsStringAsync();

					List<Mother>? mothersList = JsonConvert.DeserializeObject<List<Mother>>(json1);
					List<Helper>? helpersList = JsonConvert.DeserializeObject<List<Helper>>(json2);
					List<Ticket>? ticketsList = JsonConvert.DeserializeObject<List<Ticket>>(json3);


					Ticket tempTicket = new Ticket();
					foreach (Ticket index in ticketsList)
					{
						if (index.Id == _ticketID)
						{
							tempTicket = index;
							break;
						}
					}

					int momIndexInList = 0;
					foreach (Mother index in mothersList)
					{
						if (index.Id == tempTicket.MomId)
						{
							momIndexInList = index.Id;
							break;
						}
					}

					int helperIndexInList = 0;
					foreach (Helper index in helpersList)
					{
						if (index.Id == tempTicket.HelperId)
						{
							helperIndexInList = index.Id;
							break;
						}
					}

					// int momID, int helperID, int ticketID
					await Navigation.PushAsync(new MomReportPage(momIndexInList, helperIndexInList, _ticketID));
				}
				catch (Exception ex)
				{
					await DisplayAlert("ERROR:", $"{ex.Message}", "OK");
				}
			}
		}

    }
}