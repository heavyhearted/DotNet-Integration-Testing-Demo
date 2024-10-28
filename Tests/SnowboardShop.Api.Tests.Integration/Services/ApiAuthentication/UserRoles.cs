namespace SnowboardShop.Api.Tests.Integration.Services.ApiAuthentication;
/*
The roles are defined as flags so that a user can have multiple roles.
We use bitwise operations to set and check the roles in order 
to ensure uniqueness and to allow for multiple roles. 
Formula is 1 x 2^n where n is the position of the bit.
*/

[Flags]
public enum UserRoles
{
    RegularUser = 0,                   // 0000 - Regular User (no special roles)
    TrustedMember = 1 << 0,            // 0001 - Trusted Member role
    Admin = 1 << 1                     // 0010 - Admin role
}