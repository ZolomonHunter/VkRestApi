using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<JsonResult> Post(User user)
        {
            await _context.Users.AddAsync(user);
            _context.SaveChanges();
            return new JsonResult(Ok(user));
        }
    }
}
