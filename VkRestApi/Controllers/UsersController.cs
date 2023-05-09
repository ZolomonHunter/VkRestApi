using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Diagnostics;
using VkRestApi.Data;
using VkRestApi.Models;

namespace VkRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;

        public UsersController(ApiContext apiContext)
        {
            _context = apiContext;
        }


        // Adding Users to DB
        // Minimum set of fileds: login, password
        // If UserGroup is provided, UserState.Code is required (default -> Active)
        // UserState is optional                                (default -> User)
        // All ids set to 0 for compatabilty with DB (autoincrementing itself)
        [HttpPost]
        public async Task<ActionResult> Post(User user)
        {
            // Marking the time
            Stopwatch stopwatch = Stopwatch.StartNew();

            user.Id = 0;
            user.CreatedDate = DateTime.Now.ToUniversalTime();
            await _context.Users.AddAsync(user);

            // Managing collisions
            if (_context.Users.Include("UserState")
                .Any(u => u.Login == user.Login && u.UserState.Code == UserStateEnum.ACTIVE))
                return BadRequest();

            // Managing UserState
            if (user.UserState == null)
                user.UserState = new UserState();
            else
            {
                user.UserState.Id = 0;
                user.UserState.Code = UserStateEnum.ACTIVE;
                user.UserState.Description ??= "";
            }

            // Managing UserGroup
            if (user.UserGroup == null)
                user.UserGroup = new UserGroup();
            else
            {
                user.UserGroup.Id = 0;
                if (user.UserGroup.Code != UserGroupEnum.USER && user.UserGroup.Code != UserGroupEnum.ADMIN)
                    user.UserGroup.Code = UserGroupEnum.USER;
                user.UserGroup.Description ??= "";
            }

            // Check if there are any Active Admins
            if (user.UserGroup.Code == UserGroupEnum.ADMIN)
            {
                if (_context.Users.Include("UserGroup").Include("UserState")
                    .Any(u => u.UserGroup.Code == UserGroupEnum.ADMIN && u.UserState.Code == UserStateEnum.ACTIVE))
                    return BadRequest(user);
            }

            try
            {
                await Task.Delay(5000 - (int)stopwatch.ElapsedMilliseconds);
            }
            catch (ArgumentOutOfRangeException e) { }

            // Check if User with the same Login is pending or already added to DB
            if (_context.ChangeTracker.Entries<User>().Any(u => u.Entity.Login == user.Login && u.Entity.CreatedDate != user.CreatedDate))
                return BadRequest();
            if (_context.Users.Any(u => u.Login == user.Login))
                return BadRequest();

            await _context.SaveChangesAsync();
            return new JsonResult(user);
        }
        
        // Getting Users from DB
        // Blocked Users do not return 
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                UserGroup? userGroup = await _context.UserGroups.FindAsync(user.UserGroupId);
                UserState? userState = await _context.UserStates.FindAsync(user.UserStateId);
                if (userGroup == null || userState == null)
                    return NotFound();
                user.UserGroup = userGroup;
                user.UserState = userState;
                if (user.UserState.Code == UserStateEnum.BLOCKED)
                    return NoContent();
                return new JsonResult(user);
            }
            else return NoContent();
        }

        // Getting Users from DB
        // Blocked Users do not return 
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            return new JsonResult(await _context.Users.Include("UserGroup").Include("UserState")
                .Where(user => user.UserState.Code == UserStateEnum.ACTIVE)
                .ToListAsync());
        }


        // Soft deleting Users
        // Deleted User's State becomes Blocked
        // Can't delete Blocked Users
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                UserState? userState = await _context.UserStates.FindAsync(user.UserStateId);
                if (userState == null)
                    return NotFound();
                if (userState.Code == UserStateEnum.BLOCKED)
                    return NoContent();
                userState.Code = UserStateEnum.BLOCKED;
                await _context.SaveChangesAsync(true);
                return new JsonResult(user);
            }
            else return NoContent();
        }
    }
}
