
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using RandomUserGenerator.Models;
using RandomUserGenerator.Utils;

namespace RandomUserGenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomUserController : ControllerBase
    {
        private readonly IUserWorker _userWorker;
        private readonly IRandomIDGenerator _randomIDGenerator;

        public RandomUserController(IUserWorker userWorker, IRandomIDGenerator randomIDGenerator)
        {
            _userWorker = userWorker;
            _randomIDGenerator = randomIDGenerator;
        }

        //GET api/randomuser/getuser
        [Route("GetUser")]
        [HttpGet]
        public async Task<ActionResult<UserModel>> GetUser(int? id = null)
        {
            var randomUser = await _userWorker.GetUser(id ?? _randomIDGenerator.GetRandomID());
            return randomUser;
        }

        //GET api/randomuser/getusers
        [Route("GetUsers")]
        [HttpGet]
        public async Task<ActionResult<UserModel[]>> GetUsers(int numberOfUsers)
        {
            var randomIds = _randomIDGenerator.GetRandomIDs(numberOfUsers);
            var users = await _userWorker.GetUsers(randomIds);
            return users.ToArray();
        }

        //POST api/randomuser/createorupdateuser
        [Route("CreateOrUpdateUser")]
        [HttpPost]
        public async Task<ActionResult<UserModel>> CreateOrUpdateUser([FromBody]UserModel user)
        {
            var updatedUser = await _userWorker.UpdateUser(user);
            return updatedUser;
        }

        //POST api/randomuser/createorupdateuser
        [Route("DeleteUser")]
        [HttpDelete]
        public async Task<string> DeleteUser(int id)
        {
            var deletedUserResult = await _userWorker.DeleteUser(id);
            return $"User with ID: {id} {(deletedUserResult ? "deleted successfully" : "not deleted")}.";
        }
    }
}
