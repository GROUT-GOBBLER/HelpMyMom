using momUI.models;

namespace momUI;

public partial class ChildMenu : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    Child account;
    public ChildMenu(Child acc = null)
	{
		InitializeComponent();

        if (acc != null)
        {
            account = acc;
        }
        else
        {
            account = new Child();
            account.FName = "[Object]";
            account.LName = "[object]";
            account.Id = 2;
        }
    }

    protected override async void OnAppearing()
    {
        loggedIn.Text = $"Logged in as {account.FName} {account.LName}";
    }

    async private void CreateTicketClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChildTicketFactory(account));
    }

    async private void AssignHelperClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AssignHelperPage(account));
    }

    async private void ViewProgressClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TicketProgress(account));
    }

    private void NothingClicked(object sender, EventArgs e)
    {

    }

}