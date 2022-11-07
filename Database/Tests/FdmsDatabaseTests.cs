using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using FDMS.DAL;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Tests
{
    [TestClass]
    public class FdmsDatabaseTests
    {
        public static FdmsDatabase db = new FdmsDatabase();

        public static List<TelemetryRecordDAL> TestData = new List<TelemetryRecordDAL>();

        [TestMethod]
        public void InsertRecordsIsSuccessful()
        {
            foreach(var record in TestData)
            {
                var r = db.Insert(record);
                Assert.IsTrue(r.Success);
            }
        }

        [TestMethod]
        public void SelectOneRecordIsSuccessful()
        {
            var insertResult = db.Insert(TestData[0]);
            Assert.IsTrue(insertResult.Success);

            var selectResult = db.Select(TestData[0].AircraftTailNum, 1);
            Assert.IsTrue(selectResult.Success);
            Assert.IsTrue(selectResult.Records.Count == 1);
        }


        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            var r = db.Connect("Server=(localdb)\\MSSQLLocalDB;User ID=FDMS_User;Password=FDMS_Password;Database=FDMS_Server");
            Assert.IsTrue(r.Success);

            string text;
            using (TextReader reader = new StreamReader(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "testData.txt"))))
            {
                text = reader.ReadToEnd();
            }

            text = text.Replace('_', '-');

            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[][] parsedLines = lines.ToList().Select(l => l.Split(',')).ToArray();

            foreach(string[] l in parsedLines)
            {
                TestData.Add(
                    new TelemetryRecordDAL(
                    )
                    {
                        AircraftTailNum=l[0],
                        Timestamp=DateTime.Parse(l[1]),
                        Accel_X=float.Parse(l[2]),
                        Accel_Y=float.Parse(l[3]),
                        Accel_Z=float.Parse(l[4]),
                        Weight=float.Parse(l[5]),
                        Altitude=float.Parse(l[6]),
                        Pitch=float.Parse(l[7]),
                        Bank=float.Parse(l[8])
                    }
                );
            }
        }

        [AssemblyCleanup]
        public static void DiscconectFromLocalDB()
        {
            db.Disconnect();
        }
    }
}
