using System;

namespace Snifter.Protocol.Internet
{
    [Flags]
    public enum FragmentationFlags : byte 
    {
        DontFragment = 0x01,
        MoreFragments = 0x02
    }
}
