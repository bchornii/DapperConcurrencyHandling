using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OptimisticConcurencyControl.Infrastructure.Attributes;
using OptimisticConcurencyControl.Infrastructure.Filters;
using OptimisticConcurencyControl.Models;
using OptimisticConcurencyControl.Repositories;

namespace OptimisticConcurencyControl.Controllers
{    
    [Route("api/Student")]        
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentRepository.GetStudents();
            return Ok(students);
        }

        [HttpGet("{id}")]
        [ETagSupply]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _studentRepository.GetStudent(id);
            return Ok(student);
        }

        [HttpPost]
        [ETagCheck]
        public async Task<IActionResult> EditStudent([FromBody] [ETagModel] Student student)
        {
            var success = await _studentRepository.Edit(student);
            return success ? NoContent() : StatusCode((int)HttpStatusCode.Conflict);
        }

        [HttpPost("test")]
        public IActionResult Update()
        {
            return NoContent();
        }        
    }
}