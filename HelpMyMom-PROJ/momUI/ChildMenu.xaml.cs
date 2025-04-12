namespace momUI;

public partial class ChildMenu : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    public ChildMenu()
	{
		InitializeComponent();
	}

    async private void CreateTicketClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChildTicketFactory());
    }
    private void NothingClicked(object sender, EventArgs e)
    {

    }

}