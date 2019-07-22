using System;

namespace EntityModel
{
    [Flags]
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        //     = 4
        //     = 8
        // Todo: Add more options

        All = Male | Female
    }
}
