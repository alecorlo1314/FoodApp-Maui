using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Users.Queries.GetProfiles;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class PersonalInfoViewModel(IMediator _mediator) : ObservableObject
{
    [ObservableProperty]
    private Profile? profile;

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (IsLoading)
            return;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsLoading = true;

            // Operaciones asíncronas independientes
            await Task.WhenAll(
                LoadProfileAsync(cancellationToken)
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync() =>
        await Shell.Current.GoToAsync("..", animate: true);

    [RelayCommand]
    public async Task LoadProfileAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Envía la petición a través de MediatR al Query Handler
            var query = new GetProfileQuery();
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsSuccess && result.Value != null)
            {
                Profile = result.Value;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", result.ErrorMessage, "OK");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}
