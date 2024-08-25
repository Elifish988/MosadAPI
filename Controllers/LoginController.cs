//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using MosadApi.DAL;
//using MosadApi.Meneger;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace MosadApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class LoginController : ControllerBase
//    {
//        private readonly MosadDBContext _context;
        
//        public LoginController(MosadDBContext mosadDBContext, MissionsMeneger missionsMeneger)
//        {
//            _context = mosadDBContext;
//        }

//        private string GenerateToken(string userIP)
//        {
//            // token handler can create token
//            var tokenHandler = new JwtSecurityTokenHandler();

//            string secretKey = "1234dyi5fjthgjdndfadsfgdsjfgj464twiyyd5ntyhgkdrue74hsf5ytsusefh55678"; //TODO: remove this from code
//            byte[] key = Encoding.ASCII.GetBytes(secretKey);

//            // token descriptor describe HOW to create the token
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                // things to include in the token
//                Subject = new ClaimsIdentity(
//                    new Claim[]
//                    {
//                    new Claim(ClaimTypes.Name, userIP),
//                    }
//                ),
//                // expiration time of the token
//                Expires = DateTime.UtcNow.AddSeconds(60),
//                // the secret key of the token
//                SigningCredentials = new SigningCredentials(
//                    new SymmetricSecurityKey(key),
//                    SecurityAlgorithms.HmacSha256Signature
//                    )
//            };

//            // creating the token
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            // converting the token to string
//            var tokenString = tokenHandler.WriteToken(token);
//            return tokenString;
//        }

//        [HttpPost("login")]
//        public IActionResult Login(LoginObject loginObject)
//        {
//            if (loginObject.UserName == "admin" &&
//                loginObject.Password == "123456")
//            {

//                // getting the user (requester) IP
//                string userIP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
//                var token = GenerateToken(userIP);
//                return Ok(token);
//            }
//            return StatusCode(StatusCodes.Status401Unauthorized,
//                    new { error = "invalid credentials" });
//        }
//    }
//}
