
public interface INoteService
{
    Task<IEnumerable<NoteListItem>> GetAllNotesAsync();
    Task<bool> CreateNoteAsync(NoteCreate model);
    Task<NoteDetail> GetNoteByIdAsync(int id);
    Task<bool> UpdateNoteAsync(NoteEdit model);
    Task<bool> DeleteNoteAsync(int id);
    Task<bool> DeleteNoteAsync(string userId);
    void SetUserId(string userId);
}

