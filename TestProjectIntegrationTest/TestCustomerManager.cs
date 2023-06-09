using BusinessLayer;
using DataLayer;
using System.Data.SqlClient;
using Xunit.Priority;

namespace TestProjectIntegrationTest
{
    public class DatabaseFixture : IDisposable
    {
        private string connString = @"Data Source=NB21-6CDPYD3\SQLEXPRESS;Initial Catalog=BikeRepairShopTestDB;Integrated Security=True";
        public CustomerRepositoryHelper dbHelper;
        private void CleanDB()
        {
            string clearDB = "DELETE FROM Bike;DBCC CHECKIDENT ('Bike', RESEED, 0); DELETE FROM Customer; DBCC CHECKIDENT ('Customer', RESEED, 0)";
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand command = conn.CreateCommand())
            {
                conn.Open();
                command.CommandText = clearDB;
                command.ExecuteNonQuery();
            }
        }
        public DatabaseFixture()
        {            
            CleanDB();
            customerRepository = new CustomerRepositoryADO(connString);
            dbHelper = new CustomerRepositoryHelper(connString);
            // ... initialize data in the test database ...
        }
        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
        public ICustomerRepository customerRepository { get; private set; }
    }
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class TestCustomerManager : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture fixture;
        CustomerManager customerManager;

        public TestCustomerManager(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            customerManager = new CustomerManager(fixture.customerRepository);
        }
        [Fact, Priority(1)]
        public void Test_AddBike_Valid()
        {
            //Test();
            string description = "green bike";
            BikeType bikeType = BikeType.racingBike;
            int customerId = 1;
            string customerDescription = "jos (jos@gmail)";
            double purchaseCost = 175;
            BikeInfo bikeInfo = new BikeInfo(null, description, bikeType, customerId, customerDescription, purchaseCost);

            customerManager.AddBike(bikeInfo);
            var bDB = customerManager.GetBikesInfo();

            Assert.NotNull(bDB);
            Assert.Contains(bikeInfo, bDB);
        }
        [Fact, Priority(0)]
        public void Test_AddCustomer_Valid()
        {
            string name = "jos";
            string email = "jos@gmail";
            string address = "9000 Gent";
            CustomerInfo customerInfo = new CustomerInfo(null,name,email,address,0,0);
            customerManager.AddCustomer(customerInfo);
            Customer cDB=customerManager.GetCustomer(1);
            Assert.NotNull(cDB);
            Assert.Equal(email, cDB.Email);
            Assert.Equal(name, cDB.Name);
            Assert.Equal(address, cDB.Address);
        }
        [Fact, Priority(2)]
        public void Test_UpdateBike()
        {
            string description = "blue bike";
            BikeType bikeType = BikeType.childBike;
            int customerId = 1;
            string customerDescription = "jos (jos@gmail)";
            double purchaseCost = 75;
            BikeInfo bikeInfo = new BikeInfo(1, description, bikeType, customerId, customerDescription, purchaseCost); //id gekend eerste fiets

            customerManager.UpdateBike(bikeInfo);
            var bDB = customerManager.GetBikesInfo();

            Assert.NotNull(bDB);
            Assert.Contains(bikeInfo, bDB);
        }
        [Fact, Priority(3)]
        public void Test_DeleteBike()
        {
            string description = "blue bike";
            BikeType bikeType = BikeType.childBike;
            int customerId = 1;
            string customerDescription = "jos (jos@gmail)";
            double purchaseCost = 75;
            BikeInfo bikeInfo = new BikeInfo(1, description, bikeType, customerId, customerDescription, purchaseCost); //id gekend eerste fiets
            customerManager.DeleteBike(bikeInfo);
            var customerDB = customerManager.GetCustomer(customerId);
            Assert.DoesNotContain(DomainFactory.ExistingBike(1,bikeType,purchaseCost,description), customerDB.Bikes());
            Assert.DoesNotContain(bikeInfo, customerManager.GetBikesInfo());
            Assert.Equal(0, fixture.dbHelper.StatusBike((int)bikeInfo.Id));
        }
    }
}