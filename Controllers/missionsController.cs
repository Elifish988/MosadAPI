using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosadApi.DAL;

namespace MosadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class missionsController : ControllerBase
    {
        private readonly MosadDBContext _context;
        public missionsController(MosadDBContext mosadDBContext)
        {
            _context = mosadDBContext;
        }
    }
}
