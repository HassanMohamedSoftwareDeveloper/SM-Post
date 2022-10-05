using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DatabaseContextFactory _databaseContextFactory;

    public CommentRepository(DatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public async Task CreateAsync(CommentEntity comment)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        context.Comments.Add(comment);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        var comment = await GetByIdAsync(commentId);
        if (comment is null) return;
        context.Comments.Remove(comment);
        _ = await context.SaveChangesAsync();
    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Comments
            .FirstOrDefaultAsync(p => p.CommentId == commentId);
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        context.Comments.Update(comment);
        _ = await context.SaveChangesAsync();
    }
}
