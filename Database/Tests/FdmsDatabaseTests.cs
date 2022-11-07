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


        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            // connect to database
            var r = db.Connect("Server=(localdb)\\MSSQLLocalDB;User ID=FDMS_User;Password=FDMS_Password;Database=FDMS_Server");
            Assert.IsTrue(r.Success);


            // read in and populate TestData from file
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
        public static void Cleanup()
        {
            db.Disconnect();
        }

        [TestMethod]
        [DoNotParallelize]
        public void Insert_CanInsertMultipleRecords()
        {
            foreach(var record in TestData)
            {
                var r = db.Insert(record);
                Assert.IsTrue(r.Success);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Insert_InsertedRecordsEndsUpInDatabase()
        {
            TelemetryRecordDAL recordToInsert = new TelemetryRecordDAL()
            {
                AircraftTailNum = "ABCDEF",
                Timestamp = DateTime.Parse("1-20-1992 6:06:50"),
                Accel_X = 1.1f,
                Accel_Y = 2.2f,
                Accel_Z = 3.3f,
                Weight = 4.4f,
                Altitude = 5.5f,
                Pitch = 6.6f,
                Bank = 7.7f,
            };

            var insertResult = db.Insert(recordToInsert);
            Assert.IsTrue(insertResult.Success);

            var selectResult = db.Select("ABCDEF", 1);
            Assert.IsTrue(selectResult.Success);
            Assert.IsTrue(selectResult.Records.Count == 1);
            Assert.IsTrue(EquateRecords(recordToInsert, selectResult.Records[0]));
        }

        [TestMethod]
        [DoNotParallelize]
        public void Select_LimitOfOneWorks()
        {
            var recordsToInsert = TestData.OrderBy(r => r.AircraftTailNum).Take(2).ToList();
            var insertResult = db.Insert(recordsToInsert[0]);
            Assert.IsTrue(insertResult.Success);
            insertResult = db.Insert(recordsToInsert[1]);
            Assert.IsTrue(insertResult.Success);

            var selectResult = db.Select(TestData[0].AircraftTailNum, 1);
            Assert.IsTrue(selectResult.Success);
            Assert.IsTrue(selectResult.Records.Count == 1);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Select_LimitOfTwoWorks()
        {
            var recordsToInsert = TestData.OrderBy(r => r.AircraftTailNum).Take(3).ToList();
            var insertResult = db.Insert(recordsToInsert[0]);
            Assert.IsTrue(insertResult.Success);
            insertResult = db.Insert(recordsToInsert[1]);
            Assert.IsTrue(insertResult.Success);
            insertResult = db.Insert(recordsToInsert[2]);
            Assert.IsTrue(insertResult.Success);

            var selectResult = db.Select(TestData[0].AircraftTailNum, 2);
            Assert.IsTrue(selectResult.Success);
            Assert.IsTrue(selectResult.Records.Count == 2);
        }

        [TestMethod]
        [DoNotParallelize]
        public void Select_RetrievesLatestInsertedRecords()
        {
            var recordsToInsert = TestData.Where(r => TestData[0].AircraftTailNum == r.AircraftTailNum).ToList();

            foreach(var record in TestData)
            {
                var r = db.Insert(record);
                Assert.IsTrue(r.Success);
            }

            var selectResult = db.Select(recordsToInsert[0].AircraftTailNum, recordsToInsert.Count);
            Assert.IsTrue(selectResult.Success);
            Assert.AreEqual(recordsToInsert.Count, selectResult.Records.Count);

            recordsToInsert.Reverse();

            for(int i = 0; i < selectResult.Records.Count; i++)
            {
                Assert.IsTrue(EquateRecords(selectResult.Records[i], recordsToInsert[i]));
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void Select_RetrievesCorrectTailNum()
        {
            var recordToInsert = TestData.Last();

            var insertResult = db.Insert(recordToInsert);
            Assert.IsTrue(insertResult.Success);

            var selectResult = db.Select(recordToInsert.AircraftTailNum);
            Assert.IsTrue(selectResult.Success);
            Assert.IsTrue(selectResult.Records.Count > 1);
            Assert.AreEqual(recordToInsert.AircraftTailNum, selectResult.Records[0].AircraftTailNum);
        }

        public static bool EquateRecords(TelemetryRecordDAL recordA, TelemetryRecordDAL recordB)
        {
            return
                recordA.AircraftTailNum == recordB.AircraftTailNum &&
                recordA.Timestamp.CompareTo(recordB.Timestamp) == 0 &&
                Math.Round(recordA.Accel_X, 6) == Math.Round(recordB.Accel_X, 6) &&
                Math.Round(recordA.Accel_Y, 6) == Math.Round(recordB.Accel_Y, 6) &&
                Math.Round(recordA.Accel_Z, 6) == Math.Round(recordB.Accel_Z, 6) &&
                Math.Round(recordA.Weight, 6) == Math.Round(recordB.Weight, 6) &&
                Math.Round(recordA.Altitude, 6) == Math.Round(recordB.Altitude, 6) &&
                Math.Round(recordA.Pitch, 6) == Math.Round(recordB.Pitch, 6) &&
                Math.Round(recordA.Bank, 6) == Math.Round(recordB.Bank, 6);
        }
    }
}
