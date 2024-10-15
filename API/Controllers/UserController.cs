namespace API.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Entity;
using BLL;
using API.Models;
using API.Helper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UserController : Controller
{
    const string ACCESO_DENEGADO = "ACCESO DENEGADO";
    const string ACCESO_CONCEDIDO = "ACCESO CONCEDIDO";
    const string ESTUDIANTE = "ESTUDIANTE";
    const string PROFESOR = "PROFESOR";
    const string ADMIN = "ADMIN";

    private readonly UserService _userService;
    public IConfiguration Configuration { get; }

    public UserController(IConfiguration configuration)
    {
        Configuration = configuration;
        string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        _userService = new UserService(connectionString);
    }

    [HttpGet("get-users")]
    [Authorize(Roles="Admin")]
    public ActionResult<List<UserViewModel>> GetUsers()
    {   
        var response = _userService.SearchAllUsers();

        if(response.Error)
        {
            return BadRequest(response.Message);
        }

        var users = response.Response.Select(p => new UserViewModel(p));

        return Ok(users);
    }

    [Authorize(Roles="Admin")]
    [HttpPost("register-default-user")]
    public ActionResult<DefaultUserLoginModel> RegisterDefaultUser(DefaultUserLoginModel user)
    {
        HashedPassword hashedPassword = HashHelper.Hash(user.Password);
        user.Password = hashedPassword.Password;
        string salt = hashedPassword.Salt;

        var response = _userService.SaveUser(MapUser(user, salt));

        if(response.Error)
        {
            return Ok(response);
        }

        return Ok(response);
    } 

    [HttpGet("identity")]
    [Authorize]
    public IActionResult GetIdentity()
    {
        var nameIdentifier = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
        var rol = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role);
        if(nameIdentifier == null || rol == null) {return Forbid();}
        IdentityModel identityModel = new IdentityModel(nameIdentifier.Value, rol.Value);
        return Ok(identityModel == null ? "" : identityModel);
    }

    [HttpGet("StudentVerify")]
    [Authorize]
    public IActionResult StudentVerify()
    {
        var nameIdentifier = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
        var rol = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role);
        if(nameIdentifier == null || rol == null) {return Forbid();}
        if(rol.Value.ToUpper().Equals(ESTUDIANTE)){return Ok(ACCESO_CONCEDIDO);}
        return Unauthorized(ACCESO_DENEGADO);
    }

    [HttpGet("ProfesorVerify")]
    [Authorize]
    public IActionResult ProfesorVerify()
    {
        var nameIdentifier = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
        var rol = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role);
        if(nameIdentifier == null || rol == null) {return Forbid();}
        if(rol.Value.ToUpper().Equals(PROFESOR)){return Ok(ACCESO_CONCEDIDO);}
        return Unauthorized(ACCESO_DENEGADO);
    }

    [HttpGet("AdminVerify")]
    [Authorize]
    public IActionResult AdminVerify()
    {
        var nameIdentifier = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier);
        var rol = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role);
        if(nameIdentifier == null || rol == null) {return Forbid();}
        if(rol.Value.ToUpper().Equals(ADMIN)){return Ok(ACCESO_CONCEDIDO);}
        return Unauthorized(ACCESO_DENEGADO);
    }



    private LoginUser MapUser(DefaultUserLoginModel user, string salt)
    {
        return new LoginUser{
            Rol = user.Rol,
            Identificacion = user.UsuarioIdentificacion,
            Email = user.Email,
            Password = user.Password,
            Salt = salt
        };
    }
}
