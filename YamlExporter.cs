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

        public static void WriteObjToYaml(string loc, object obj)
        {
            WriteStringTo(loc,BuildYaml(obj));
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

        public static void WriteStringTo(string loc, string content)
        {
            try
            {
                // Create or overwrite the file with the specified content.
                File.WriteAllText(loc, content);

                Console.WriteLine("String successfully written to " + loc);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to " + ex.Message);
            }
        }
    }
}
