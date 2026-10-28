using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class PersonalInfoViewModel : ObservableObject
{
    [RelayCommand]
    private async Task GoBackAsync() => 
        await Shell.Current.GoToAsync("..");
}
