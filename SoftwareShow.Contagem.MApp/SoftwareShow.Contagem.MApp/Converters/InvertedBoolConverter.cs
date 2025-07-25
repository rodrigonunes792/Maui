using System.Globalization;

namespace SoftwareShow.Contagem.MApp.Converters
{
    /// <summary>
    /// Conversor para background das tabs
    /// </summary>
    public class BoolToTabBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
                return Color.FromArgb("#FFFFFF"); // Branco quando selecionado
            return Color.FromArgb("#F0F0F0"); // Cinza bem claro quando não selecionado
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para borda das tabs (linha embaixo)
    /// </summary>
    public class BoolToTabBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
                return Color.FromArgb("#333333"); // PrimaryColor quando selecionado (linha visível)
            return Colors.Transparent; // Transparente quando não selecionado (linha invisível)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para cor do texto das tabs
    /// </summary>
    public class BoolToTabTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
                return Color.FromArgb("#333333"); // PrimaryColor quando selecionado
            return Color.FromArgb("#999999"); // Cinza mais claro quando não selecionado
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para atributos da fonte das tabs
    /// </summary>
    public class BoolToFontAttributesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
                return FontAttributes.Bold;
            return FontAttributes.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para visibilidade baseada em string
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
                return !string.IsNullOrWhiteSpace(stringValue);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para formatação de data da contagem
    /// </summary>
    public class ContagemDateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
                return dateTime.ToString("dd/MM/yyyy");
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para formatação de quantidade de produtos
    /// </summary>
    public class QuantidadeProdutosConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantidade)
            {
                return quantidade == 1 ? "1 produto" : $"{quantidade} produtos";
            }
            return "0 produtos";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para cor de status da contagem
    /// </summary>
    public class ContagemStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool enviada && enviada)
                return Color.FromArgb("#27AE60"); // Verde para enviadas
            return Color.FromArgb("#F39C12"); // Laranja para em andamento
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversor para ícone de status da contagem
    /// </summary>
    public class ContagemStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool enviada && enviada)
                return "check_circle"; // Ícone de check para enviadas
            return "clock"; // Ícone de relógio para em andamento
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}