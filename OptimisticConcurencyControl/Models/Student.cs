using System;
using Newtonsoft.Json;
using OptimisticConcurencyControl.Infrastructure.Attributes;

namespace OptimisticConcurencyControl.Models
{
    public class Student
    {        
        public int Id { get; set; }        
        public string LastName { get; set; }        
        public string FirstMidName { get; set; }        
        public DateTime EnrollmentDate { get; set; }    
        
        [JsonIgnore]
        [RowVersion]
        public byte[] Version { get; set; }
    }
}
