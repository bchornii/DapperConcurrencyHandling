using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using OptimisticConcurencyControl.Models;

namespace OptimisticConcurencyControl.Repositories
{
    public class StudentRepository : IStudentRepository
    {        
        private readonly string _connectionString;

        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IReadOnlyCollection<Student>> GetStudents()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sQuery = @"SELECT Id, 
                                      FirstMidName,
                                      LastName
                               FROM Student";

                return (await connection.QueryAsync<Student>(sQuery)).ToArray();
            }
        }

        public async Task<Student> GetStudent(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sQuery = @"SELECT Id, 
                                      FirstMidName,
                                      LastName,
                                      RowVersion as Version
                               FROM Student 
                               WHERE Id = @Id";
                return await connection
                    .QueryFirstOrDefaultAsync<Student>(sQuery, new {Id = id});
            }            
        }

        public async Task<bool> Edit(Student student)
        {
            using (var connection = new SqlConnection(_connectionString))
            {                
                var sQuery = @"UPDATE Student 
                             SET LastName = @LastName, 
                                 FirstMidName = @FirstMidName                                 
                             WHERE Id = @Id AND RowVersion = @Version";
                var count = await connection.ExecuteAsync(sQuery, new
                {
                   student.Id,
                   student.Version,
                   student.FirstMidName,
                   student.LastName                   
                });
                return count > 0;
            }
        }
    }
}
