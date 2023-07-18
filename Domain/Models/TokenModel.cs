using System;

namespace Domain.Models
{
    public class TokenModel
    {
        public string Token { get; init; }
        public DateTime ExpiresIn { get; init; }
    }
}