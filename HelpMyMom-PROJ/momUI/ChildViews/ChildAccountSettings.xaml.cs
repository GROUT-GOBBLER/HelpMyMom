using momUI.models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace momUI;

public partial class ChildAccountSettings : ContentPage
{
	Child account;
	List<Account>? allAccounts;
	List<Mother>? allMoms;
	List<Mother>? linkedMoms;
    List<Relationship>? relationships = new List<Relationship>();
    List<Relationship>? linkedR = new List<Relationship>();
    List<string> momsString = new List<string>();

    string newFirst = "";
    string newLast = "";
    string newMomUser = "";

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
                HttpResponseMessage accountResponse = await client.GetAsync(URL + "/Accounts");
                HttpResponseMessage momResponse = await client.GetAsync(URL + "/Mothers");
                HttpResponseMessage relationshipResponse = await client.GetAsync(URL + "/Relationships");

                changeNameBtn.IsEnabled = false;
                AddMomBtn.IsEnabled = false;
                RemoveMom.IsEnabled = false;

                string[]? notifSettings = null;
                if (account.Notifs != null) notifSettings = account.Notifs.Split(",");

                if (notifSettings != null && notifSettings.Length == 5) 
                {
                    NewSwitch.IsToggled = bool.Parse(notifSettings[0].ToLower());
                    AssignedSwitch.IsToggled = bool.Parse(notifSettings[1].ToLower());
                    ProgressSwitch.IsToggled = bool.Parse(notifSettings[2].ToLower());
                    CompleatedSwitch.IsToggled = bool.Parse(notifSettings[3].ToLower());
                    ApprovedSwitch.IsToggled = bool.Parse(notifSettings[4].ToLower());
                }

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
						allAccounts.RemoveAll(acc => acc.MomId == null);

                        if (relationships != null)
                        {
                            linkedR = relationships.ToList();

                            linkedR.RemoveAll(r => r.ChildId != account.Id);


                            var relatedMomIds = linkedR
                                .Where(r => r.MomId.HasValue)
                                .Select(r => r.MomId.Value)
                                .ToHashSet();

                            linkedMoms = allMoms.ToList();
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

                                Thread.Sleep(100);

                                changeNameBtn.IsEnabled = true;
                                AddMomBtn.IsEnabled = true;
                                RemoveMom.IsEnabled = true;
                            }
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
        using (HttpClient client = new HttpClient())
        {
            try
            {
                account.FName = fName.Text;
                account.LName = lName.Text;

                HttpResponseMessage response1 = await client.PutAsJsonAsync($"{URL}/Children/{account.Id}", account);

                if (response1.IsSuccessStatusCode )
                {
                    changeNameBtn.Text = "Name changed";
                }
                else
                {
                    changeNameBtn.Text = "Failed";
                }
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

    private async void AddMomClicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                Account acc = allAccounts.SingleOrDefault(a => a.Username == newMomUser);

                if (acc != null)
                {
                    if (allMoms == null || allMoms.Count < 1) throw new Exception("no moms");
                    Mother mom = allMoms.SingleOrDefault(m => m.Id == acc.MomId);

                    if (mom != null)
                    {
                        if (linkedMoms.Contains(mom))
                        {
                            AddMomBtn.Text = "Mom Already Added";
                        }
                        else
                        {
                            Relationship relationship = new Relationship();

                            int newID = 0;
                      
                            foreach (Relationship r in relationships)
                            {
                                if (r.Id > newID) newID = r.Id;
                            }

                            relationship.Id = newID + 1;
                            relationship.MomId = mom.Id;
                            relationship.ChildId = account.Id;
                            relationship.Mom = null;
                            relationship.Child = null;

                            await DisplayAlert("relationship", JsonConvert.SerializeObject(relationship), "close");

                            HttpResponseMessage response3 = await client.PostAsJsonAsync(URL + "/Relationships", relationship);

                            if (response3.IsSuccessStatusCode)
                            {
                                AddMomBtn.Text = "Mom Added";
                                Thread.Sleep(100);

                                Application.Current.MainPage = new NavigationPage(new ChildMenu(account));
                            }
                            else
                            {
                                AddMomBtn.Text = "Failed";
                            }
                        }
                    }
                    else
                    {
                        AddMomBtn.Text = "Mom not found";
                    }
                }
                else
                {
                    AddMomBtn.Text = "Invalid mom";
                }
            }
            catch (Exception ex)
            {
                AddMom.Text = ex.Message;

                Console.WriteLine("\n-------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------------\n");
            }
        }
    }

    private async void RemoveMomClicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                if (momSelect.SelectedItem == null) RemovedMomBtn.Text = "No mom Selected";
                else
                {
                    string[] selectedMom = momSelect.SelectedItem.ToString().Split(" ");
                    Mother mom = linkedMoms.Single(m => m.Id == Int32.Parse(selectedMom[0]));

                    Relationship relationship = linkedR.Single(r => r.MomId == mom.Id);

                    relationship.ChildId = null;
                    relationship.MomId = null;

                    HttpResponseMessage response = await client.PutAsJsonAsync($"{URL}/Relationships/{relationship.Id}", relationship);

                    if (response.IsSuccessStatusCode)
                    {
                        RemovedMomBtn.Text = "Mom Removed";
                        Thread.Sleep(100);

                        if (Application.Current != null) Application.Current.MainPage = new NavigationPage(new ChildMenu(account));
                    }
                    else
                    {
                        RemovedMomBtn.Text = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                RemoveMom.Text = ex.Message;

                Console.WriteLine("\n-------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------------\n");
            }
        }
    }

    private async void SetNotifsClicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            List<string> newSettings = new List<string>();

            try
            {
                newSettings.Add(NewSwitch.IsToggled.ToString());
                newSettings.Add(AssignedSwitch.IsToggled.ToString());
                newSettings.Add(ProgressSwitch.IsToggled.ToString());
                newSettings.Add(CompleatedSwitch.IsToggled.ToString());
                newSettings.Add(ApprovedSwitch.IsToggled.ToString());

                string newSettingAll = string.Join(",", newSettings);

                account.Notifs = newSettingAll;

                HttpResponseMessage response1 = await client.PutAsJsonAsync($"{URL}/Children/{account.Id}", account);

                if (response1.IsSuccessStatusCode)
                {
                    SetNotifsBtn.Text = "Notifcations Changed";
                }
                else
                {
                    SetNotifsBtn.Text = "Failed";
                }
            }
            catch (Exception ex)
            {
                NotifText.Text = ex.Message;

                Console.WriteLine("\n-------------------------------------------------------------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------------\n");
            }
        }
    }
}