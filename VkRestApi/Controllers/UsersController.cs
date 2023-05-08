using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VkRestApi.Data;

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
    }
}
