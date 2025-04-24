using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class ViewReviews : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    List<ReviewView> listOfAllReviews;
    Helper masterHelper;

	private class ReviewView
	{
		public double numberOfStars { get; set; }
        public String? reviewTextContent { get; set; }
	}

	public ViewReviews(Helper h)
	{
		InitializeComponent();

        masterHelper = h;
		listOfAllReviews = new List<ReviewView>();

        PopulateReviews();
    }

    async void PopulateReviews() // determines what the page does when it opens.
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage reviewsResponse = await client.GetAsync($"{URL}/{"Reviews"}");
                String reviewJSON = await reviewsResponse.Content.ReadAsStringAsync();
                List<Review>? reviewsList = JsonConvert.DeserializeObject<List<Review>>(reviewJSON); // reviewsList.

                if (reviewsResponse.IsSuccessStatusCode)
                {
                    if (reviewsList != null)
                    {
                        foreach (Review r in reviewsList)
                        {
                            if (r.HelperId == masterHelper.Id)
                            {
                                ReviewView tempReviewView = new ReviewView();
                                tempReviewView.reviewTextContent = r.Text;
                                if(r.Stars != null)
                                {
                                    tempReviewView.numberOfStars = (double)(r.Stars / 2.0);
                                }
                                else { await DisplayAlert("ReviewWithoutStars", $"Review with ID {r.Id} did not have any stars.", "Ok."); }    
                                listOfAllReviews.Add(tempReviewView);
                            }
                        }
                    }
                    else { await DisplayAlert("ReviewsNotFound", "Error! Failed to find any reviews.", "Ok."); }

                    AllReviewsListView.ItemsSource = listOfAllReviews;
                }
                else { await DisplayAlert("DatabaseConnectionFailure", "Error! Failed to connect to the database.", "Ok."); }
            }
            catch (Exception e)
            {
                AllReviewsListView.ItemsSource = $"Exception occured! {e}";
            }
        }
    }
}