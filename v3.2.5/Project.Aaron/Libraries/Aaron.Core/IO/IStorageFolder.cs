using System;

namespace Aaron.Core.IO
{
    public interface IStorageFolder 
    {
        string GetPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        IStorageFolder GetParent();
    }
}