using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using lab4.DataAccess;
using static lab4.Pages.StudentManagement.IndexModel;
using System.Globalization;

namespace lab4.Pages.StudentManagement
{
    public class DetailsModel : PageModel
    {
        private readonly lab4.DataAccess.StudentrecordContext _context;

        public DetailsModel(lab4.DataAccess.StudentrecordContext context)
        {
            _context = context;
        }

        public Student Student { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id, string sortBy)
        {

            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Academicrecords)
                .ThenInclude(ar => ar.CourseCodeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }
            else
            {
                Student = student;
            }

            SortColumns(sortBy);

            return Page();
        }

        public void SortColumns(string sortBy)
        {
            switch (sortBy)
            {
                case "title":
                    Student.Academicrecords = Student.Academicrecords.OrderBy(ar => ar.CourseCodeNavigation.Title).ToList();
                    break;
                case "grade":
                    Student.Academicrecords = Student.Academicrecords.OrderBy(ar => ar.Grade).ToList();
                    break;
            }
        }
        public class StudentViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Course { get; set; }
            public double Grade { get; set; }
        }
    }
}
