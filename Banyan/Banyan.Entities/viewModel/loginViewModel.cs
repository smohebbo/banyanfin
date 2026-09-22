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
    public string UserId { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
