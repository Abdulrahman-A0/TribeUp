using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupExceptions
{
    public sealed class GroupAdminOnlyException
    : UnauthorizedDomainException
    {
        public GroupAdminOnlyException()
            : base("Only group admins can perform this action") { }
    }

}
