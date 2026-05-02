using System;
using System.ComponentModel.DataAnnotations;

namespace LogicParser.Api.Request_Response;

public class SignUpRequest
{
    [Required]
    public string Username{get; set;} = string.Empty;
    [Required]
    public string Password{get; set;} = string.Empty;
}
