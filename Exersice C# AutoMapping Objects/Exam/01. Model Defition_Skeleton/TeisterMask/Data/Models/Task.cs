using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.Data.Models
{
    public class Task
    {
        public Task()
        {
            this.EmployeesTasks = new HashSet<EmployeeTask>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public DateTime OpenDate { get; set; }
        public DateTime DueDate { get; set; }
        [Required]
        public ExecutionType ExecutionType { get; set; }
        [Required]
        public LabelType LabelType { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<EmployeeTask> EmployeesTasks { get; set; }
    }
}

