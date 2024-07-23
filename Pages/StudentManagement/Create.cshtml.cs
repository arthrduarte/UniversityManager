using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using lab4.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace lab4.Pages.StudentManagement
{
    public class CreateModel : PageModel
    {
        private readonly lab4.DataAccess.StudentrecordContext _context;

        public CreateModel(lab4.DataAccess.StudentrecordContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; } = new Student();

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await IsStudentIdTakenAsync(Student.Id))
            {
                ModelState.AddModelError("Student.Id", "Student ID already taken.");
                return Page();
            }


            _context.Students.Add(Student);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task<bool> IsStudentIdTakenAsync(string studentId)
        {
            return await _context.Students.AnyAsync(s => s.Id == studentId);
        }
    }
}
