using System;

namespace API.DTOs;

public class MemberDTO
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public int Age { get; set; }
    public string? PhotoURL { get; set; }
    public string? KnownAs  { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public required string Gender { get; set; }
    public string? Intrests { get; set; }
    public string? LookingFor { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public List<PhotoDTO>? Photos { get; set; }
}

