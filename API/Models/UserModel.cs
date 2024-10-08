namespace API.Models;
using Entity;

public class UserInputModel : UserViewModel
{
    public string Password { get; set; }
    public string Salt { get; set; }

    public UserInputModel()
    {
    
    }
}

public class UserViewModel 
{
    public string Email { get; set; } 

    public string Rol { get; set;} 

    public string Identificacion { get; set; }

    public int UserId { get; set; }

    public UserViewModel()
    {
        
    }

    public UserViewModel(User user)
    {
        Email = user.Email;
        Rol = user.Rol;
        Identificacion = user.Identificacion;
        UserId = user.Id;
    }
} 