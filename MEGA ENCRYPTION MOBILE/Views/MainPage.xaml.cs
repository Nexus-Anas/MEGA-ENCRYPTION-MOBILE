using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using MEGA_ENCRYPTION_MOBILE.Views;
using MauiPopup;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace MEGA_ENCRYPTION_MOBILE;

public partial class MainPage : ContentPage
{
    //Global variables
    private readonly CancellationTokenSource _cancel = new();
    private readonly IFileSaver _fileSaver;
    private string _path;



    //Constructors
    public MainPage() { }
    public MainPage(IFileSaver fileSaver)
	{
		InitializeComponent();
        _fileSaver = fileSaver;
    }




    //Upload logic
    private async void btnUpload_Clicked(object sender, EventArgs e)
    {
        var options = new PickOptions { PickerTitle = "Pick a file please" };
        var file = await FilePicker.PickAsync(options);

        if (file is null)
        {
            lblInfo.Text = "No file picked.";
            lblInfo.TextColor = Color.FromArgb("#E82F2F");
            return;
        }

        _path = file.FullPath;
        lblInfo.Text = file.FileName;
        lblInfo.TextColor = Color.FromArgb("#0A95CD");
    }




    //Encryption logic
    private async void btnEnc_Clicked(object sender, EventArgs e)
    {
        if (!ValidateFields())
            return;

        ShowPopup();
        await Task.Delay(500);
        await Cipher.Encrypt(_path, txtPsw.Text, _fileSaver, _cancel);
        ClosePopup();
        DisplaySnackbar("File Encrypted with success!");
    }




    //Decryption logic
    private async void btnDec_Clicked(object sender, EventArgs e)
    {
        if (!ValidateFields())
            return;

        ShowPopup();
        await Task.Delay(500);
        await Cipher.Decrypt(_path, txtPsw.Text, _fileSaver, _cancel);
        ClosePopup();
        DisplaySnackbar("File Decrypted with success!");
    }




    //Popup call
    static void ShowPopup()
        => PopupAction.DisplayPopup(new LoadingScreen());
    static void ClosePopup()
        => PopupAction.ClosePopup();




    //Fields validation
    private bool ValidateFields()
    {
        if (string.IsNullOrEmpty(txtPsw.Text))
        {
            Toast.Make("Please type a password!")?.Show(_cancel.Token);
            return false;
        }
        return true;
    }




    //Snackbar call
    private static void DisplaySnackbar(string message)
    {
        var snackbar = Snackbar.Make(message, action: null, null,
                                        TimeSpan.FromSeconds(5), new SnackbarOptions
                                        {
                                            TextColor = Colors.White,
                                            BackgroundColor = Colors.DodgerBlue,
                                            Font = Microsoft.Maui.Font.SystemFontOfSize(16),
                                            CornerRadius = new CornerRadius(15, 15, 15, 15)
                                        });
        snackbar.Show();
    }
}