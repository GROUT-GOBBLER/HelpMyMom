using momUI.models;
using Newtonsoft.Json;

namespace momUI.HelperViews;

public partial class HelperOpenProfile : ContentPage
{
    String URL = "https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
	String[] helperSPECIALTIES;

    public HelperOpenProfile()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing() // determines what the page does when it opens.
	{
		using (HttpClient client = new HttpClient())
		{
			try
			{
				// Get lists of Accounts, Helpers, and Specs.
				HttpResponseMessage response1 = await client.GetAsync($"{URL}/{"Accounts"}");
					String json1 = await response1.Content.ReadAsStringAsync();
					List<Account> accountsList = JsonConvert.DeserializeObject<List<Account>>(json1); // accountsList.
                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
					String json2 = await response2.Content.ReadAsStringAsync();
					List<Helper> helpersList = JsonConvert.DeserializeObject<List<Helper>>(json2); // helpersList.
                HttpResponseMessage response3 = await client.GetAsync($"{URL}/{"Specs"}");
					String json3 = await response3.Content.ReadAsStringAsync();
					List<Spec> specsList = JsonConvert.DeserializeObject<List<Spec>>(json3); // specsList.

				/* 
                 *	The below codes assumes that we are accessing the FIRST account in the account table.
				 *	ACCOUNT ... username = "DefinetlyNotPartOfTheMafia", helperID = 1.
				 *	HELPER ... fName = "Rob", lName = "Bankman", specs = "0,1", description = "Young cash getter."
				 *	SPECS ... 1 = Roku TV, 2 = Microsoft Word.
				 */

				// Define a bunch of variables to hold all of the values.
                Account tempAccount = accountsList.First();
                String accountID = tempAccount.Username;
                int? helperID = tempAccount.HelperId;

				Helper tempHelper = new Helper();
				String specs = "";
				String[] specsAsNumbers, specsAsStringsFull;

                // Find our helper.
                foreach (Account a in accountsList) // Get Helper Id from Accounts table.
                {
					if(a.Username == accountID)
					{
						UsernameLabel.Text = a.Username;
                        helperID = a.HelperId;
					}
				}

				foreach(Helper h in helpersList) // Get HELPER object from Helper table.
				{
					if(h.Id == helperID)
					{
						tempHelper = h;
					}
				}
				
				specs = tempHelper.Specs;

				// Turn the specs values into their string equivalents.
					specsAsNumbers = specs.Split(','); // split Comma-Separated list.
					specsAsStringsFull = new String[specsAsNumbers.Length];
				
				foreach(Spec s in specsList) // converts all of the int values in listOfSpecs into their String descriptions.
				{
					for(int x = 0; x < specsAsNumbers.Length; x++)
					{
						short shortIDFromSpec = short.Parse(specsAsNumbers[x].Trim()); // convert string # into short #.

						if (s.Id == shortIDFromSpec)
						{
							specsAsStringsFull[x] = s.Name;
						}
					}
				}

				// Set objects in the XAML file with the newfound values.
				FirstNameLabel.Text = tempHelper.FName;
				LastNameLabel.Text = tempHelper.LName;
				DescriptionLabel.Text = tempHelper.Description;

                helperSPECIALTIES = specsAsStringsFull;
				SpecialtiesListView.ItemsSource = helperSPECIALTIES;
            }
            catch (Exception except)
			{
				ProfilePictureImage.Source = $"Exception occured {except}";
            }
		}
    }

    async private void ProfileEditButton_Clicked(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new HelperEditProfile());
    }
}