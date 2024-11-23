using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Service_Academy1.Models
{
    public class DepartmentsModel
    {
        [Key]
        public int DepartmentId { get; set; }
        public string Department { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }
}
