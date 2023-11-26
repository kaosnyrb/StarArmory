using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace StarArmory
{
    public class YamlExporter
    {

        public static void WriteObjToYamlFile(string filePath, object obj)
        {
            WriteStringToFile(filePath,BuildYaml(obj));
        }

        public static string BuildYaml(object obj)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
            var yaml = serializer.Serialize(obj);
            System.Console.WriteLine(yaml);
            return yaml;
        }

        public static void WriteStringToFile(string filePath, string content)
        {
            try
            {
                // Create or overwrite the file with the specified content.
                File.WriteAllText(filePath, content);

                Console.WriteLine("String successfully written to file: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file: " + ex.Message);
            }
        }
    }
}
