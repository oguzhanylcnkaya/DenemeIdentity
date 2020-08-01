using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DenemeIdentity.Security
{
    public class CustomEmailConfirmationTokenProvider<T> : DataProtectorTokenProvider<T> where T:class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<CustomEmailConfirmationTokenProviderOptions> options) 
            : base(dataProtectionProvider, options)
        {
        }
    }
}
