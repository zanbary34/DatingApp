using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SQLitePCL;

namespace API.Extenstions;

public static class DateTimeExtensions
{
    public static int CalculateAge(this DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var age = today.Year - dob.Year;

        if (dob > today.AddYears(-age)) age --;

        return age;
    }
}
