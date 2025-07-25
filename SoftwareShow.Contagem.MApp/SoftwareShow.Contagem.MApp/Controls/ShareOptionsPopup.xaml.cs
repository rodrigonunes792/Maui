using CommunityToolkit.Maui.Views;

namespace SoftwareShow.Contagem.MApp.Controls;

public partial class ShareOptionsPopup : Popup
{
    public ShareOptionsPopup()
    {
        InitializeComponent();
    }

    private void OnPdfClicked(object sender, EventArgs e)
    {
        Close("PDF");
    }

    private void OnExcelClicked(object sender, EventArgs e)
    {
        Close("Excel");
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close("Cancel");
    }
}