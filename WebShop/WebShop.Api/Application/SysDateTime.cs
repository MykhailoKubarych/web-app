using System;

namespace WebShop.Api.Application
{
    public abstract class SysDateTime
    {
        public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
    }
}