namespace SoftwareShow.Contagem.MApp.Controls;

public partial class StandardHeader : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(StandardHeader), string.Empty);

    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(nameof(ShowBackButton), typeof(bool), typeof(StandardHeader), false);

    public static readonly BindableProperty ShowActionButtonProperty =
        BindableProperty.Create(nameof(ShowActionButton), typeof(bool), typeof(StandardHeader), false);

    public static readonly BindableProperty ActionIconProperty =
        BindableProperty.Create(nameof(ActionIcon), typeof(string), typeof(StandardHeader), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public bool ShowActionButton
    {
        get => (bool)GetValue(ShowActionButtonProperty);
        set => SetValue(ShowActionButtonProperty, value);
    }

    public string ActionIcon
    {
        get => (string)GetValue(ActionIconProperty);
        set => SetValue(ActionIconProperty, value);
    }

    public event EventHandler BackClicked;
    public event EventHandler ActionClicked;

    public StandardHeader()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
        BackClicked?.Invoke(this, e);

        // Navegação padrão
        if (BackClicked == null && Shell.Current != null)
        {
            Shell.Current.GoToAsync("..");
        }
    }


    private void OnActionClicked(object sender, EventArgs e)
    {
        ActionClicked?.Invoke(this, e);
    }
}