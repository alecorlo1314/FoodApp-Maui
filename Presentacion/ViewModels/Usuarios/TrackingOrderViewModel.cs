using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Location.Queries.GetLatLongLocation;
using MediatR;
using System.Globalization;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class TrackingOrderViewModel(IMediator _mediator) : ObservableObject
{
    [ObservableProperty]
    private string restaurantName = "Uttora Coffee House";

    [ObservableProperty]
    private string restaurantImage = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5";

    [ObservableProperty]
    private string orderDateFormatted = "06 Sept, 10:00pm";

    [ObservableProperty]
    private string orderSummaryText = "2x Burger\n4x Sandwich";

    [ObservableProperty]
    private string estimatedTime = "20 min";

    [ObservableProperty]
    private string courierName = "Robert F.";

    [ObservableProperty]
    private string courierImage = "https://images.unsplash.com/photo-1534528741775-53994a69daeb";

    [ObservableProperty]
    private HtmlWebViewSource mapHtmlSource;

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
            await Task.WhenAll(
                LoadMapHtml(cancellationToken)
                );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Error al cargar los datos", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

private async Task LoadMapHtml(CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    try
    {
        // Obtener coordenadas mediante MediatR
        var result = await _mediator.Send(new GetLatLongQuery(), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        if (result.IsFailure || result.Value is null)
            return;

        var latitude = result.Value.Latitude;
        var longitude = result.Value.Longitude;

        // Evitar coordenadas inválidas por falta de ubicación
        if (latitude == 0 && longitude == 0)
            return;

        // Convertir utilizando punto decimal para JavaScript
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        string htmlContent = $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8">
                <meta name="viewport"
                      content="width=device-width, initial-scale=1.0">

                <link rel="stylesheet"
                      href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css">

                <style>
                    html, body, #map {
                        margin: 0;
                        padding: 0;
                        width: 100%;
                        height: 100%;
                    }
                </style>
            </head>
            <body>

                <div id="map"></div>

                <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>

                <script>
                    // Coordenadas obtenidas desde .NET MAUI
                    const lat = {{lat}};
                    const lon = {{lon}};

                    const map = L.map('map', {
                        zoomControl: false
                    }).setView([lat, lon], 15);

                    L.tileLayer(
                        'https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png?key=cb1_4s44_1_78c0d2e601dd893af7096ebf',
                        {
                            maxZoom: 19,
                            subdomains: 'abcd',
                            attribution: '&copy; OpenStreetMap contributors &copy; CARTO'
                        }
                    ).addTo(map);

                    L.marker([lat, lon])
                        .addTo(map)
                        .bindPopup("Tu ubicación actual");
                </script>

            </body>
            </html>
            """;

        MapHtmlSource = new HtmlWebViewSource
        {
            Html = htmlContent
        };
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
        throw;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine(
            $"Error al cargar mapa: {ex.Message}");
    }
}


[RelayCommand]
    private async Task CallCourierAsync()
    {
        await Shell.Current.DisplayAlertAsync("Llamar", "Iniciando llamada con el repartidor...", "OK");
    }

    [RelayCommand]
    private async Task ChatCourierAsync()
    {
        await Shell.Current.DisplayAlertAsync("Chat", "Abriendo chat con el repartidor...", "OK");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}