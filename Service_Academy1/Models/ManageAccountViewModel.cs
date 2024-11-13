using Microsoft.AspNetCore.Identity;
using Service_Academy1.Models;
using System.ComponentModel.DataAnnotations;

public class ManageAccountViewModel
{
    public List<ApplicationUser>? Users { get; set; }
    public CreateAccountViewModel? CreateAccountForm { get; set; }
}