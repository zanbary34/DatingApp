using System;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SQLitePCL;

namespace API.Extenstions;

public static class ClaimsPrincipleExtenstions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new Exception("Cannot get username from token");
        
        return username;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new Exception("Cannot get username from token"));
        
        return userId;
    }
}
