namespace Entity;
public class User
{
    public string? Rol { get; set;} 

    public string? Identificacion { get; set; }

    public string? Email { get; set; }

    public int Id { get; set; }

}

public class LoginUser
{
    public int Id { get; set; }
    public string Rol { get; set; }
    public string Identificacion { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Salt { get; set; }
}

public class IdentityModel{
    public string Email { get; set; }
    public string Rol { get; set; }

    public IdentityModel(string email, string rol)
    {
        Email = email;
        Rol = rol;
    }
}

