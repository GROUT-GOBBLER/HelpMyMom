
using momUI.models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace momUI;


public partial class HelperAccountCreation : ContentPage
{

    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    List<Spec> GlobalSpecList;
    ObservableCollection<Spec> SelectedSpecList = new ObservableCollection<Spec>();
    public HelperAccountCreation()
	{
		InitializeComponent();
        DateOnly Current = new DateOnly();
        Current = DateOnly.FromDateTime(DateTime.Now);
        
    }

    
    protected override async void OnAppearing()

    {
        /*
        Title: 35
        Header: 25
        Normal: 15
        Buttons:
        Small: 20
        Med: 30
        Large: 35
        */
        base.OnAppearing();
        //SearchResultList.IsRefreshing = true;
        Accessibility a = Accessibility.getAccessibilitySettings();
        UserL.FontSize = a.fontsize;
        UsernameEntry.FontSize = a.fontsize;
        LNameL.FontSize = a.fontsize;
        FNameL.FontSize = a.fontsize;
        PasswordEntry.FontSize = a.fontsize;
        PasswordL.FontSize = a.fontsize;
        EmailL.FontSize = a.fontsize;
        EmailEntry.FontSize = a.fontsize;
        FirstNameEntry.FontSize = a.fontsize;
        LastNameEntry.FontSize = a.fontsize;
        CreateAccountButton.FontSize = a.fontsize + 10;
        ErrorLabel.FontSize = a.fontsize;
        specailL.FontSize = a.fontsize;
        SelSpecailL.FontSize = a.fontsize;
        DOBL.FontSize = a.fontsize;
        DescL.FontSize = a.fontsize;
        descriptionEditor.FontSize = a.fontsize;

       }



        using (HttpClient client = new HttpClient())
        {
            try
            {


                HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Specs"}");
                String json = await response2.Content.ReadAsStringAsync();

                List<Spec> specList = JsonConvert.DeserializeObject<List<Spec>>(json);
                
                if (specList == null || specList.Count < 1)
                {
                    ErrorLabel.Text = "Error in Spec";
                    SearchResultList.IsRefreshing = false;
                    return;
                }

                GlobalSpecList = specList.ToList();

                SearchResultList.ItemsSource = GlobalSpecList;
                
                SelectedList.ItemsSource = SelectedSpecList;

               // SearchResultList.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "Error";
            }
            finally
            {
                SearchResultList.IsRefreshing = true;
                SearchResultList.IsRefreshing = false;
            }

            
        }
       
    }


    async private void CreateAccountButton_Clicked(object sender, EventArgs e)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response2 = await client.GetAsync($"{URL}/{"Helpers"}");
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Connection Error has occured";
                return;
            }
            String json = await response2.Content.ReadAsStringAsync();

            List<Helper> mList = JsonConvert.DeserializeObject<List<Helper>>(json);

            Helper helper = new Helper();
            Account account = new Account();

            helper.Id = (mList[mList.Count - 1].Id + 1);
            helper.FName = FirstNameEntry.Text;
            helper.LName = LastNameEntry.Text;
            helper.Email = EmailEntry.Text;
            String specString = "";
            int i = 0;
            foreach (Spec item in SelectedSpecList)
            {
                if (i < SelectedSpecList.Count - 1)
                {

                    specString += item.Id + ", ";
                }
                else
                {
                    specString += item.Id;
                }

                    i++;
            }
          
            helper.Specs = specString; 
            /* String dob = "2000/1/1"; //DOBEntry.Text;
             if (!dob.Contains("/"))
             {
                 ErrorLabel.Text = "Needs / between yyyy/mm/dd";
                 return;
             }
             String[] d_o_b = dob.Split('/');
             if (d_o_b.Length != 3)
             {
                 ErrorLabel.Text = "Needs / between yyyy/mm/dd";
                 return;
             }
             if (d_o_b[0].Length != 4)
             {
                 ErrorLabel.Text = "Needs / between yyyy/mm/dd";
                 return;
             }*/

            // int d = Convert.ToInt32(d_o_b[2]);
            //  int m = Convert.ToInt32(d_o_b[1]);
            //  int y = Convert.ToInt32(d_o_b[0]);
            //DateOnly DateOfBirth = new DateOnly(y,m,d);
            DateOnly DateOfBirth = DateOnly.FromDateTime(DatePicker.Date);
            DateOnly Current = new DateOnly();
             Current = DateOnly.FromDateTime(DateTime.Now);
             if (Current.Year - DateOfBirth.Year < 16)
             {
                  ErrorLabel.Text = "You must be at least 16 to apply to be a helper";
                  return;
             }


            helper.Dob = DateOfBirth;
            if(descriptionEditor.Text.Length > 800)
            {
                ErrorLabel.Text = "Your description is too long";
                return;
            }

            helper.Description = descriptionEditor.Text;
            account.HelperId = helper.Id;
            account.Username = UsernameEntry.Text;
            account.Password = PasswordEntry.Text;

            if (account.Username == null)
            {
                ErrorLabel.Text = "Please enter a username";
                return;
            }
            else if (helper.FName == null)
            {
                ErrorLabel.Text = "Please enter a first name";
                return;
            }
            else if (helper.LName == null)
            {
                ErrorLabel.Text = "Please enter a last name";
                return;
            }
            else if (helper.Email == null)
            {
                ErrorLabel.Text = "Please enter an email";
                return;
            }

            response2 = await client.PostAsJsonAsync($"{URL}/{"Helpers"}", helper);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in helper creation";
                return;

            }
            response2 = await client.PostAsJsonAsync($"{URL}/{"Accounts"}", account);
            if (!response2.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Error in account creation";
                response2 = await client.DeleteAsync($"{URL}/{"Helpers"}/{helper.Id}");
                return;

            }
            ErrorLabel.Text = "Success";

        }

    }
  
   

    private void SearchResultList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        SelectedList.BeginRefresh();
        Spec s = e.SelectedItem as Spec;
        ErrorLabel.Text = s.Name;
        SelectedSpecList.Add(s);
 

        SelectedList.EndRefresh();

    }


    private void SelectedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        Spec s = e.SelectedItem as Spec;
        SelectedSpecList.Remove(s);
        
    }
    /* private void descriptionEditor_TextChanged(object sender, TextChangedEventArgs e)
{

}

private void descriptionEditor_Completed(object sender, EventArgs e)
{

}
TextChanged="descriptionEditor_TextChanged"
Completed="descriptionEditor_Completed"


*/
}