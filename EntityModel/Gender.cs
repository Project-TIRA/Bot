using System;

namespace EntityModel
{
    [Flags]
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        NonBinary = 4,
        All = Male | Female | NonBinary
        //     = 4
        //     = 8
        // Todo: Add more options
    }
}
