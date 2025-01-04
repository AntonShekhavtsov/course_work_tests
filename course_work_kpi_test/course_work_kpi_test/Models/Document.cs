using System.ComponentModel.DataAnnotations;

namespace course_work_kpi_test.Models
{
    public class DocumentCheckResult
    {
        public string SectionName { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
