using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RandomUserGenerator.DataAccess.DAO;
using RandomUserGenerator.Models;
using RandomUserGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess.Impl
{
    public class UserWorker : IUserWorker
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public UserWorker(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;            
        }

        #region Interface Methods
        public async Task<UserModel> GetUser(int id)
        {
            await CheckTableExists();
            var userDao = await FetchUserByID(id);
            return userDao.ToUserModel(ImageType.Large);
        }

        public async Task<IEnumerable<UserModel>> GetUsersByName(string name) {

            var condition = new Condition
            {
                ComparisonOperator = ComparisonOperator.CONTAINS,
                AttributeValueList = new List<AttributeValue> { new AttributeValue { S = name } }
            };

            var scanRequest = new ScanRequest
            {
                TableName = Constants.UserTableName,
                ConditionalOperator = ConditionalOperator.OR,
                ScanFilter = new Dictionary<string, Condition>
                {
                    { "firstName", condition },
                    { "lastName", condition }
                }
            };

            var results = await _amazonDynamoDB.ScanAsync(scanRequest);
            var userDaos = results.Items.Select(x => x.MapSimpleResponse<User>());
            return userDaos.Select(x => x.ToUserModel());
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        //it does have await operators, they are just in the linq query.
        public async Task<IEnumerable<UserModel>> GetUsers(IEnumerable<int> ids)
        {
            //Calling multiple GetItemAsync is actually faster than scanning or querying the table.
            return ids.Select(async x => await FetchUserByID(x))
                .Select(t => t.Result)
                .Select(u => u.ToUserModel(ImageType.Thumbnail));
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task<UserModel> UpdateUser(UserModel updatedUserModel)
        {
            var id = updatedUserModel.Id;
            var updatedDAO = updatedUserModel.ToUserDAO();
            var fromDynamo = await UpdateUserByID(id, updatedDAO);
            return fromDynamo.ToUserModel();
        }

        public async Task<bool> DeleteUser(int id)
        {
            var request = new DeleteItemRequest
            {
                TableName = Constants.UserTableName,
                Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { N = id.ToString() } } }
            };
            var response = await _amazonDynamoDB.DeleteItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        #endregion Interface Methods



        #region helper methods
        private async Task<User> FetchUserByID(int id)
        {
            var request = new GetItemRequest
            {
                TableName = Constants.UserTableName,
                Key = new Dictionary<string, AttributeValue>() { { "id", new AttributeValue { N = id.ToString() } } }
            };
            var response = await _amazonDynamoDB.GetItemAsync(request);
            return response.Item.MapSimpleResponse<User>();
        }

        private async Task<User> UpdateUserByID(int id, User updatedUser)
        {
            var currentUser = await FetchUserByID(id);
            var combinedUser = currentUser.OverwriteUser(updatedUser);

            var request = new PutItemRequest
            {
                TableName = Constants.UserTableName,
                Item = combinedUser.ToDyanmoRequest()
            };

            await _amazonDynamoDB.PutItemAsync(request);

            var updatedObjectResponse = await FetchUserByID(id);
            return updatedObjectResponse;
        }
        #endregion helper methods


        #region table init methods
        private async Task<bool> CheckTableExists()
        {
            var tables = await _amazonDynamoDB.ListTablesAsync();
            if (!tables.TableNames.Contains(Constants.UserTableName))
            {
                await SetUpTable();
                await PopulateTable();
            }
            return true;
        }

        private async Task<bool> SetUpTable()
        {
            var createRequest = new CreateTableRequest
            {
                TableName = Constants.UserTableName,
                AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "id",
                            AttributeType = "N"
                        }
                    },
                KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "id",
                            KeyType = "HASH"  //Partition key
                        }
                    },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 3,
                    WriteCapacityUnits = 3
                }
            };
            await _amazonDynamoDB.CreateTableAsync(createRequest);
            return true;
        }

        private async Task<bool> PopulateTable()
        {
            var json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "users.json");
            var deserialised = JsonSerializer.Deserialize<User[]>(json);
            foreach (var user in deserialised)
                await UpdateUserByID(user.id, user);

            return true;
        }
        #endregion table init methods
    }
}
