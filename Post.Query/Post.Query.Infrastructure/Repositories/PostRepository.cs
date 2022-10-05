using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _databaseContextFactory;

    public PostRepository(DatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        context.Posts.Add(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);
        if (post is null) return;
        context.Posts.Remove(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Where(p => p.Author.Contains(author))
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Where(p => p.Comments != null && p.Comments.Any())
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Where(p => p.Likes >= numberOfLikes)
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using DatabaseContext context = _databaseContextFactory.CreateDbContext();
        context.Posts.Update(post);
        _ = await context.SaveChangesAsync();
    }
}
