namespace Lakeshore.Tests
{
    public class Tests
    {
        private IHptHelper _hptHelper;
        private LakeshoreStagingContext _dbContext;
        private HptGroupedEventHandler _hptGroupedEventHandler;

        [SetUp]
        public void Setup()
        {
            // mock the dependencies of HptHelper
            var mockDcToSapPlant = new Mock<IRepositoryKeyStore<DcToSapPlant>>();
            var mockLoggerHptHelper = new Mock<ILogger<HptHelper>>();

            _hptHelper = new HptHelper(mockDcToSapPlant.Object, mockLoggerHptHelper.Object);

            // mock the LakeshoreStagingContext
            //var options = new DbContextOptionsBuilder<LakeshoreStagingContext>()
            //    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            //    .Options;

            //_dbContext = new LakeshoreStagingContext(options);
            //_dbContext.Database.EnsureCreated();

            // add fake data to the in-memory database
            //_dbContext.HptConfirmationOfLabors.AddRange(FakeDatabaseData.GetFakeData());
            //_dbContext.SaveChangesAsync();

            // mock the HptGroupedEventHandler
            var mockLoggerHptEvenHandler = new Mock<ILogger<HptGroupedEventHandler>>();
            var mockKafkaProducerClient = new Mock<IKafkaProducerClient>();
            _hptGroupedEventHandler = new HptGroupedEventHandler(mockLoggerHptEvenHandler.Object, mockKafkaProducerClient.Object, _hptHelper);
        }

        [Test]
        public void TestGetTotalTimeWorked()
        {
            // create a List<HptConfirmationOfLabor> with fake data
            var fakeData = FakeDatabaseData.GetFakeData().ToList();

            string expectedTotalTime = "540.50";

            // call GetTotalTimeWorked and assert that the result is correct
            var result = _hptHelper.GetTotalTimeWorked(fakeData);

            Assert.That(result, Is.EqualTo(expectedTotalTime));
        }

        [Test]
        public void TestGetDateFromWMS()
        {
            var fakeData = 230407;

            var expectedResult = DateTime.Parse("2023-04-07").ToString("yyyy-MM-dd");

            var result = _hptHelper.GetDateFromWMS(fakeData);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void TestProcessGroupedResults()
        {
            var fakehptGroups = new List<HptGroup>()
            {
                new HptGroup()
                {
                    JobNumber = "1000110",
                    OrderSequence = "0",
                    OrderOperation = "0010",
                    Pptdate = 230331,
                    HptConfirmationOfLabors = FakeDatabaseData.GetFakeData().ToList()
                }
            };

            var expectedCount = 2;
            var expectedDomainEventCount = 1;

            var result = Hpt.ProcessGroupedResults(fakehptGroups);

            Assert.That(result.Count, Is.EqualTo(expectedCount));
            Assert.That(result.First().DomainEvents.Count, Is.EqualTo(expectedDomainEventCount));
        }

        [Test]
        public void TestCreateHptDTO()
        {
            var fakehptGroups = new List<HptGroup>()
            {
                new HptGroup()
                {
                    JobNumber = "1000110",
                    OrderSequence = "0",
                    OrderOperation = "0010",
                    Pptdate = 230331,
                    HptConfirmationOfLabors = FakeDatabaseData.GetFakeData().ToList()
                }
            };

            var expectedHptObject = new HptDto()
            {
                ProdnOrdConf2 = new List<ProdnOrdConf2> {
                        new ProdnOrdConf2()
                        {
                            ProdnOrdConf2Type = new ProdnOrdConf2Type()
                            {
                                JobNumber = "1000110",
                                OperationNo = "0010",
                                PPTDate = "2023-03-31",
                                PPTTime = "540.50",
                                PPTUoM = "MIN",
                                Sequence = "0"
                            }
                        }
                    }

            };

            var result = _hptGroupedEventHandler.CreateHptDTO(fakehptGroups);

            expectedHptObject.Should().BeEquivalentTo(result, "Result HptDTO is not equal to expected HptDTO");
        }

        [TearDown]
        public void TearDown()
        {
            //_dbContext.Database.EnsureDeleted();
            //_dbContext.Dispose();
        }
    }
}