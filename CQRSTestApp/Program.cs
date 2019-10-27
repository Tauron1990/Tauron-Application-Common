using System.Collections.Generic;
using System.Text.Json;
using Tauron.Application.CQRS.Common.Converter;

namespace CQRSTestApp
{
    class Program
    {


        static void Main(string[] args)
        {
            var info = new List<ObjectInfo>
                       {
                           new ObjectInfo(),
                           new ObjectInfo
                           {
                               Element = new List<string>
                                         {
                                             "Hallo",
                                             "Welt"
                                         }
                           },
                           new ObjectInfo()
                       };

            var test = JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true});

            info = JsonSerializer.Deserialize<List<ObjectInfo>>(test);
        }
    }
}
