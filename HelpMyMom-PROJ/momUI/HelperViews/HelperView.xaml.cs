using momUI.HelperViews;

namespace momUI;

public partial class HelperView : ContentPage
{
	public HelperView()
	{
		InitializeComponent();
	}

    async private void OpenChatsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperCurrentChats());
    }

    async private void OpenTicketsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperAvailableTickets());
    }

    async private void OpenProfileButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelperOpenProfile());
    }
}