using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

using System;
using System.Linq;

namespace EdwardHsu.Lab.InfluxDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = "THIS_TOKEN_JUST_FOR_TEST";
            var client = InfluxDBClientFactory.Create("http://influxdb:8086", token);

            var now = DateTime.UtcNow;

            InitMockData(now, client);
            var result = client.GetQueryApi().QueryAsync(@$"
                from(bucket: ""test"")
                  |> range(start: {now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")} , stop: {now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                  |> filter(fn: (r) => r[""_field""] == ""amount"")
                  |> aggregateWindow(every: 5m, fn: sum)
            ", "test").Result;

            var every5minuteSum = result.SelectMany(x => x.Records.Select(y => (time: y.GetTime(), sum: y.GetValue())));

            foreach (var item in every5minuteSum)
            {
                Console.WriteLine($"Time: {item.time}\tSum: {item.sum}");
            }
        }

        static void InitMockData(DateTime now, InfluxDBClient client)
        {
            using (var writeApi = client.GetWriteApi())
            {
                var rand = new Random((int)now.Ticks);
                for (DateTime time = now.AddHours(-1); time <= now; time = time.AddSeconds(rand.Next(1, 60)))
                {
                    writeApi.WritePoint(
                        bucket: "test",
                        org: "test",
                        PointData.Measurement("transaction") // 計量命名為交易
                            .Field("amount", rand.Next(1, 100000)) // 交易量
                            .Timestamp(time, WritePrecision.Ns) // 時間戳記，精度為NS，TIME必須為UTC
                    );
                }
                writeApi.Flush();
            }
        }
    }
}
