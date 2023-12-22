using DataAccess;

namespace BusinessLogic;

public class LogicHandler : IDisposable
{
    public LogicHandler(string filename = "database.json")
    {
        readWriter = new DatabaseReadWriter<DataStorage>(filename);
        DataStorage data = readWriter.Read();
        trains = data.Trains.ToList();
        reservations = data.Reservations.ToList();
    }

    public void Dispose()
    {
        readWriter.Dispose();
    }

    private void SaveChanges()
    {
        readWriter.Save(new DataStorage()
        {
            Trains = trains.ToList(),
            Reservations = reservations.ToList()
        });
    }

    public IEnumerable<Train> GetTrains(string name)
    {
        return trains.Where(t => t.Name.Contains(name));
    }

    public void ChangeTrainName(Guid id, string name)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == id);
        if (train == null)
            throw new ArgumentException("Train not found");
        train.Name = name;
        SaveChanges();
    }

    public void ChangeTrainStartDate(Guid id, DateTime date)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == id);
        if (train == null)
            throw new ArgumentException("Train not found");
        train.StartDate = date;
        SaveChanges();
    }

    public void ChangeTrainEndDate(Guid id, DateTime date)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == id);
        if (train == null)
            throw new ArgumentException("Train not found");
        train.EndDate = date;
        SaveChanges();
    }

    public void DeleteTrain(Guid id)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == id);
        if (train == null)
            throw new ArgumentException("Train not found");
        if (reservations.Any(r => r.TrainId == id))
            throw new ApplicationException("Cannot delete train with reservations");
        trains.Remove(train);
        SaveChanges();
    }

    public void TrainStationChanged(Guid trainId, Guid stationId, string name, DateTime? time)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == trainId);
        if (train == null)
            throw new ArgumentException("Train not found");
        if (stationId == Guid.Empty)
        {
            train.Schedule.Add(new Train.Station()
            {
                Id = Guid.NewGuid(),
                Name = name,
                ArrivalTime = time.Value
            });
        }
        else
        {
            Train.Station? station = train.Schedule.FirstOrDefault(s => s.Id == stationId);
            if (station == null)
                throw new ArgumentException("Station not found");
            if (time == null)
            {
                train.Schedule.Remove(station);
                return;
            }
            station.Name = name;
            station.ArrivalTime = time.Value;
        }
        SaveChanges();
    }

    public void TrainCarriageChanged(Guid trainId, Guid carriageId, int number, int capacity)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == trainId);
        if (train == null)
            throw new ArgumentException("Train not found");
        if (carriageId == Guid.Empty)
        {
            train.Carriages.Add(new Train.Carriage()
            {
                Id = Guid.NewGuid(),
                Number = number,
                Capacity = capacity,
                Reserved = 0
            });
        }
        else
        {
            Train.Carriage? carriage = train.Carriages.FirstOrDefault(c => c.Id == carriageId);
            if (carriage == null)
                throw new ArgumentException("Carriage not found");
            if (carriage.Reserved > 0 && capacity < carriage.Reserved)
                throw new ApplicationException("Capacity cannot be less than reserved seats");
            if (capacity == -1)
            {
                if (carriage.Reserved > 0)
                    throw new ApplicationException("Cannot delete carriage with reserved seats");
                train.Carriages.Remove(carriage);
                return;
            }
            carriage.Number = number;
            carriage.Capacity = capacity;
        }
        SaveChanges();
    }

    public void AddNewTrain(string name, List<Train.Station> schedule, List<Train.Carriage> carriages, DateTime startDate, DateTime endDate)
    {
        trains.Add(new Train()
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartDate = startDate,
            EndDate = endDate,
            //Deep copy
            Schedule = schedule.Select(s => new Train.Station()
            {
                Id = Guid.NewGuid(),
                Name = s.Name,
                ArrivalTime = s.ArrivalTime
            }).ToList(),
            //Deep copy
            Carriages = carriages.Select(c => new Train.Carriage()
            {
                Id = Guid.NewGuid(),
                Number = c.Number,
                Capacity = c.Capacity,
                Reserved = 0
            }).ToList()
        });
        SaveChanges();
    }

    public int GetFreeSeats(Guid trainId, Guid carriageId)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == trainId);
        if (train == null)
            throw new ArgumentException("Train not found");
        Train.Carriage? carriage = train.Carriages.FirstOrDefault(c => c.Id == carriageId);
        if (carriage == null)
            throw new ArgumentException("Carriage not found");
        return carriage.Free;
    }

    public void ChangeReservationSeats(Guid reservationId, Guid carriageId, int seats)
    {
        Reservation? reservation = reservations.FirstOrDefault(r => r.Id == reservationId);
        if (reservation == null)
            throw new ArgumentException("Reservation not found");
        if (reservation.ReservedSeats.ContainsKey(carriageId) && seats > reservation.ReservedSeats[carriageId] + GetFreeSeats(reservation.TrainId, carriageId))
            throw new ArgumentException("Cannot reserve more seats than available");
        Train? train = trains.FirstOrDefault(t => t.Id == reservation.TrainId);
        if (train == null)
            throw new ArgumentException("Train not found");
        Train.Carriage? carriage = train.Carriages.FirstOrDefault(c => c.Id == carriageId);
        if (carriage == null)
            throw new ArgumentException("Carriage not found");
        if (reservation.ReservedSeats.ContainsKey(carriageId))
            carriage.Reserved -= reservation.ReservedSeats[carriageId];
        carriage.Reserved += seats;
        reservation.ReservedSeats[carriageId] = seats;
        SaveChanges();
    }

    public Reservation AddNewReservation(Guid trainId, Dictionary<Guid, int> reservedSeats)
    {
        Reservation reservation = new()
        {
            Id = Guid.NewGuid(),
            TrainId = trainId,
            Name = "New reservation",
            ReservedSeats = reservedSeats
        };
        reservations.Add(reservation);
        SaveChanges();
        return reservation;
    }

    public Train GetTrain(Guid id)
    {
        Train? train = trains.FirstOrDefault(t => t.Id == id);
        if (train == null)
            throw new ArgumentException("Train not found");
        return train;
    }

    public IEnumerable<Reservation> GetReservations()
    {
        return reservations;
    }

    public void DeleteReservation(Guid id)
    {
        Reservation? reservation = reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
            throw new ArgumentException("Reservation not found");
        reservations.Remove(reservation);
        Train? train = trains.FirstOrDefault(t => t.Id == reservation.TrainId);
        if (train == null)
            throw new ArgumentException("Train not found");
        foreach (var (carriageId, seats) in reservation.ReservedSeats)
        {
            Train.Carriage? carriage = train.Carriages.FirstOrDefault(c => c.Id == carriageId);
            if (carriage == null)
                throw new ArgumentException("Carriage not found");
            carriage.Reserved -= seats;
        }
        SaveChanges();
    }

    private struct DataStorage
    {
        public DataStorage()
        {
            Trains = new List<Train>();
            Reservations = new List<Reservation>();
        }

        public List<Train> Trains { get; set; }
        public List<Reservation> Reservations { get; set; }
    }

    private DatabaseReadWriter<DataStorage> readWriter;
    private List<Train> trains { get; set; }
    private List<Reservation> reservations { get; set; }
}
