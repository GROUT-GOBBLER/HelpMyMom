using momUI.models;

namespace momUI;

public partial class ChildMenu : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    Child account;

    int titleFont = 35;
    int headerFont = 25;
    int normalFont = 18;
    int largeBtnFont = 40;

    int fontOffset = 0;

    public ChildMenu(Child acc)
	{
		InitializeComponent();

        if (acc != null)
        {
            account = acc;
        }
        else
        {
            account = new Child();
            account.FName = "You shouldn't";
            account.LName = "be here";
            account.Id = 2;
            account.Email = "hmmprojectchild@hotmail.com";
        }
    }

    protected override void OnAppearing()
    {
        loggedIn.Text = $"Logged in as {account.FName} {account.LName}";

        loggedIn.FontSize = normalFont + fontOffset;
        LogOutBtn.FontSize = headerFont + fontOffset;
        CreateBtn.FontSize = largeBtnFont + fontOffset;
        AssignBtn.FontSize = largeBtnFont + fontOffset;
        ProgressBtn.FontSize = largeBtnFont + fontOffset;
        AccountBtn.FontSize = largeBtnFont + fontOffset;
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

    async private void AccManagementClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChildAccountSettings(account));
    }

    private void SignOutClicked(object sender, EventArgs e)
    {
        if (Application.Current != null) Application.Current.MainPage = new NavigationPage(new MainPage());
    }

    private void NothingClicked(object sender, EventArgs e)
    {

    }

}