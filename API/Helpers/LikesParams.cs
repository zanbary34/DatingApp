using System;

namespace API.Helpers;

public class LikesParams : PaginationParams
{
    public string Predicate { get; set; } = "liked";
}
