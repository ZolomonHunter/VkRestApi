using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public async Task<ActionResult> Post(User user)
        {
            // TODO 5sec, collission
            user.Id = 0;
            user.CreatedDate = DateTime.Now.ToUniversalTime();

            if (user.UserState == null)
                user.UserState = new UserState();
            else
            {
                user.UserState.Id = 0;
                user.UserState.Code = UserState.StateCodes.Active;
                user.UserState.Description ??= "";
            }


            if (user.UserGroup == null)
                user.UserGroup = new UserGroup();
            else
            {
                user.UserGroup.Id = 0;
                user.UserGroup.Description ??= "";
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new JsonResult(user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.UserGroup = await _context.UserGroups.FindAsync(user.UserGroupId);
                user.UserState = await _context.UserStates.FindAsync(user.UserStateId);
                if (user.UserGroup == null || user.UserState == null)
                    return NotFound();
                return new JsonResult(user);
            }
            else return NoContent();
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            return new JsonResult(await _context.Users.Include("UserGroup").Include("UserState").ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                UserState? userState = await _context.UserStates.FindAsync(user.UserStateId);
                if (userState == null)
                    return NotFound();
                userState.Code = UserState.StateCodes.Blocked;
                await _context.SaveChangesAsync(true);
                return new JsonResult(user);
            }
            else return NoContent();
        }
    }
}
