using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Presentacion.Models;
using System.Collections.ObjectModel;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class AddressViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<AddressModel> addresses = new()
    {
        new AddressModel
        {
            Title = "HOME",
            Description = "2464 Royal Ln. Mesa, New Jersey\n45463",
            IconSource = "ic_home_address.png"
        },
        new AddressModel
        {
            Title = "WORK",
            Description = "3891 Ranchview Dr. Richardson,\nCalifornia 62639",
            IconSource = "ic_work_address.png"
        },
        new AddressModel
        {
            Title = "OTHER",
            Description = "Apartment 2, 2464 Royal Ln. Mesa, New Jersey\n45463",
            IconSource = "ic_other_address.png"
        }
    };

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private void AddNewAddress()
    {
    }

    [RelayCommand]
    private void EditAddress(AddressModel address)
    {
    }

    [RelayCommand]
    private void DeleteAddress(AddressModel address)
    {
        Addresses.Remove(address);
    }
}
