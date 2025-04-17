using momUI.models;
using Newtonsoft.Json;

namespace momUI;

public partial class ChildAccountSettings : ContentPage
{
	Child account;
	List<Account> allAccounts;
	List<Mother> allMoms;
	List<Mother> linkedMoms;
    List<string> momsString = new List<string>();

    string newFirst;
    string newLast;
    string newMomUser;

    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
	
    public ChildAccountSettings(Child acc)
	{
		InitializeComponent();
		account = acc;
	}

    protected override async void OnAppearing()
	{
        using (HttpClient client = new HttpClient())
		{
			try
			{
                List<Relationship> relationships = new List<Relationship>();
                HttpResponseMessage accountResponse = await client.GetAsync(URL + "/Accounts");
                HttpResponseMessage momResponse = await client.GetAsync(URL + "/Mothers");
                HttpResponseMessage relationshipResponse = await client.GetAsync(URL + "/Relationships");

                changeNameBtn.IsEnabled = false;
                AddMomBtn.IsEnabled = false;
                RemoveMom.IsEnabled = false;

                if (accountResponse.IsSuccessStatusCode && momResponse.IsSuccessStatusCode && relationshipResponse.IsSuccessStatusCode)
				{
                    string json1 = await accountResponse.Content.ReadAsStringAsync();
                    string json2 = await momResponse.Content.ReadAsStringAsync();
                    string json3 = await relationshipResponse.Content.ReadAsStringAsync();

                    allAccounts = JsonConvert.DeserializeObject<List<Account>>(json1);
                    allMoms = JsonConvert.DeserializeObject<List<Mother>>(json2);
                    relationships = JsonConvert.DeserializeObject<List<Relationship>>(json3);

                    if (allAccounts != null && allMoms != null)
					{
						allAccounts.RemoveAll(acc => acc.MomId != null);
                        relationships.RemoveAll(r => r.ChildId != account.Id);

                        var relatedMomIds = relationships
                            .Where(r => r.MomId.HasValue)
                            .Select(r => r.MomId.Value)
                            .ToHashSet();

                        linkedMoms = allMoms;
                        linkedMoms.RemoveAll(m => !relatedMomIds.Contains(m.Id));

                        if (linkedMoms.Count < 1 || linkedMoms == null)
                        {
                            throw new Exception("You have no mom :(");
                        }
                        else
                        {
                            foreach (Mother m in linkedMoms)
                            {
                                momsString.Add($"{m.Id} {m.FName} {m.LName}");
                            }

                            momSelect.ItemsSource = momsString;

                            changeNameBtn.IsEnabled = true;
                            AddMomBtn.IsEnabled = true;
                            RemoveMom.IsEnabled = true;
                        }
                    }
					else
					{
						throw new Exception("Error getting moms or accounts");
					}
                }
				else throw new Exception("DB Error");
            }
			catch (Exception ex)
			{
                NameText.Text = ex.Message;

                Console.WriteLine("\n-------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------------\n");
            }
		}
    }

    private void FirstEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newFirst = fName.Text;
    }

    private void LastEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newLast = lName.Text;
    }

    private void MomEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        newMomUser = MomUsername.Text;
    }

    private async void ChangeNameClicked(object sender, EventArgs e)
	{

	}
}