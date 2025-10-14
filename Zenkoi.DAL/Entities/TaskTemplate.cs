using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class TaskTemplate
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int? PondId { get; set; }
        public Pond? Pond { get; set; }

        public int AssignedToUserId { get; set; }
        public ApplicationUser AssignedTo { get; set; }

        public DateTime ScheduledAt { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }

        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
    }
}
