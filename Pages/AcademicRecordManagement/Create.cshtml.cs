using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using lab4.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace lab4.Pages.AcademicRecordManagement
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
            PopulateDropdownLists();
            return Page();
        }

        [BindProperty]
        public Academicrecord Academicrecord { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var existingRecord = await _context.Academicrecords
                .AnyAsync(ar => ar.StudentId == Academicrecord.StudentId && ar.CourseCode == Academicrecord.CourseCode);

            if (existingRecord)
            {
                ViewData["ErrorMessage"] = "The specific academic record already exist!";
                PopulateDropdownLists();

                //no full page reload or GET request, just re-render the current state of pagemodel, so need to repopulate the list
                return Page();
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdownLists();
                return Page();
            }

            _context.Academicrecords.Add(Academicrecord);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void PopulateDropdownLists()
        {
            ViewData["CourseCode"] = new SelectList(_context.Courses.Select(c => new
            {
                c.Code,
                DisplayText = c.Code + " - " + c.Title
            }), "Code", "DisplayText");

            ViewData["StudentId"] = new SelectList(_context.Students.Select(s => new
            {
                s.Id,
                DisplayText = s.Id + " - " + s.Name
            }), "Id", "DisplayText");
        }
    }
}
