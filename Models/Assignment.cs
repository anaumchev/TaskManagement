using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace TaskManagement.Models
{
    public class Assignment
    {
        public int ID { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed.")]
        public int TimePlanned { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Only non-negative number allowed.")]
        public int TimeSpent { get; set; }        
    }
}
