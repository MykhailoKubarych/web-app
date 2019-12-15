using System;
using WebShop.Api.Application;

namespace WebShop.Api.Domain
{
    public class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = SysDateTime.UtcNow();
        }
    }
}