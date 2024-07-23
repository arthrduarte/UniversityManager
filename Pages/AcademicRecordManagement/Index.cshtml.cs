using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using lab4.DataAccess;

namespace lab4.Pages.AcademicRecordManagement
{
    public class IndexModel : PageModel
    {
        private readonly lab4.DataAccess.StudentrecordContext _context;

        public IndexModel(lab4.DataAccess.StudentrecordContext context)
        {
            _context = context;
        }

        public List<AcademicRecordViewModel> Academicrecord { get;set; } = default!;

        public async Task OnGetAsync(string sortBy, string deleteId, string studentId)
        {
            Console.WriteLine("Chegamos aqui");
            if(!string.IsNullOrEmpty(deleteId))
            {
                Console.WriteLine("Chegamos aqui 2");
                await OnPostAsync(deleteId, studentId);
            }
            Console.WriteLine("Chegamos aqui 3");
            Academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .Select(s => new AcademicRecordViewModel
                {
                    CourseId = s.CourseCode,
                    CourseTitle = s.CourseCodeNavigation.Title,
                    StudentId = s.StudentId,
                    StudentName = s.Student.Name,
                    Grade = s.Grade
                })
                .ToListAsync();
            SortColumns(sortBy);
        }
        public class AcademicRecordViewModel
        {
            public string CourseId { get; set; }
            public string CourseTitle { get; set; }
            public string StudentId { get; set; }
            public string StudentName { get; set; }
            public int? Grade { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string deleteId, string studentId)
        {
            if (string.IsNullOrEmpty(deleteId) || string.IsNullOrEmpty(studentId))
            {
                return BadRequest();
            }

            var academicRecord = await _context.Academicrecords
                .FirstOrDefaultAsync(ar => ar.CourseCode == deleteId && ar.StudentId == studentId);

            if (academicRecord == null)
            {
                return NotFound();
            }

            _context.Academicrecords.Remove(academicRecord);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }


        public void SortColumns(string sortBy)
        {
            switch (sortBy)
            {
                case "course":
                    Academicrecord.Sort((s1, s2) => string.Compare(s1.CourseTitle, s2.CourseTitle, StringComparison.OrdinalIgnoreCase));
                    break;
                case "student":
                    Academicrecord.Sort((s1, s2) => string.Compare(s1.StudentName, s2.StudentName, StringComparison.OrdinalIgnoreCase));
                    break;
                case "grade":
                    Academicrecord = Academicrecord.OrderBy(ar => ar.Grade).ToList();
                    break;
            }
        }
    }
}
