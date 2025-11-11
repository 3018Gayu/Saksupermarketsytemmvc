namespace Saksupermarketsytemmvc.web.Models.ViewModels
{
    
        public class LoginViewModel
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class RegisterViewModel
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string Role { get; set; } = null!;
        }

        public class ForgotPasswordViewModel
        {
            public string Email { get; set; } = null!;
            public string NewPassword { get; set; } = null!;
        }
    }


