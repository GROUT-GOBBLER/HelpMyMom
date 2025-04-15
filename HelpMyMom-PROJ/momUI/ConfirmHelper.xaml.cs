namespace momUI;

using models;

public partial class ConfirmHelper : ContentPage
{
    Child Account;
    Ticket ticket;

    public ConfirmHelper(Child acc, Ticket t = null)
	{
		InitializeComponent();

        Account = acc;
        if (t != null) ticket = t;
    }
}