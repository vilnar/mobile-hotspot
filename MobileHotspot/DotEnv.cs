using System;
using System.IO;

namespace MobileHotspot
{
    public static class DotEnv
    {
        public static bool Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("not found env file " + filePath);
                return false;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split('=');
                if (parts.Length != 2)
                {
                    continue;
                }

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }

            return true;
        }
    }
}
