using BusinessLogic;

namespace UnitTesting;

public class LogicTests
{
    private const string filename = "test.json";

    [TearDown]
    public void CleanUp()
    {
        File.Delete(filename);
    }

    [Test]
    public void BusinessLogicTests()
    {
        using LogicHandler logicHandler = new(filename);
        logicHandler.AddNewTrain("Test Train", new List<Train.Station>(), new List<Train.Carriage>(), DateTime.Now, DateTime.Now);
        Assert.That(logicHandler.GetTrains("").Count(), Is.EqualTo(1));
        Train train = logicHandler.GetTrains("").First();
        DateTime startDate = train.StartDate;
        DateTime endDate = train.EndDate;
        logicHandler.TrainCarriageChanged(train.Id, Guid.Empty, 1, 20);
        Assert.That(train.Carriages.Count, Is.EqualTo(1));
        logicHandler.TrainCarriageChanged(logicHandler.GetTrains("").First().Id, logicHandler.GetTrains("").First().Carriages.First().Id, 1, -1);
        Assert.That(train.Carriages.Count, Is.EqualTo(0));
        logicHandler.TrainStationChanged(train.Id, Guid.Empty, "Test Station", DateTime.Now);
        Assert.That(train.Schedule.Count, Is.EqualTo(1));
        logicHandler.TrainStationChanged(train.Id, train.Schedule.First().Id, "Test Station", null);
        Assert.That(train.Schedule.Count, Is.EqualTo(0));
        logicHandler.ChangeTrainName(train.Id, "Test Train 2");
        Assert.That(train.Name, Is.EqualTo("Test Train 2"));
        logicHandler.ChangeTrainStartDate(train.Id, startDate.AddDays(1));
        Assert.That(train.StartDate, Is.EqualTo(startDate.AddDays(1)));
        logicHandler.ChangeTrainEndDate(train.Id, endDate.AddDays(2));
        Assert.That(train.EndDate, Is.EqualTo(endDate.AddDays(2)));
        logicHandler.AddNewReservation(train.Id, new Dictionary<Guid, int>());
        Assert.That(logicHandler.GetReservations().Count(), Is.EqualTo(1));
        Reservation reservation = logicHandler.GetReservations().First();
        Assert.That(logicHandler.GetTrain(reservation.TrainId).Name, Is.EqualTo("Test Train 2"));
        logicHandler.DeleteReservation(reservation.Id);
        Assert.That(logicHandler.GetReservations().Count(), Is.EqualTo(0));
        logicHandler.DeleteTrain(train.Id);
        Assert.That(logicHandler.GetTrains("").Count(), Is.EqualTo(0));

    }
}