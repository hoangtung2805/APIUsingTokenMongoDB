using APIUsingTokenMongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APIUsingTokenMongoDB.Controllers
{
    public class AuthController : Controller
    {
        private IMongoCollection<Student> _students;
        
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            var client = new MongoClient("mongodb+srv://sa:sa@cluster0.tcql2oc.mongodb.net/");
            var database = client.GetDatabase("student");
            _students = database.GetCollection<Student>("student");
        }
        
        [HttpPost("Login")]
        public async Task<ActionResult<object>> Login([FromBody] Login model)
        {
            string token;
            var filter = Builders<Student>.Filter.Eq(x => x.username, model.username) & Builders<Student>.Filter.Eq(x => x.password, model.password);
            var loginStudent = await _students.Find(filter).FirstOrDefaultAsync();
            if (model.username == null || model.password == null)
            {
                return BadRequest(new { error = "Please enter username and password" });
            }
            if (loginStudent == null)
            {
                return BadRequest(new { error = "Invalid username or password" });
            }
            if (loginStudent.password != model.password)
            {
                return BadRequest(new { error = "Invalid username or password" });
            }
            token = CreateToken(loginStudent);

            return new { token };
        }
        [HttpPost("Register")]
        public async Task<ActionResult<Register>> Register(Register register,Student student)
        {
            // Check if the username already exists
            var usernameExists = await _students.Find(x => x.username == register.username).AnyAsync();
            if (usernameExists)
            {
                return BadRequest(new { error = "Username already exists" });
            }
            student = new Student();
            student.name = register.name;
            student.username = register.username;
            student.password = register.password;
            student.gender = register.gender;
            student.dob= register.dob;


            // Insert the student into the collection
            await _students.InsertOneAsync(student);

            return register;
        }




        private string CreateToken(Student loginStudent)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginStudent.username)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var _token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(_token);
            return jwt;
        }
    }
}

