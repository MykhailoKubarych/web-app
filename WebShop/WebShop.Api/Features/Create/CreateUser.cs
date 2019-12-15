using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebShop.Api.Features
{
    public class CreateUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
    }
}