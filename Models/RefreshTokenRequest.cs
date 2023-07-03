using System.ComponentModel.DataAnnotations;

namespace altenar_test_webapi.Models;

public class RefreshTokenRequest
{
    [Required]
    public string accessJwt { get; set; }
    [Required]
    public string refreshToken { get; set; }
}