using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Taskify.Models
{
    public class Duty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DutyName { get; set; }
        public string Description { get; set; }
        public DutyStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Active { get; set; }
        public bool Archived { get; set; }
        public int UserId { get; set; }

    }

    public enum DutyStatus
    {
        Bekliyor,
        DevamEdiyor,
        Tamamlandi
    }
}