using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SignHmacTutorial.Models;
using SignHmacTutorial.Services;

namespace SignHmacTutorial.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private string _secretKey = string.Empty;
    private ProductType _selectedProduct = ProductType.AIA;

    private string _aitSapSoldToAccount = "AIPROD1RQ";
    private string _aitEmsId = "199972";
    private string _aitEmFirmId = "1RQ";
    private string _aitProductDomain = "AITM";

    private string _aiaEmsId = "199972";
    private string _aiaSapSoldToAccount = "AIPROD1RQ";
    private string _aiaEmFirmId = "1RQ";

    private string _rawBody = string.Empty;
    private string _hmacBase64 = string.Empty;
    private string _errorMessage = string.Empty;

    public MainWindowViewModel()
    {
        Products = Enum.GetValues<ProductType>();
        GenerateCommand = new RelayCommand(Generate);
        SelectedProduct = ProductType.AIA;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IEnumerable<ProductType> Products { get; }

    public ICommand GenerateCommand { get; }

    public string SecretKey
    {
        get => _secretKey;
        set => SetField(ref _secretKey, value);
    }

    public ProductType SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (!SetField(ref _selectedProduct, value))
                return;

            OnPropertyChanged(nameof(IsAitSelected));
            OnPropertyChanged(nameof(IsAiaSelected));
            ErrorMessage = string.Empty;
        }
    }

    public bool IsAitSelected => SelectedProduct == ProductType.AIT;
    public bool IsAiaSelected => SelectedProduct == ProductType.AIA;

    // AIT fields
    public string AitSapSoldToAccount
    {
        get => _aitSapSoldToAccount;
        set => SetField(ref _aitSapSoldToAccount, value);
    }

    public string AitEmsId
    {
        get => _aitEmsId;
        set => SetField(ref _aitEmsId, value);
    }

    public string AitEmFirmId
    {
        get => _aitEmFirmId;
        set => SetField(ref _aitEmFirmId, value);
    }

    public string AitProductDomain
    {
        get => _aitProductDomain;
        set => SetField(ref _aitProductDomain, value);
    }

    // AIA fields
    public string AiaEmsId
    {
        get => _aiaEmsId;
        set => SetField(ref _aiaEmsId, value);
    }

    public string AiaSapSoldToAccount
    {
        get => _aiaSapSoldToAccount;
        set => SetField(ref _aiaSapSoldToAccount, value);
    }

    public string AiaEmFirmId
    {
        get => _aiaEmFirmId;
        set => SetField(ref _aiaEmFirmId, value);
    }

    public string RawBody
    {
        get => _rawBody;
        private set => SetField(ref _rawBody, value);
    }

    public string HmacBase64
    {
        get => _hmacBase64;
        private set => SetField(ref _hmacBase64, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    private void Generate()
    {
        ErrorMessage = string.Empty;
        RawBody = string.Empty;
        HmacBase64 = string.Empty;

        if (string.IsNullOrWhiteSpace(SecretKey))
        {
            ErrorMessage = "SecretKey is required.";
            return;
        }

        try
        {
            string rawBody;

            if (SelectedProduct == ProductType.AIT)
            {
                if (string.IsNullOrWhiteSpace(AitSapSoldToAccount) ||
                    string.IsNullOrWhiteSpace(AitEmsId) ||
                    string.IsNullOrWhiteSpace(AitEmFirmId) ||
                    string.IsNullOrWhiteSpace(AitProductDomain))
                {
                    ErrorMessage = "All AIT fields are required.";
                    return;
                }

                if (!long.TryParse(AitEmsId, out var emsIdNumber))
                {
                    ErrorMessage = "AIT emsId must be a number (e.g., 199972).";
                    return;
                }

                rawBody = PayloadBuilder.BuildAitJson(
                    sapSoldToAccount: AitSapSoldToAccount.Trim(),
                    emsId: emsIdNumber,
                    emFirmId: AitEmFirmId.Trim(),
                    productDomain: AitProductDomain.Trim());
            }
            else
            {
                if (string.IsNullOrWhiteSpace(AiaEmsId) ||
                    string.IsNullOrWhiteSpace(AiaSapSoldToAccount) ||
                    string.IsNullOrWhiteSpace(AiaEmFirmId))
                {
                    ErrorMessage = "All AIA fields are required.";
                    return;
                }

                rawBody = PayloadBuilder.BuildAiaJson(
                    emsId: AiaEmsId.Trim(),
                    sapSoldToAccount: AiaSapSoldToAccount.Trim(),
                    emFirmId: AiaEmFirmId.Trim());
            }

            RawBody = rawBody;
            HmacBase64 = HmacSigner.ComputeHmacBase64(SecretKey, RawBody);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
