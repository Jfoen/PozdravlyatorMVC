using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozdravlyator.Data;
using Pozdravlyator.Models;

namespace Pozdravlyator.Controllers;

public class BirthdaysController : Controller
{
    private readonly AppDbContext _context;

    public BirthdaysController(AppDbContext context)
    {
        _context = context;
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
    public async Task<IActionResult> Create(BirthdayPerson person)
    {
        if (ModelState.IsValid)
        {
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
    public async Task<IActionResult> Edit(int id, BirthdayPerson person)
    {
        if (id != person.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(person);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(person);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var person = await _context.Birthdays
            .FirstOrDefaultAsync(m => m.Id == id);

        if (person == null)
        {
            return NotFound();
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