using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class ViewReviews : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    List<ReviewView> listOfAllReviews;
    int helperID = 1; // hard-coded helper ID.

	private class ReviewView
	{
		public double numberOfStars { get; set; }
        public String? reviewTextContent { get; set; }
	}

	public ViewReviews()
	{
		InitializeComponent();
		listOfAllReviews = new List<ReviewView>();
    }

    protected override async void OnAppearing() // determines what the page does when it opens.
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{URL}/{"Reviews"}");
                String json = await response.Content.ReadAsStringAsync();
                List<Review> reviewsList = JsonConvert.DeserializeObject<List<Review>>(json); // reviewsList.

                    foreach (Review r in reviewsList)
                    {
                        if (r.HelperId == helperID)
                        {
                            ReviewView tempReviewView = new ReviewView();
                            tempReviewView.reviewTextContent = r.Text;
                            tempReviewView.numberOfStars = (double)(r.Stars / 2.0);
                            listOfAllReviews.Add(tempReviewView);
                        }
                    }

                AllReviewsListView.ItemsSource = listOfAllReviews;
            }
            catch (Exception e)
            {
                AllReviewsListView.ItemsSource = $"Exception occured! {e}";
            }
        }
    }
}