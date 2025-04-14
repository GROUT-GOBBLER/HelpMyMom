using momUI.models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace momUI;

public partial class AssignHelperPage : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    List<string> specs = new List<string>();
    List<Spec> allSpecs = new List<Spec>();
    List<SearchHelper> allHelpers = new List<SearchHelper>();
    List<Review> allReviews = new List<Review>();

    public AssignHelperPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        using (HttpClient client = new HttpClient())
        {
            List<Helper> helpers = new List<Helper>();

            helperList.IsRefreshing = true;

            try
            {
                HttpResponseMessage response = await client.GetAsync(URL + "/Helpers");
                HttpResponseMessage reviewResponse = await client.GetAsync(URL + "/Reviews");

                if (response.IsSuccessStatusCode && reviewResponse.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    string json2 = await reviewResponse.Content.ReadAsStringAsync();

                    helpers = JsonConvert.DeserializeObject<List<Helper>>(json);
                    allReviews = JsonConvert.DeserializeObject<List<Review>>(json2);

                    if (helpers.Count > 0 && helpers != null)
                    {
                        helpers.RemoveAll(h => h.FName == null || h.LName == null);
                        helpers.RemoveAll(h => h.Banned == 1);

                        foreach (Helper h in helpers)
                        {
                            SearchHelper sh = new SearchHelper();
                            sh.FName = h.FName;
                            sh.LName = h.LName;
                            sh.Description = h.Description;
                            sh.Specs = h.Specs;

                            int count = 0;
                            int calc = 0;

                            if (allReviews != null)
                            {
                                foreach (Review v in allReviews)
                                {
                                    if (v.HelperId == h.Id)
                                    {
                                        calc += (int)v.Stars;
                                        count++;
                                    }
                                }
                                if (count > 0)
                                {
                                    calc = calc / count;
                                    calc = (int)Math.Ceiling((double)calc / 2);
                                }
                            }

                            sh.Rating = calc;

                            allHelpers.Add(sh);
                        }
                    }
                    else
                    {
                        SearchHelper h = new SearchHelper();
                        h.FName = "No";
                        h.LName = "Helpers";
                        h.Description = "Something went wrong";

                        allHelpers.Add(h);
                    }

                    helperList.ItemsSource = allHelpers;
                }

                HttpResponseMessage specResponse = await client.GetAsync(URL + "/Specs");

                if (specResponse.IsSuccessStatusCode) 
                {
                    string json = await specResponse.Content.ReadAsStringAsync();

                    allSpecs = JsonConvert.DeserializeObject<List<Spec>>(json);

                    if (allSpecs.Count < 1 || allSpecs == null)
                    {
                        specs.Add("no specs found");
                    }
                    else
                    {
                        foreach (Spec s in allSpecs)
                        {
                            specs.Add(s.Name);
                        }
                    }

                    picker.ItemsSource = specs;
                }
            }
            catch (Exception ex)
            {
                Searching.Text = ex.Message;

                Console.WriteLine("\n\n");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                helperList.IsRefreshing = false;
            }
        }
    }

    private async void Reload_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            List<SearchHelper> searchedHelpers = allHelpers.ToList();
            int selectedSpec = -1;

            helperList.IsRefreshing = true;

            try
            {
                if (searchedHelpers.Count > 0 || searchedHelpers != null)
                {
                    searchedHelpers.RemoveAll(h => h.FName == null || h.LName == null);

                    if (!string.IsNullOrWhiteSpace(Searching?.Text))
                    {
                        List<SearchHelper> tempList = searchedHelpers.ToList();
                        foreach (SearchHelper h in searchedHelpers)
                        {
                            if (!h.FullName.ToLower().Contains(Searching.Text.ToLower())) tempList.Remove(h);
                        }

                        searchedHelpers = tempList;
                    }
                }

                if (picker.SelectedItem != null && allSpecs != null)
                {
                    foreach (Spec spec in allSpecs)
                    {
                        if (picker.SelectedItem.Equals(spec.Name))
                        {
                            selectedSpec = (int)spec.Id;
                            break;
                        }
                    }

                    // Only apply filter if a valid spec was found
                    if (selectedSpec != -1)
                    {
                        searchedHelpers.RemoveAll(h =>
                        {
                            if (string.IsNullOrEmpty(h.Specs)) return true;

                            var specIds = h.Specs
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(id => id.Trim());
                            return !specIds.Contains(selectedSpec.ToString());
                        });
                    }
                }

                if (searchedHelpers.Count < 1)
                {
                    SearchHelper h = new SearchHelper();
                    h.FName = "No";
                    h.LName = "Helpers";
                    h.Description = "Something went wrong";

                    searchedHelpers.Add(h);
                }

                helperList.ItemsSource = searchedHelpers;
            }
            catch (Exception ex)
            {
                Searching.Text = ex.Message;

                Console.WriteLine("\n\n");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                helperList.IsRefreshing = false;
            }
        }
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        List<SearchHelper> searchedHelpers = allHelpers.ToList();
        int selectedSpec = -1;

        helperList.IsRefreshing = true;

        try
        {
            if (searchedHelpers.Count > 0 || searchedHelpers != null)
            {
                searchedHelpers.RemoveAll(h => h.FName == null || h.LName == null);

                if (Searching.Text.Length > 0)
                {
                    List<SearchHelper> tempList = searchedHelpers.ToList();
                    foreach (SearchHelper h in searchedHelpers)
                    {
                        if (!h.FullName.ToLower().Contains(Searching.Text.ToLower())) tempList.Remove(h);
                    }

                    searchedHelpers = tempList;
                }
            }

            if (picker.SelectedItem != null && allSpecs != null)
            {
                foreach (Spec spec in allSpecs)
                {
                    if (picker.SelectedItem.Equals(spec.Name))
                    {
                        selectedSpec = (int)spec.Id;
                        break;
                    }
                }

                // Only apply filter if a valid spec was found
                if (selectedSpec != -1)
                {
                    searchedHelpers.RemoveAll(h =>
                    {
                        if (string.IsNullOrEmpty(h.Specs)) return true;

                        var specIds = h.Specs
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(id => id.Trim());
                        return !specIds.Contains(selectedSpec.ToString());
                    });
                }
            }

            if (searchedHelpers.Count < 1)
            {
                SearchHelper h = new SearchHelper();
                h.FName = "No";
                h.LName = "Helpers";
                h.Description = "Something went wrong";

                searchedHelpers.Add(h);
            }

            helperList.ItemsSource = searchedHelpers;
        }
        catch (Exception ex)
        {
            Searching.Text = ex.Message;

            Console.WriteLine("\n\n");
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            helperList.IsRefreshing = false;
        }
    }

    private void NothingClicked(object sender, EventArgs e)
    {

    }
}