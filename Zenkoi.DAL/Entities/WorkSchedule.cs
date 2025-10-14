using System;
using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class WorkSchedule
    {      
        public int Id { get; set; }   
        public int StaffId { get; set; }
        public ApplicationUser Staff { get; set; } = default!;      
        public int TaskTemplateId { get; set; }
        public TaskTemplate TaskTemplate { get; set; } = default!;
        public DateTime WorkDate { get; set; } 
        public TimeSpan? StartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan? EndTime { get; set; }   
        public DateTime? CheckedInAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }           
        public string? Notes { get; set; }
        public string? ManagerNotes { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedByUserId { get; set; }
        public ApplicationUser CreatedBy { get; set; } = default!;
    
    }
}
