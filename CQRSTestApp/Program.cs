using System;
using System.Text.Json;

namespace CQRSTestApp
{
    public class TestObject3
    {
        public JsonDocument Document { get; set; }
    }

    public class TestObject
    {
        public JsonElement Content { get; set; }
    }

    public class ContentClass
    {
        public string One { get; set; }

        public string Two { get; set; }
    }

    public class TestObject2
    {
        public ContentClass Content { get; set; }
    }

    class Program
    {


        static void Main(string[] args)
        {
            var settings = new JsonSerializerOptions {WriteIndented = true};

            var to1 = new TestObject2
                      {
                          Content = new ContentClass
                                    {
                                        One = "Hello",
                                        Two = "World"
                                    }
                      };

            string ser = JsonSerializer.Serialize(to1, settings);

            string ser2 = JsonSerializer.Serialize(new TestObject3
                                           {
                                               Document = JsonDocument.Parse(ser)
                                           });
            JsonSerializer.ser
            var to2 = JsonSerializer.Deserialize<TestObject>(ser);

            ser = JsonSerializer.Serialize(to2, settings);

            to1 = JsonSerializer.Deserialize<TestObject2>(ser);

            var to3 = JsonSerializer.Deserialize<TestObject3>(ser2);
        }
    }
}
