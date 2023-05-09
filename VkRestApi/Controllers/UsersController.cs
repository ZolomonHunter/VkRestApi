using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VkRestApi.Data;
using VkRestApi.Models;

namespace VkRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMemoryCache _memoryCache;

        public UsersController(ApiContext apiContext, IMemoryCache memoryCache)
        {
            _context = apiContext;
            _memoryCache = memoryCache;
        }


        // Adding Users to DB
        // Minimum set of fileds: login, password
        // If UserGroup is provided, UserState.Code is required (default -> Active)
        // UserState is optional                                (default -> User)
        // All ids set to 0 for compatabilty with DB (autoincrementing itself)
        [HttpPost]
        public async Task<ActionResult> Post(User user)
        {
            // Caching pending User's Logins
            // If User with the same Login already send POST, request will be denied
            if (_memoryCache.TryGetValue(user.Login, out byte temp))
                return BadRequest();
            else
                _memoryCache.Set<byte>(user.Login, 0, TimeSpan.FromMinutes(1));
            
            // Marking the time
            Stopwatch stopwatch = Stopwatch.StartNew();

            user.Id = 0;
            user.CreatedDate = DateTime.Now.ToUniversalTime();
            await _context.Users.AddAsync(user);

            // Managing collisions
            if (await _context.Users.Include("UserState")
                .AnyAsync(u => u.Login == user.Login && u.UserState.Code == UserStateEnum.ACTIVE))
                return removeFromCacheAndReturn(user, BadRequest());

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
                if (await _context.Users.Include("UserGroup").Include("UserState")
                    .AnyAsync(u => u.UserGroup.Code == UserGroupEnum.ADMIN && u.UserState.Code == UserStateEnum.ACTIVE))
                    return removeFromCacheAndReturn(user, BadRequest());
            }

            // wait remaining time
            try
            {
                await Task.Delay(5000 - (int)stopwatch.ElapsedMilliseconds);
            }
            catch (ArgumentOutOfRangeException e) { }

            
            await _context.SaveChangesAsync();
            return removeFromCacheAndReturn(user, new JsonResult(user));
        }

        // Post method must remove cached Login before responding
        private ActionResult removeFromCacheAndReturn(User user, ActionResult result)
        {
            _memoryCache.Remove(user.Login);
            return result;
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

        // Getting all Users from DB
        // Blocked Users do not return 
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            return new JsonResult(await _context.Users.Include("UserGroup").Include("UserState")
                .Where(user => user.UserState.Code == UserStateEnum.ACTIVE)
                .ToListAsync());
        }

        // Getting Users from DB with pages of 25
        // Blocked Users do not return
        // Pages indexed from 1
        [HttpGet("GetAllByPagination/{page}")]
        public async Task<ActionResult> GetAllByPagination(int page)
        {
            const int PAGE_OFFSET = 25;

            // Check for wrong arg
            if (page < 1)
                return BadRequest();

            return new JsonResult(await _context.Users.Include("UserGroup").Include("UserState")
                .Where(user => user.UserState.Code == UserStateEnum.ACTIVE)
                .Skip(PAGE_OFFSET * (page - 1))
                .Take(PAGE_OFFSET)
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
