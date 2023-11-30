﻿
using System.ComponentModel.DataAnnotations;

namespace JWTApi.Models
{
    public class UserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Rol { get; set; }
    }
}
