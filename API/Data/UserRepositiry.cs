using System;
using API.DTOs;
using API.Entities;
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
        var user = await _context.Users.
        Include(x => x.Photos).
        FirstOrDefaultAsync(x => x.Id == id);
        
        return user;
    }

    public async Task<AppUser?> GetUserByNameAsync(string username)
    {
        var user = await _context.Users.
        Include(x => x.Photos).
        FirstOrDefaultAsync(x => x.UserName == username);
        
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

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        return await _context.Users
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .ToListAsync();    
    }

    public async Task<MemberDTO?> GetMemberAsync(string username)
    {
        return await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}
