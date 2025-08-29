using System;

namespace Habitus.Domain.Enums
{
    public enum UserRole
    {
        User = 1,
        Admin = 2,
        SuperAdmin = 3
    }

    public enum LoginProvider
    {
        Local = 1,
        Google = 2,
        Microsoft = 3,
        Facebook = 4
    }
}
