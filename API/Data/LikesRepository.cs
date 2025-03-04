using API.DTOs;
using API.Entities;
using API.Extenstions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
        
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams, int userId)
    {
        var likes = context.Likes.AsQueryable();
        var query =  context.Users.AsQueryable();


        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes.Where(x => x.SourceUserId == userId).Select(x => x.TargetUser!);
                break;

            case "likedBy":
                query =  likes
                    .Where(x => x.TargetUserId == userId)
                    .Select(x => x.SourceUser!);

                break;

            default:
                var likeIds = await GetCurrentUserLikeIds(userId);
                query = likes
                    .Where(x => x.TargetUserId == userId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser!);
                    
                break;
        }

        return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider),
            likesParams.PageNumber, likesParams.PageSize);  
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}