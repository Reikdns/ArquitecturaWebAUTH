namespace BLL;

using System;
using System.Collections.Generic;
using DAL;
using Entity;
using System.Data.SqlClient;
public class UserService
{
    private readonly ConnectionManager _connection;
    private readonly UserDataAcces _repository;
    public UserService(string cadenaDeConexión)
    {
        _connection = new ConnectionManager(cadenaDeConexión);
        _repository = new UserDataAcces(_connection);
    }


    public RequestResponse<IdentityModel> SaveUser(LoginUser user)
    {
        IdentityModel identityModel = new IdentityModel(user.Email, user.Rol);
        if(EmailValidate(user.Email) && IdentificationValidate(user.Identificacion))
        {
            try
            {
                _connection.Open();
                _repository.SaveDefaultUser(user);
                _connection.Close();
                return new RequestResponse<IdentityModel>(identityModel, "El usuario ha sido guardado correctamente.");
            }
            catch (Exception e)
            {
                return new RequestResponse<IdentityModel>(e.Message);
            }
        }
        else
        {
            return new RequestResponse<IdentityModel>("El usuario a registrar ya existe en el sistema.");
        }
    }

    private bool EmailValidate(string email)
    {
        var response = GetUserByEmail(email);

        if (response.Error)
        {
            return false;
        }
        return true;
    }

    private bool IdentificationValidate(string identificacion)
    {
        var response = GetUserByIdentification(identificacion);

        if (response.Error)
        {
            return false;
        }
        return true;
    }
    
    public RequestResponse<List<User>> SearchAllUsers()
    {
        try
        {
            _connection.Open();
            List<User> Users = _repository.SearchAll();
            _connection.Close();
            return new RequestResponse<List<User>>(Users, "OK");
        }
        catch (Exception e)
        {
            return new RequestResponse<List<User>>(e.Message);
        }
    }

    public RequestResponse<LoginUser> GetUserByEmail(string email)
    {
        try
        {
            _connection.Open();
            LoginUser user = _repository.GetUserByEmail(email);
            _connection.Close();

            if (user.Email == null)
            {
                return new RequestResponse<LoginUser>(user, "OK");
            }

            return new RequestResponse<LoginUser>(user, "El usuario ya se encuentra registrado");
        }
        catch (Exception e)
        {
            return new RequestResponse<LoginUser>(e.Message);
        }
    }

    public RequestResponse<LoginUser> GetUserByIdentification(string identificacion)
    {
        try
        {
            _connection.Open();
            LoginUser user = _repository.GetUserByIdentificaction(identificacion);
            _connection.Close();

            if (user.Email == null)
            {
                return new RequestResponse<LoginUser>(user, "OK");
            }

            return new RequestResponse<LoginUser>(user, "El usuario ya se encuentra registrado");
        }
        catch (Exception e)
        {
            return new RequestResponse<LoginUser>(e.Message);
        }
    }

}

public class RequestResponse<T>
{
    public bool Error { get; set; }
    public string Message { get; set; }
    public T Response { get; set; }
    public RequestResponse(T response, string message)
    {
        Error = false;
        Response = response;
        Message = message;
    }
    public RequestResponse(string message)
    {
        Error = true;
        Message = message;
    }
}
