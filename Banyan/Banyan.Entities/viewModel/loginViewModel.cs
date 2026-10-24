using System;
using System.ComponentModel.DataAnnotations;

namespace Banyan.Entities.viewModel;

public class loginViewModel
{
    public loginViewModel()
    {
        UserId = string.Empty;
        Password = string.Empty;
    }
    [Required(ErrorMessage = "User ID is required.")]
    [StringLength(100, ErrorMessage = "User ID cannot exceed 100 characters.")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public string SecretKey { get; set; } = string.Empty;
}
