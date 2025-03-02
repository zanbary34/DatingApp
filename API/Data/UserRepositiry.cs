using System;
using API.DTOs;
using API.Entities;
using API.Extenstions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepositiry(DataContext context, IMapper mapper) : IUserRepository
{
    private DataContext _context = context;

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var users = await _context.Users
        .Include(x => x.Photos)
        .ToListAsync();

        return users;
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        return user;
    }

    public async Task<AppUser?> GetUserByNameAsync(string username)
    {
        var user = await _context.Users.
        Include(x => x.Photos).
        SingleOrDefaultAsync(x => x.UserName == username);
        
        return user;
    }

    public async Task<bool> SaveAllAsync()
    {
         return await _context.SaveChangesAsync() > 0 ;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {
        var query =  _context.Users.AsQueryable();
        
        query = query.Where(x => x.UserName != userParams.CurrentUserName);

        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }

        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));  
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1)); 

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };
        
        return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider),
            userParams.PageNumber, userParams.PageSize);  
    }

    public async Task<MemberDTO?> GetMemberAsync(string username)
    {
        return await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}
