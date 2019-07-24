using System;

namespace EntityModel
{
    [Flags]
    public enum FamilyStatus
    {
        Single = 0,
        Pregnant = 1,
        Parenting = 2,
        All = Pregnant | Parenting
    }
}
