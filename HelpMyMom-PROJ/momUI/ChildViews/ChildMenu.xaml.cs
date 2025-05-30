using momUI.models;

namespace momUI;

public partial class ChildMenu : ContentPage
{
    string URL = $"https://momapi20250409124316-bqevbcgrd7begjhy.canadacentral-01.azurewebsites.net/api";
    Child account;
    Accessibility a;

    int normalFont = 15;

    int titleFont = 20;
    int headerFont = 10;
    int medBtnFont = 15;

    public ChildMenu(Child acc)
	{
		InitializeComponent();

        a = Accessibility.getAccessibilitySettings();
        normalFont = a.fontsize;

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

        loggedIn.FontSize = normalFont + headerFont;
        LogOutBtn.FontSize = normalFont + medBtnFont;
        CreateBtn.FontSize = normalFont + titleFont;
        AssignBtn.FontSize = normalFont + titleFont;
        ProgressBtn.FontSize = normalFont + titleFont;
        AccountBtn.FontSize = normalFont + titleFont;
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

    private void testClicked(object sender, EventArgs e)
    {
        Ticket t = new Ticket();
        t.Id = -1;
        EmailServices.SendDenyMessage("hmmprojectchild@hotmail.com", "hmmprojectmom@hotmail.com", "Egg man", t);
    }

    private void NothingClicked(object sender, EventArgs e)
    {

    }

}