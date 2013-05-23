using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Aaron.Core.Infrastructure
{
    public class AaronManager
    {
        #region Initialization Methods
        /// <summary>Initializes a static instance of the Nop factory.</summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ICore Initialize(bool forceRecreate)
        {
            if (Singleton<ICore>.Instance == null || forceRecreate)
            {
                Debug.WriteLine("Constructing core " + DateTime.Now);
                Singleton<ICore>.Instance = CreateEngineInstance();
                Debug.WriteLine("Initializing core " + DateTime.Now);
                Singleton<ICore>.Instance.Initialize();
            }
            return Singleton<ICore>.Instance;
        }

        /// <summary>Sets the static core instance to the supplied core. Use this method to supply your own core implementation.</summary>
        /// <param name="core">The core to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(ICore core)
        {
            Singleton<ICore>.Instance = core;
        }

        /// <summary>
        /// Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <returns>A new factory</returns>
        public static ICore CreateEngineInstance()
        {
            return new AaronCore();
        }

        #endregion

        /// <summary>Gets the singleton Nop core used to access Nop services.</summary>
        public static ICore Current
        {
            get
            {
                if (Singleton<ICore>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<ICore>.Instance;
            }
        }
    }
}
