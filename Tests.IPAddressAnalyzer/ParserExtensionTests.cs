using IPAddressAnalyzer;
using IPAddressAnalyzer.Types;

namespace Tests.IPAddressAnalyzer;

public class ParserExtensionsTests
{
    [Fact]
    public void ParseArguments_ValidArguments_ReturnsDictionary()
    {
        var parser = new ParserExtensions();
        var arguments = new string[] { "--address-start", "192.168.1.1", "--address-mask", "255.255.255.0" };

        var result = parser.ParseArguments(arguments);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("192.168.1.1", result["--address-start"]);
        Assert.Equal("255.255.255.0", result["--address-mask"]);
    }

    [Fact]
    public void ReadLogs_ValidFilePath_ReturnsListOfLogs()
    {
        var parser = new ParserExtensions();
        var filePath = "test_logs.txt";
        File.WriteAllLines(filePath, new[] { "192.168.1.1 2024-04-05T12:00:00", "192.168.1.2 2024-04-05T12:30:00" });

        var result = parser.ReadLogs(filePath);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("192.168.1.1", result[0].IpAddress);
        Assert.Equal(DateTime.Parse("2024-04-05T12:00:00"), result[0].TimeStamps);
        Assert.Equal("192.168.1.2", result[1].IpAddress);
        Assert.Equal(DateTime.Parse("2024-04-05T12:30:00"), result[1].TimeStamps);
             
        File.Delete(filePath);
    }

    [Fact]
    public void FilterLogs_ValidLogsAndArguments_ReturnsFilteredLogs()
    {        
        var parser = new ParserExtensions();
        var logs = new List<Logs>
            {
                new Logs { IpAddress = "192.168.1.1", TimeStamps = DateTime.Parse("2024-04-05T12:00:00") },
                new Logs { IpAddress = "192.168.1.2", TimeStamps = DateTime.Parse("2024-04-05T12:30:00") },
                new Logs { IpAddress = "192.168.1.3", TimeStamps = DateTime.Parse("2024-04-05T13:00:00") }
            };
        var arguments = new Dictionary<string, string>
        {
            ["--address-start"] = "192.168.1.1",
            ["--address-mask"] = "255.255.255.0"
        };
        
        var result = parser.FilterLogs(logs, arguments);
        
        Assert.NotNull(result);
        Assert.Equal("192.168.1.1", result[0].IpAddress);
        Assert.Equal(DateTime.Parse("2024-04-05T12:00:00"), result[0].TimeStamps);
    }

    [Fact]
    public void WriteResults_ValidLogsAndFilePath_WritesResultsToFile()
    {
        
        var parser = new ParserExtensions();
        var logs = new List<Logs>
            {
                new Logs { IpAddress = "192.168.1.1", TimeStamps = DateTime.Parse("2024-04-05T12:00:00") },
                new Logs { IpAddress = "192.168.1.2", TimeStamps = DateTime.Parse("2024-04-05T12:30:00") },
                new Logs { IpAddress = "192.168.1.1", TimeStamps = DateTime.Parse("2024-04-05T13:00:00") }
            };
        var filePath = "test_output.txt";

        
        parser.WriteResults(filePath, logs);

        
        Assert.True(File.Exists(filePath));

        
        File.Delete(filePath);
    }
}

