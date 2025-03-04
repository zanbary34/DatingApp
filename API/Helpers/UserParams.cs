using System;
using API.Helpers;

namespace API.Extenstions;

public class UserParams : PaginationParams
{
    public string? Gender { get; set; }
    public string? CurrentUserName { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActive";
}
