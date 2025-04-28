namespace momUI;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        Accessibility a = Accessibility.getAccessibilitySettings();
        Mom.FontSize = a.fontsize + 5;
        Child.FontSize = a.fontsize + 5;
        Helper.FontSize = a.fontsize + 5;
    }
    async private void Mom_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MomAccountCreation());
    }

    async private void Child_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChildAccountCreation());
    }

    async private void Helper_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperAccountCreation());
    }
}