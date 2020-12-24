using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class Session
    {
        public string ID { get; set; }
        public UserRole Role { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Expires { get; set; }
    }
}
