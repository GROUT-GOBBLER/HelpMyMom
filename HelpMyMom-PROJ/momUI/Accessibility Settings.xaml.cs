namespace momUI;

public partial class Accessibility_Settings : ContentPage
{
    public Accessibility_Settings()
    {
        InitializeComponent();
    }

    private void picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Accessibility a = Accessibility.getAccessibilitySettings();
        a.setFontSize(fontsize: Int32.Parse(picker.SelectedItem.ToString()));
        L.FontSize = a.fontsize;

    }
}