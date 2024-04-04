using IPAddressAnalyzer.Types;
using System.Net;

namespace IPAddressAnalyzer;

public class ParserExtensions
{
    public Dictionary<string, string> ParseArguments(string[] arguments)
    {
        var result = new Dictionary<string, string>();
        for (int i = 0; i < arguments.Length; i = i + 2)
        {
            if (arguments.Length <= i + 1 || !arguments[i].StartsWith("--"))
                return null;
            result[arguments[i]] = arguments[i + 1];
        }
        return result;
    }

    public List<Logs> ReadLogs(string filePath)
    {       
        try
        {
            var logs = new List<Logs>();
            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(' ');
                logs.Add(new Logs
                {
                    IpAddress = parts[0],
                    TimeStamps = DateTime.Parse(parts[1])
                });
            }
            return logs;
        }
        catch 
        {
            return null;
        }
    }

    public List<Logs> FilterLogs(List<Logs> logs, Dictionary<string, string> arguments)
    {
        try
        {
            string startAddress = arguments.ContainsKey("--address-start") ? arguments["--address-start"] : string.Empty;
            string mask = arguments.ContainsKey("--address-mask") ? arguments["--address-mask"] : string.Empty;

            if (string.IsNullOrEmpty(startAddress))
                return logs;

            var ipAddress = IPAddress.Parse(startAddress);
            var maskAddress = !string.IsNullOrEmpty(mask) ? IPAddress.Parse(mask) : null;

            var filteredLogs = logs.Where(log =>
            {
                var address = IPAddress.Parse(log.IpAddress);
                if(maskAddress != null)
                {
                    var addressBytes = address.GetAddressBytes();
                    var maskBytes = maskAddress.GetAddressBytes();
                    for(int i = 0; i < maskBytes.Length; i++)
                    {
                        if ((addressBytes[i] & maskBytes[i]) != (ipAddress.GetAddressBytes()[i] & maskBytes[i]))
                            return false;
                    }
                }
                return true;
            }).ToList();

            var startTime = filteredLogs.Min(log => log.TimeStamps);
            var endTime = filteredLogs.Max(log => log.TimeStamps);

            return logs.Where(log => log.TimeStamps >= startTime && log.TimeStamps <= endTime).ToList();            
        }
        catch
        {
            return null;
        }
    }
    
    public void WriteResults(string filePath, List<Logs> logs)
    {
        try
        {
            using(var writer  = new StreamWriter(filePath))
            {
                foreach (var group in logs.GroupBy(log => log.IpAddress))
                {
                    var ipAddress = group.Key;
                    var count = group.Count();
                    var startTime = group.Min(log => log.TimeStamps);
                    var endTime = group.Max(log => log.TimeStamps);

                    writer.WriteLine($"IP адрес: {ipAddress} \nЗапросов: {count} \nПромежуток с {startTime} по {endTime}\n");
                }                
            }
        }
        catch
        {
            Console.WriteLine("Ошибка при записи в файл.");
        }
    }
}
