namespace AzureFunctions.Models;

public record TransportEntity(
    DateOnly TransportedDate,
    string FacilityId,
    string ParcelId,
    DateTimeOffset TransportedAt,
    string LocationFrom,
    string LocationTo,
    long TimeSpentSeconds,
    string DeviceId,
    string TransportId)
{
    public static TransportEntity FromTransport(Transport transport) =>
        new(TransportedDate: DateOnly.FromDateTime(dateTime: transport.TransportedAt.UtcDateTime),
            FacilityId: transport.FacilityId,
            ParcelId: transport.ParcelId,
            TransportedAt: transport.TransportedAt,
            LocationFrom: transport.LocationFrom,
            LocationTo: transport.LocationTo,
            TimeSpentSeconds: transport.TransportDurationSec,
            DeviceId: transport.DeviceId,
            TransportId: transport.TransportId);
}
