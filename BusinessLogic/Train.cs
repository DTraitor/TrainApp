namespace BusinessLogic;

public class Train
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Carriage> Carriages { get; set; }
    public List<Station> Schedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public class Station
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime ArrivalTime { get; set; }
    }

    public class Carriage
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public int Reserved { get; set; }
        public int Free => Capacity - Reserved;
    }
}