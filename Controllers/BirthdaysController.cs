using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozdravlyator.Data;
using Pozdravlyator.Models;

namespace Pozdravlyator.Controllers;

public class BirthdaysController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BirthdaysController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _context.Birthdays.ToListAsync();
        return View(list);
    }

    // Открывает страницу формы
    public IActionResult Create()
    {
        return View();
    }

    // Обрабатывает отправку формы
    [HttpPost]
    public async Task<IActionResult> Create(BirthdayPerson person, IFormFile PhotoFile)
    {
        if (ModelState.IsValid)
        {
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);

                var webRoot = _env.WebRootPath
              ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var folder = Path.Combine(webRoot, "images");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }

                person.PhotoPath = "/images/" + fileName;
            }

            _context.Birthdays.Add(person);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(person);

    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var person = await _context.Birthdays.FindAsync(id);

        if (person == null)
        {
            return NotFound();
        }

        return View(person);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, BirthdayPerson person, IFormFile PhotoFile)
    {
        if (id != person.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);

                var folder = Path.Combine(_env.WebRootPath, "images");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }

                person.PhotoPath = "/images/" + fileName;
            }
            else
            {
                // если фото не загрузили — оставить старое
                var existing = await _context.Birthdays.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                person.PhotoPath = existing?.PhotoPath;
            }

            _context.Update(person);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(person);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var person = await _context.Birthdays.FindAsync(id);

        if (person != null)
        {
            _context.Birthdays.Remove(person);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Upcoming()
    {
        var today = DateTime.Today;

        var list = await _context.Birthdays.ToListAsync();

        var upcoming = list
            .Where(x =>
            {
                var thisYearBirthday = new DateTime(today.Year, x.BirthDate.Month, x.BirthDate.Day);

                return thisYearBirthday >= today &&
                       thisYearBirthday <= today.AddDays(14);
            })
            .OrderBy(x =>
            {
                var thisYearBirthday = new DateTime(today.Year, x.BirthDate.Month, x.BirthDate.Day);
                return thisYearBirthday;
            })
            .ToList();

        return View(upcoming);
    }

    public async Task<IActionResult> Today()
    {
        var today = DateTime.Today;

        var list = await _context.Birthdays.ToListAsync();

        var todayBirthdays = list
            .Where(x => x.BirthDate.Month == today.Month &&
                        x.BirthDate.Day == today.Day)
            .ToList();

        return View(todayBirthdays);
    }
}