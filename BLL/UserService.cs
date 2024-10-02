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


    public RequestResponse<LoginUser> SaveUser(LoginUser user)
    {
        if(EmailValidate(user.Email))
        {
            try
            {
                _connection.Open();
                _repository.SaveDefaultUser(user);
                _connection.Close();
                return new RequestResponse<LoginUser>("La cuenta ha sido registrada exitosamente.");
            }
            catch (Exception e)
            {
                return new RequestResponse<LoginUser>(e.Message);
            }
        }
        else
        {
            return new RequestResponse<LoginUser>("El correo ingresado ya está asociado a una cuenta.");
        }
    }

    private bool Validate(string key, string value){

        var response = SearchByKey(key, value);

        if (response.Error)
        {  
            return true;
        } 
        return false;
    }

    private bool EmailValidate(string email)
    {
        var response = GetUserByEmail(email);

        if (response.Error)
        {
            return true;
        }
        return false;
    }
    
    public RequestResponse<List<User>> SearchAllUsers()
    {
        try
        {
            _connection.Open();
            List<User> Users = _repository.SearchAll();
            _connection.Close();
            return new RequestResponse<List<User>>(Users);
        }
        catch (Exception e)
        {
            return new RequestResponse<List<User>>(e.Message);
        }
    }

    public RequestResponse<User> SearchByKey(string key, string value)
    {
        try
        {
            _connection.Open();
            User user = _repository.SearchByKey(key, value);
            _connection.Close();

            if(user.Email == null)
            {
                throw new Exception("El usuario no ha sido encontrado");
            }

            return new RequestResponse<User>(user);
        }
        catch (Exception e)
        {
            return new RequestResponse<User>(e.Message);
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
                throw new Exception("El usuario no ha sido encontrado");
            }

            return new RequestResponse<LoginUser>(user);
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
    public RequestResponse(T response)
    {
        Error = false;
        Response = response;
    }
    public RequestResponse(string message)
    {
        Error = true;
        Message = message;
    }
}
