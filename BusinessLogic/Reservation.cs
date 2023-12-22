namespace BusinessLogic;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid TrainId { get; set; }
    public string Name { get; set; }
    // Carriage ID, Number of seats
    public Dictionary<Guid, int> ReservedSeats { get; set; }
}