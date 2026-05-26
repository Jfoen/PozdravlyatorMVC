using System.ComponentModel.DataAnnotations;

namespace Pozdravlyator.Models;

public class BirthdayPerson
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime BirthDate { get; set; }

    public string? PhotoPath { get; set; }
}