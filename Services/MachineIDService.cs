using System;
using System.IO;

namespace SparkHRMS.Services
{
    public class MachineIDService
    {
        private readonly string _machineIdPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "MyApp",
            "machine.id");

        public string? GetMachineId()
        {
            if (File.Exists(_machineIdPath))
            {
                return File.ReadAllText(_machineIdPath).Trim();
            }
            else
            {
                return null;
            }
        }
    }

}
