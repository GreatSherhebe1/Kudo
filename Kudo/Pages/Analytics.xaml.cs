using Syncfusion.Maui.Buttons;

namespace Kudo.Pages;

public partial class Analytics : ContentPage
{
	public Analytics()
	{
		InitializeComponent();
        SfSwitch sfSwitch = new SfSwitch();
        sfSwitch.StateChanged += SfSwitch_StateChanged;
        this.Content = sfSwitch;
    }

    private void SfSwitch_StateChanged(object sender, SwitchStateChangedEventArgs e)
    {
        DisplayAlert("Message", "SUCCESS", "OK");
    }
}