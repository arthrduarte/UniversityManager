using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab4.DataAccess;

namespace lab4.Pages.AcademicRecordManagement
{
    public class EditModel : PageModel
    {
        private readonly lab4.DataAccess.StudentrecordContext _context;

        public EditModel(lab4.DataAccess.StudentrecordContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Academicrecord Academicrecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicrecord =  await _context.Academicrecords.FirstOrDefaultAsync(m => m.StudentId == id);
            if (academicrecord == null)
            {
                return NotFound();
            }
            Academicrecord = academicrecord;
           ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code");
           ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Academicrecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcademicrecordExists(Academicrecord.StudentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AcademicrecordExists(string id)
        {
            return _context.Academicrecords.Any(e => e.StudentId == id);
        }
    }
}
