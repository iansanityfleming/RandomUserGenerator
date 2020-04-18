using Amazon.DynamoDBv2;
using DataAccess;
using Moq;
using NUnit.Framework;
using RandomUserGenerator.Controllers;
using RandomUserGenerator.DataAccess.DAO;
using RandomUserGenerator.Models;
using RandomUserGenerator.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomUserGeneratorTests
{
    public class Tests
    {        
        private const int testUserID = 2;
        private const string testUserName = "testUser";
        private const string testUserLastNameSuffix = "Last";
        private const int testUsers = 10;

        private RandomUserController _randomUserController;
        private IUserWorker _mockUserWorker;
        private IRandomIDGenerator _mockRandomIDGenerator;

        [SetUp]
        public void Setup()
        {
            _mockUserWorker = new MockUserWorker();
            _mockRandomIDGenerator = new MockRandomIDGenerator();
            _randomUserController = new RandomUserController(_mockUserWorker, _mockRandomIDGenerator);
        }

        #region Tests

        [Test]
        public void GetSingleRandomUser()
        {
            var id = _mockRandomIDGenerator.GetRandomID();
            var user = _randomUserController.GetUser(id).Result.Value;
            Assert.AreEqual($"{testUserName}{id}", user.FirstName);
        }

        [Test]
        public void GetMultipleUsers()
        {
            var numberToFetch = testUsers;
            var users = _randomUserController.GetUsers(numberToFetch).Result.Value;
            Assert.IsTrue(users.Count() == numberToFetch);
        }

        [Test]
        public void UpdateUserDetails()
        {
            var id = _mockRandomIDGenerator.GetRandomID();
            var user = _randomUserController.GetUser(id).Result.Value;
            var newName = "newName";
            user.FirstName = newName;
            var updatedUser = _randomUserController.CreateOrUpdateUser(user).Result.Value;
            Assert.AreEqual(user.FirstName, updatedUser.FirstName);
        }
        #endregion Tests


        #region Mocking
        class MockRandomIDGenerator : IRandomIDGenerator
        {
            public int GetRandomID()
            {
                return testUserID;
            }

            public IEnumerable<int> GetRandomIDs(int numberRequired)
            {
                return Enumerable.Range(0, testUsers).ToArray();
            }
        }

        class MockUserWorker : IUserWorker
        {
            private readonly Dictionary<int, User> _fakeUsers;

            public MockUserWorker()
            {
                _fakeUsers = Enumerable.Range(0, testUsers)
                    .ToDictionary(x => x, x => new User {
                        id = x,
                        firstName = $"{testUserName}{x}",
                        lastName = $"{testUserName}{testUserLastNameSuffix}{x}",
                    });
            }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public async Task<UserModel> GetUser(int id)
            {
                return _fakeUsers[id].ToUserModel();
            }

            public async Task<IEnumerable<UserModel>> GetUsers(IEnumerable<int> ids)
            {
                return _fakeUsers.Where(x => ids.Contains(x.Key)).Select(x => x.Value.ToUserModel());
            }

            public async Task<UserModel> UpdateUser(UserModel updatedUser)
            {
                var currentUser = _fakeUsers[updatedUser.Id];
                var combinedUser = currentUser.OverwriteUser(updatedUser.ToUserDAO());
                return combinedUser.ToUserModel();
            }
            public async Task<bool> DeleteUser(int id)
            {
                //There is no value testing this feature as all it does is call the DynamoDB deleteItem method
                //I would effectively only be testing that the DynamoDB API is functioning.
                return true;
            }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        }
        #endregion Mocking
    }
}