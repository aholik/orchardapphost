﻿using Orchard.Environment.Descriptor.Models;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    public interface IDefaultTransientShellDescriptorProvider
    {
        ShellDescriptor GetDefaultShellDescriptor();
    }


    public class DefaultTransientShellDescriptorProvider : IDefaultTransientShellDescriptorProvider
    {
        private readonly ShellDescriptor _defaultShellDescriptor;


        public DefaultTransientShellDescriptorProvider(ShellDescriptor defaultShellDescriptor)
        {
            _defaultShellDescriptor = defaultShellDescriptor;
        }
        

        public ShellDescriptor GetDefaultShellDescriptor()
        {
            return _defaultShellDescriptor;
        }
    }
}
