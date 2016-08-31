using System;

namespace Hafnium
{
    /// <summary>
    /// Describes a lookup.
    /// </summary>
    public interface ILookup
    {
    }


    /// <summary>
    /// Describes a lookup.
    /// </summary>
    /// <typeparam name="Rq">Type of request message.</typeparam>
    /// <typeparam name="Rp">Type of response message.</typeparam>
    public interface ILookup<Rq,Rp> : ILookup
    {
        Rp Execute( Rq request );
    }
}
