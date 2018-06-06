using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GHIElectronics.TinyCLR.Devices.Adc.Provider {
    public interface IAdcControllerProvider {
        int ChannelCount {
            get;
        }

        int ResolutionInBits {
            get;
        }

        int MinValue {
            get;
        }

        int MaxValue {
            get;
        }

        ProviderAdcChannelMode ChannelMode {
            get;
            set;
        }

        bool IsChannelModeSupported(ProviderAdcChannelMode channelMode);
        void AcquireChannel(int channel);
        void ReleaseChannel(int channel);
        int ReadValue(int channelNumber);
    }

    public interface IAdcProvider {
        IAdcControllerProvider[] GetControllers();
    }

    public class AdcProvider : IAdcProvider {
        private IAdcControllerProvider[] controllers;
        private readonly static Hashtable providers = new Hashtable();

        public string Name { get; }

        public IAdcControllerProvider[] GetControllers() => this.controllers;

        private AdcProvider(string name) {
            this.Name = name;

            var api = Api.Find(name, ApiType.AdcProvider);

            this.controllers = new IAdcControllerProvider[DefaultAdcControllerProvider.GetControllerCount(api.Implementation)];

            for (var i = 0; i < this.controllers.Length; i++)
                this.controllers[i] = new DefaultAdcControllerProvider(api.Implementation, i);
        }

        public static IAdcProvider FromId(string id) {
            if (AdcProvider.providers.Contains(id))
                return (IAdcProvider)AdcProvider.providers[id];

            var res = new AdcProvider(id);

            AdcProvider.providers[id] = res;

            return res;
        }
    }

    internal class DefaultAdcControllerProvider : IAdcControllerProvider {
#pragma warning disable CS0169
        private readonly IntPtr nativeProvider;
        private readonly int idx;
#pragma warning restore CS0169

        internal DefaultAdcControllerProvider(IntPtr nativeProvider, int idx) {
            this.nativeProvider = nativeProvider;
            this.idx = idx;
            this.AcquireNative();
        }

        ~DefaultAdcControllerProvider() => this.ReleaseNative();

        public extern ProviderAdcChannelMode ChannelMode {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
        }

        public extern int ChannelCount {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        public extern int MaxValue {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        public extern int MinValue {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        public extern int ResolutionInBits {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsChannelModeSupported(ProviderAdcChannelMode channelMode);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AcquireChannel(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ReleaseChannel(int channel);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int ReadValue(int channelNumber);

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern private void AcquireNative();

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern private void ReleaseNative();

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern static internal int GetControllerCount(IntPtr nativeProvider);
    }
}
