
namespace notepad_backend.Entities;

public class NotePadEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public bool IsPinned { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public NotePedDetailEntity Detail { get; set; }
}