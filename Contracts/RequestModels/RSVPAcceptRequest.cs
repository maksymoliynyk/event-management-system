using System.ComponentModel.DataAnnotations;

namespace Contracts.RequestModels
{
    public class RSVPAcceptRequest
    {
        [Required]
        public bool AcceptInvite { get; init; }
    }
}