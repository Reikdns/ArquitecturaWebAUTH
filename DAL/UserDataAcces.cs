namespace DAL;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Entity;

public class UserDataAcces{

    private readonly SqlConnection _connection;
    
    public UserDataAcces(ConnectionManager connection){
        _connection = connection._connection;
    }

    public void SaveDefaultUser(LoginUser user)
    {
        using(var command = _connection.CreateCommand())
        {
            SqlTransaction transaction = _connection.BeginTransaction();
            command.Transaction = transaction;
            try
            {
                SaveLoginUser(user, command);
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
            }      
        }
    }

    private static void SaveLoginUser(LoginUser user, SqlCommand command)
    {
        command.CommandText = @"INSERT INTO Users(usuarioidentificacion, email, clave, salt, rol)"
                            + "VALUES (@usuarioidentificacion, @email, @clave, @salt, @rol)";
        command.Parameters.AddWithValue("@usuarioidentificacion", user.Identificacion);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@clave", user.Password);
        command.Parameters.AddWithValue("@salt", user.Salt);
        command.Parameters.AddWithValue("@rol", user.Rol);

        command.ExecuteNonQuery();
    }

    public List<User> SearchAll ( ) {
        SqlDataReader dataReader;
        List<User> users = new List<User> ( );
        using (var command = _connection.CreateCommand ( )) {
            command.CommandText = "SELECT * FROM UsersTemp";
            dataReader = command.ExecuteReader ( );
            if (dataReader.HasRows) {
                while (dataReader.Read ( )) {
                    User user = DataMapInReader (dataReader);
                    users.Add (user);
                }
            }
        }
        return users;
    }

    public User SearchByKey(string key, string value)
    {
        User user = Select(key, value);
        return user;
    }

    public LoginUser DefaultSearchByKey(string key, string value)
    {
        LoginUser user = DefaultSelect(key, value);
        return user;
    }

    private User Select(string key, string value)
    {
        SqlDataReader dataReader;
        User user = new User();

        using(var command = _connection.CreateCommand())
        {
            command.CommandText = $@"SELECT * FROM UsersTemp WHERE @value = {key}";
            command.Parameters.AddWithValue("@value", value);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                dataReader.Read();
                user = DataMapInReader(dataReader);
            }
            return user;
        }
    }

    private LoginUser DefaultSelect(string key, string value)
    {
        SqlDataReader dataReader;
        LoginUser user = new LoginUser();

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = $@"SELECT * FROM UsersTemp WHERE @value = {key}";
            command.Parameters.AddWithValue("@value", value);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                dataReader.Read();
                user = DefaultDataMapInReader(dataReader);
            }
            return user;
        }
    }

    public LoginUser GetUserByIdentificaction(string identificacion)
    {
        SqlDataReader dataReader;
        LoginUser user = new LoginUser();

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = $@"SELECT * FROM Usuarios WHERE @identificacion = identificacion";
            command.Parameters.AddWithValue("@identificacion", identificacion);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                dataReader.Read();
                user = DefaultDataMapInReader(dataReader);
            }
            return user;
        }
    }

        public LoginUser GetUserByEmail(string email)
    {
        SqlDataReader dataReader;
        LoginUser user = new LoginUser();

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = $@"SELECT * FROM Users WHERE @email = email";
            command.Parameters.AddWithValue("@email", email);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                dataReader.Read();
                user = DefaultDataMapInReader(dataReader);
            }
            return user;
        }
    }

    private User DataMapInReader (SqlDataReader dataReader) {
        if (!dataReader.HasRows) return null;
        User user = new User ( );
        user.Id = (int) dataReader["id"];
        user.Email = (string) dataReader["email"];          
        user.Rol = (string) dataReader["rol"];
        user.Identificacion = ColumnValueIsNull(dataReader["identificacion"]) ? null : (string) dataReader["identificacion"];;
        return user;
    }

    private bool ColumnValueIsNull(object reader)
    {
        return DBNull.Value.Equals(reader);
    }

    private LoginUser DefaultDataMapInReader(SqlDataReader dataReader)
    {
        if (!dataReader.HasRows) return null;
        LoginUser user = new LoginUser();
        user.Id = (int)dataReader["id"];
        user.Email = (string)dataReader["email"];
        user.Identificacion = (string)dataReader["UsuarioIdentificacion"];
        user.Rol = (string)dataReader["rol"];
        user.Password = (string)dataReader["clave"];
        user.Salt = (string)dataReader["salt"];

        return user;
    }
}