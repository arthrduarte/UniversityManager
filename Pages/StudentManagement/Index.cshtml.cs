using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using lab4.DataAccess;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace lab4.Pages.StudentManagement
{
    public class IndexModel : PageModel
    {
        private readonly lab4.DataAccess.StudentrecordContext _context;

        public IndexModel(lab4.DataAccess.StudentrecordContext context)
        {
            _context = context;
        }
        
        public List<StudentViewModel> Student { get;set; } = new List<StudentViewModel>();

        public async Task OnGetAsync(string sortBy, string deleteId)
        {
            if (!string.IsNullOrEmpty(deleteId))
            {
                await OnPostDeleteAsync(deleteId);
            }

            Student = await _context.Students
                .Include(s => s.Academicrecords)
                .ThenInclude(ar => ar.CourseCodeNavigation)
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    NumberOfCourses = s.Academicrecords.Count,
                    AverageGrade = s.Academicrecords.Any() ? s.Academicrecords.Average(ar => ar.Grade.HasValue ? ar.Grade.Value : 0) : 0
                })
                .ToListAsync();

            SortColumns(sortBy);
        }

        public class StudentViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int NumberOfCourses { get; set; }
            public double AverageGrade { get; set; }
        }
        
        public void SortColumns(string sortBy)
        {
            switch (sortBy)
            {
                case "name":
                    Student.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase));
                    break;
                case "course":
                    Student.Sort((s1, s2) => s1.NumberOfCourses.CompareTo(s2.NumberOfCourses));
                    break;
                case "grade":
                    Student.Sort((s1, s2) => s1.AverageGrade.CompareTo(s2.AverageGrade));
                    break;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest();
            }

            var student = await _context.Students
                .Include(s => s.Academicrecords)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound();
            }

            _context.Academicrecords.RemoveRange(student.Academicrecords);

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");

        }
    }
}
