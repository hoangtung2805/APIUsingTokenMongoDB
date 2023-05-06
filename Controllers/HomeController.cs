﻿using APIUsingTokenMongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Security.Claims;
using ZstdSharp.Unsafe;

namespace APIUsingTokenMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
   
    public class HomeController  : Controller
    {
        private IMongoCollection<Student> _students;
        public HomeController()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("student");
            _students = database.GetCollection<Student>("student");
        }
        [HttpGet("GetAllStudent")]
        [Authorize]
        public async Task<List<Student>> GetAllStudent()
        {
            var students = _students.Find(student => true).ToList();
            return students;
        }
        [HttpGet("GetStudentByID")]
        [Authorize]
        public async Task<Student> GetStudentByID(int id)
        {
            var filter = Builders<Student>.Filter.Eq("Id", id);
            var student = await _students.Find(filter).FirstOrDefaultAsync();
            return student;
        }
        [HttpPost("CreateStudent")]
        [Authorize]
        public async Task<Student> CreateStudent(Student student)
        {
            await _students.InsertOneAsync(student);
            return student;
        }

        [HttpPost("UpdateStudentById")]
        [Authorize]
        public async Task<Student> UpdateStudent(Student student, int id)
        {
            var filter = Builders<Student>.Filter.Eq("Id", id);
            var update = Builders<Student>.Update
                .Set(s => s.name, student.name)
                .Set(s => s.gender, student.gender)
                .Set(s => s.dob, student.dob)
                .Set(s => s.username, student.username)
                .Set(s => s.password, student.password);

            var options = new FindOneAndUpdateOptions<Student>
            {
                ReturnDocument = ReturnDocument.After
            };

            var updatedStudent = await _students.FindOneAndUpdateAsync(filter, update, options);
            if (updatedStudent == null)
            {
                throw new Exception("Student not found");
            }

            return updatedStudent;
        }

        [HttpDelete("DeleteStudent")]
        [Authorize]
        public async Task DeleteStudent(int id)
        {
            var filter = Builders<Student>.Filter.Eq("Id", id);
            var result = await _students.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new Exception("Student not found");
            }
        }

    }
}
