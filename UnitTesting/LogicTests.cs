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

    }
}