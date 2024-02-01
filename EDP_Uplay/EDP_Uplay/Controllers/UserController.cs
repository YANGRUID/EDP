
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using EDP_Uplay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EDP_Uplay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        //RUIDONG
        public static string ConnectionString
        {
            get
            {
                #region ConnectionString
                // 创建 ConfigurationBuilder 对象
                var builder = new ConfigurationBuilder()
                    // 指定 appsettings.json 文件路径（如果不在当前目录）
                    .SetBasePath(Directory.GetCurrentDirectory())
                    // 添加 JSON 格式的配置源
                    .AddJsonFile("appsettings.json");
                // 构建 Configuration 对象
                IConfiguration configuration = builder.Build();
                // 使用 GetConnectionString 方法获取数据库连接字符串
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                return connectionString;
                #endregion
            }
        }


        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //RUIDONG GetUSer
        [HttpGet]
        public User Get(int id = 0)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                return connection.QuerySingleOrDefault<User>($"select * from Users where id={id}");
            }
        }
        //RUIDONG GetUserList
        [HttpGet]
        public List<User> GetUsersList(string query = "")
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(query))
            {
                sb.Append($" and name like '%{query}%'");
            }
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // 查询数据
                var result = connection.Query<User>($"select * from Users where 1=1 {sb.ToString()}").ToList();
                return result;
            }
        }
        //RUIDONG EDIT User Points
        [HttpPut]
        public MessageModel<string> Edit(User request)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute($@"update Users set integral={request.integral},state='{request.state}' where id={request.Id}");
                return new MessageModel<string>()
                {
                    msg = "保存成功",
                    success = true,
                    response = "保存成功"
                };
            }
        }



        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            request.Name = request.Name.Trim();
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();
            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser != null)
            {
                string message = "Email already exists.";
                return BadRequest(new { message });
            }

            // Create user object
            var now = DateTime.Now;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                Username = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = now,
                UpdatedAt = now,
                Role = UserRole.User
        };
            // Add user
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }


        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Trim string values
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();
            // Check email and password
            
            var foundUser = _context.Users.Where(
            x => x.Email == request.Email).FirstOrDefault();
            if (foundUser == null)
            {
                string message = "User does not exist";
                return BadRequest(new { message });
            }
            bool verified = BCrypt.Net.BCrypt.Verify(
            request.Password, foundUser.PasswordHash);
            if (!verified)
            {
                string message = "Email or password is not correct.";
                return BadRequest(new { message });
            }
            // Return user info
            var user = new
            {
                foundUser.Id,
                foundUser.Email,
                foundUser.Username,
                foundUser.Role
            };
            string accessToken = CreateToken(foundUser);
            return Ok(new { user, accessToken });
        }
        private string CreateToken(User user)
        {
            string secret = _configuration.GetValue<string>(
            "Authentication:Secret");
            int tokenExpiresDays = _configuration.GetValue<int>(
            "Authentication:TokenExpiresDays");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
                Expires = DateTime.UtcNow.AddDays(tokenExpiresDays),
                SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);
            return token;
        }
        [HttpGet("auth"), Authorize]
        public IActionResult Auth()
        {
            var id = Convert.ToInt32(User.Claims.Where(
             c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => c.Value).SingleOrDefault());
            var username = User.Claims.Where(c => c.Type == ClaimTypes.Name)
            .Select(c => c.Value).SingleOrDefault();
            var email = User.Claims.Where(c => c.Type == ClaimTypes.Email)
            .Select(c => c.Value).SingleOrDefault();
            var roleString = User.Claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value).SingleOrDefault();

            // Convert the role string to an integer
            int roleNumeric = roleString == "Admin" ? 1 : 0;

            if (id != 0 && username != null && email != null)
            {
                var user = new
                {
                    id,
                    email,
                    username,
                    role = roleNumeric // Include the integer role
                };
                return Ok(new { user });
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
