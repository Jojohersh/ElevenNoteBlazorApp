
using ElevenNoteWebApp.Server.Data;
using Microsoft.EntityFrameworkCore;

public class NoteService : INoteService
{
    private readonly ApplicationDbContext _context;
    private string _userId;
    public NoteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateNoteAsync(NoteCreate model)
    {
        var noteEntity = new Note
        {
            Title = model.Title,
            Content = model.Content,
            OwnerId = _userId,
            CreatedUtc = DateTime.UtcNow,
            CategoryId = model.CategoryId
        };

        _context.Notes.Add(noteEntity);
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> DeleteNoteAsync(int id)
    {
        var noteEntity = await _context.Notes.FindAsync(id);
        if (noteEntity?.OwnerId != _userId)
            return false;
        _context.Notes.Remove(noteEntity);
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> DeleteNoteAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
    {
        var notes = _context.Notes
            .Where(n => n.OwnerId== _userId)
            .Select(n => new NoteListItem
            {
                Id= n.Id,
                Title = n.Title,
                CategoryName = n.Category.Name,
                CreatedUtc = n.CreatedUtc
            })
            .ToList();
        return notes;
    }

    public async Task<NoteDetail> GetNoteByIdAsync(int id)
    {
        var note = await _context.Notes
            .Include(nameof(Category))
            .FirstOrDefaultAsync(n => n.Id == id && n.OwnerId == _userId);
        if (note is null)
            return null;
        return new NoteDetail
        {
            Id = note.Id,
            Title = note.Title,
            Content= note.Content,
            CategoryName = note.Category.Name,
            CategoryId= note.Category.Id,
            CreatedUtc = note.CreatedUtc,
            ModifiedUtc= note.ModifiedUtc
        };
    }

    public void SetUserId(string userId) => _userId = userId;

    public async Task<bool> UpdateNoteAsync(NoteEdit model)
    {
        if (model == null) return false;
        var noteEntity = await _context.Notes.FirstOrDefaultAsync(n => n.Id == model.Id && n.OwnerId == _userId);
        if (noteEntity is null) return false;
        noteEntity.Title = model.Title;
        noteEntity.Content = model.Content;
        noteEntity.CategoryId = model.CategoryId;
        noteEntity.ModifiedUtc = DateTimeOffset.Now;
        
        return await _context.SaveChangesAsync() == 1;
    }
}

