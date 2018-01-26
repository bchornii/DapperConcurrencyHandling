using System.Collections.Generic;
using System.Threading.Tasks;
using OptimisticConcurencyControl.Models;

namespace OptimisticConcurencyControl.Repositories
{
    public interface IStudentRepository
    {
        Task<IReadOnlyCollection<Student>> GetStudents();
        Task<Student> GetStudent(int id);
        Task<bool> Edit(Student student);
    }
}