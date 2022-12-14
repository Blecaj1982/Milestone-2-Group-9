using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using FDMS.DAL;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Tests
{
    /// <summary>
    /// Contains tests for the FdmsDatbase class
    /// </summary>
    [TestClass]
    public class FdmsDatabaseTests
    {
        public static FdmsDatabase db = new FdmsDatabase();

        public static List<TelemetryRecordDal> TestData = new List<TelemetryRecordDal>();


        /// <summary>
        /// Setup code to run before all tests. Connects the FdmsDatabase under
        /// test to a local database then reads in and stores testing data
        /// </summary>
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

            foreach (string[] l in parsedLines)
            {
                TestData.Add(
                    new TelemetryRecordDal(
                    )
                    {
                        AircraftTailNum = l[0],
                        Timestamp = DateTime.Parse(l[1]),
                        Accel_X = float.Parse(l[2]),
                        Accel_Y = float.Parse(l[3]),
                        Accel_Z = float.Parse(l[4]),
                        Weight = float.Parse(l[5]),
                        Altitude = float.Parse(l[6]),
                        Pitch = float.Parse(l[7]),
                        Bank = float.Parse(l[8])
                    }
                );
            }
        }

        /// <summary>
        /// Runs after all tests. Closes the FdmsDatabase connection
        /// </summary>
        [AssemblyCleanup]
        public static void Cleanup()
        {
            db.Disconnect();
        }

        [TestMethod]
        [DoNotParallelize]
        public void IDB0301_Insert_1_Record_In_1_Second()
        {
            // setup
            FdmsDatabase d = new FdmsDatabase();
            var connectResult = d.Connect("Server=(localdb)\\MSSQLLocalDB;User ID=FDMS_User;Password=FDMS_Password;Database=FDMS_Server");
            Assert.IsTrue(connectResult.Success);
            TelemetryRecordDal record = new TelemetryRecordDal(
                "G-SQXT",
                DateTime.Parse("1-20-1992 6:06:50"),
                new GForceParameters(1.1f, 2.2f, 3.3f, 70f),
                new AltitudeParameters(9.9f, 10f, 11.1f)
            );

            // test
            Stopwatch timer = Stopwatch.StartNew();
            var insertResult = d.Insert(record);
            timer.Stop();

            var selectResult = d.Select(record.AircraftTailNum);
            d.Disconnect();

            // Confirm results
            Assert.IsTrue(insertResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <  1);
            Assert.IsTrue(selectResult.Success);
            Assert.AreEqual(1, selectResult.Records.Count);
            Assert.IsTrue(EquateRecords(record, selectResult.Records[0]));
        }

        [TestMethod]
        [DoNotParallelize]
        public void DB0101_InsertReturnsErrorAfterDisconnection()
        {
            // setup
            FdmsDatabase d = new FdmsDatabase();
            var connectResult = d.Connect("Server=(localdb)\\MSSQLLocalDB;User ID=FDMS_User;Password=FDMS_Password;Database=FDMS_Server");
            Assert.IsTrue(connectResult.Success);

            TelemetryRecordDal record = new TelemetryRecordDal(
                "G-TTXT",
                DateTime.Parse("1-20-1992 6:06:50"),
                new GForceParameters(1.1f, 2.2f, 3.3f, 70f),
                new AltitudeParameters(9.9f, 10f, 11.1f)
            );

            d.Disconnect();
            var insertResult = d.Insert(record);

            // Confirm results
            Assert.IsFalse(insertResult.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(insertResult.FailureMessage));
        }

        [TestMethod]
        [DoNotParallelize]
        public void DB0301_InsertReturnsErrorForRecordWithInvalidTailNum()
        {
            FdmsDatabase d = new FdmsDatabase();
            var connectResult = d.Connect("Server=(localdb)\\MSSQLLocalDB;User ID=FDMS_User;Password=FDMS_Password;Database=FDMS_Server");
            Assert.IsTrue(connectResult.Success);

            TelemetryRecordDal record = new TelemetryRecordDal(
                "BAD_TAIL_NUMBER_TOO_LONG",
                DateTime.Parse("1-20-1992 6:06:50"),
                new GForceParameters(1.1f, 2.2f, 3.3f, 70f),
                new AltitudeParameters(9.9f, 10f, 11.1f)
            );

            var insertResult = d.Insert(record);
            d.Disconnect();

            // Confirm results
            Assert.IsFalse(insertResult.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(insertResult.FailureMessage));
        }

        [TestMethod]
        [DoNotParallelize]
        public void DBFN030B1_InsertReturnsErrorWhenNotConnected()
        {
            FdmsDatabase d = new FdmsDatabase();
            var insertResult = d.Insert(TestData[0]);
            Assert.IsFalse(insertResult.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(insertResult.FailureMessage));
        }

        [TestMethod]
        [DoNotParallelize]
        public void DBFN030B2_Insert_1_Record_In_1_Second()
        {
            // setup
            FdmsDatabase d = new FdmsDatabase();
            var connectResult = d.Connect("Server=(localdb)\\MSSQLLocalDB;User ID=FDMS_User;Password=FDMS_Password;Database=FDMS_Server");
            Assert.IsTrue(connectResult.Success);
            TelemetryRecordDal record = new TelemetryRecordDal(
                "G-TTXT",
                DateTime.Parse("1-20-1992 6:06:50"),
                new GForceParameters(1.1f, 2.2f, 3.3f, 70f),
                new AltitudeParameters(9.9f, 10f, 11.1f)
            );

            // test
            Stopwatch timer = Stopwatch.StartNew();
            var insertResult = d.Insert(record);
            timer.Stop();

            var selectResult = d.Select(record.AircraftTailNum);
            d.Disconnect();

            // Confirm results
            Assert.IsTrue(insertResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <  1);
            Assert.IsTrue(selectResult.Success);
            Assert.AreEqual(1, selectResult.Records.Count);
            Assert.IsTrue(EquateRecords(record, selectResult.Records[0]));
        }

        [TestMethod]
        [DoNotParallelize]
        public void DBFN0501_Search_1000_Records_In_1_Second()
        {
            // create db instance
            FdmsDatabase d = new FdmsDatabase();

            // connect
            var connectResult = d.Connect("Server=(localdb)\\MSSQLLocalDB;User ID=FDMS_User;Password=FDMS_Password;Database=FDMS_Server");
            Assert.IsTrue(connectResult.Success);

            // insert 1000 etnries
            int entries = 0;
            while(entries< 1000) {
            for (int i = 0; i < TestData.Count; i++) {
                    d.Insert(TestData[i]);
                    entries++;
                }
            }

            // start timer
            Stopwatch timer = Stopwatch.StartNew();
            // search for 1000 entries
            var searchResult = d.Select(TestData.First().AircraftTailNum);
            timer.Stop();

            d.Disconnect();

            // Confirm results
            Assert.IsTrue(searchResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <  1);
        }

        /// <summary>
        /// Tests that FdmsDatabase can insert multiple records
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        public void Insert_CanInsertMultipleRecords()
        {
            foreach (var record in TestData)
            {
                var r = db.Insert(record);
                Assert.IsTrue(r.Success);
            }
        }

        /// <summary>
        /// Tests that after inserting a record into the database, the
        /// FdmsDatabase class can find it
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        public void Insert_InsertedRecordsEndsUpInDatabase()
        {
            TelemetryRecordDal recordToInsert = new TelemetryRecordDal()
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

            var selectResult = db.Select("ABCDEF");
            Assert.IsTrue(selectResult.Success);
            Assert.IsTrue(selectResult.Records.Count == 1);
            Assert.IsTrue(EquateRecords(recordToInsert, selectResult.Records[0]));
        }

        /// <summary>
        /// Tests that Select returned records with the correct tail number
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        public void Select_RetrievesCorrectTailNum()
        {
            var recordToInsert = TestData.Last();

            var insertResult = db.Insert(recordToInsert);
            Assert.IsTrue(insertResult.Success);

            var selectResult = db.Select(recordToInsert.AircraftTailNum);
            Assert.IsTrue(selectResult.Success);
            Assert.IsTrue(selectResult.Records.Count >= 1);
            Assert.AreEqual(recordToInsert.AircraftTailNum, selectResult.Records[0].AircraftTailNum);
        }

        /// <summary>
        /// Used for comparing the fields of two records except for their EntryTimestamp fields
        /// </summary>
        /// <returns>
        /// True if every field of two TelemetryRecordDALs are equal while
        /// ignoring their EntryTimestamp fields
        /// </returns>
        public static bool EquateRecords(TelemetryRecordDal recordA, TelemetryRecordDal recordB)
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
