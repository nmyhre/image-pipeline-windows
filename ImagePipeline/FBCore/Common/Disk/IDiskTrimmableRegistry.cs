﻿namespace FBCore.Common.Disk
{
    /// <summary>
    /// A class which keeps a list of other classes to be notified of system disk events.
    ///
    /// <para />If a class uses a lot of disk space and needs these notices from the system, 
    /// it should implement the <see cref="IDiskTrimmable"/> interface.
    ///
    /// <para />Implementations of this class should notify all the trimmables that have 
    /// registered with it when they need to trim their disk usage.
    /// </summary>
    public interface IDiskTrimmableRegistry
    {
        /// <summary>
        /// Register an object.
        /// </summary>
        void RegisterDiskTrimmable(IDiskTrimmable trimmable);

        /// <summary>
        /// Unregister an object.
        /// </summary>
        void UnregisterDiskTrimmable(IDiskTrimmable trimmable);
    }
}
