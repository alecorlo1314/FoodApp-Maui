using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Presentacion.Views.Usuarios;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class ProfileViewModel : ObservableObject
{
    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task NavigateToPersonalInfoAsync() =>
        await Shell.Current.GoToAsync(nameof(PersonalInfoPage));
}
