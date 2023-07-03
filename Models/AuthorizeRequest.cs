using System.ComponentModel.DataAnnotations;

namespace altenar_test_webapi.Models;
public class AutorizeRequest
{
    [Required]
    public string userName {get; set;}
    [Required]
    public string password {get; set;}
}