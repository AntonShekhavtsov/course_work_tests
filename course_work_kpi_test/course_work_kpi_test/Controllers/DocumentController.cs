using Aspose.Words;
using course_work_kpi_test.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace course_work_kpi_test.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckDocument(IFormFile uploadedFile, int duration, int budget)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var stream = new MemoryStream())
            {
                uploadedFile.CopyTo(stream);
                Document doc = new Document(stream);

                var results = new List<DocumentCheckResult>();

                CheckSection(doc, "Мета проекту", results);
                CheckSection(doc, "Основні функції", results);
                CheckSection(doc, "Вимоги до продуктивності", results);

                CheckProjectDuration(doc, duration, results);

                CheckBudget(doc, budget, results);

                return View("Results", results);
            }
        }

        private void CheckSection(Document doc, string sectionName, List<DocumentCheckResult> results)
        {
            var found = false;
            foreach (Paragraph para in doc.GetChildNodes(NodeType.Paragraph, true))
            {
                if (para.GetText().Contains(sectionName))
                {
                    found = true;
                    break;
                }
            }

            var status = found ? "PASSED" : "FAILED";
            results.Add(new DocumentCheckResult { SectionName = sectionName, Status = status });
        }
        private void CheckProjectDuration(Document doc, int duration, List<DocumentCheckResult> results)
        {
            string text = doc.GetText();
            var match = Regex.Match(text, @"Загальна тривалість розробки:\s*(\d+)(?:-(\d+))?");
            if (match.Success)
            {
                int total_duration = int.Parse(match.Groups[1].Value);
                var status = total_duration <= duration ? "PASSED" : "FAILED";
                results.Add(new DocumentCheckResult { SectionName = "Project Duration", Status = status });
            }
            else
            {
                results.Add(new DocumentCheckResult { SectionName = "Project Duration", Message = "Project duration check failed.", Status = "FAILED" });
            }
        }

        private void CheckBudget(Document doc, int budget, List<DocumentCheckResult> results)
        {
            string text = doc.GetText();
            var match = Regex.Match(text, @"Вартість розробки:\s*(\d+)(?:-(\d+))?");
            if (match.Success)
            {
                int total_budget = int.Parse(match.Groups[1].Value);
                var status = total_budget <= budget ? "PASSED" : "FAILED";
                results.Add(new DocumentCheckResult { SectionName = "Budget Check", Status = status});
            }
            else
            {
                results.Add(new DocumentCheckResult { SectionName = "Budget Check", Message = "Budget check failed.", Status = "FAILED" });
            }
        }
    }
}
