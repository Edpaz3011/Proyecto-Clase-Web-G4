using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Cloud.Firestore;
using ProyectoClaseG4.DTOs;
using ProyectoClaseG4.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin.Auth;

namespace ProyectoClaseG4.Services;

public class AuthService
{
    // maneja lo realcionado a registro e inicio de sesion
    private readonly FirebaseService _firebaseService;
    private readonly IConfiguration _configuration;

    public AuthService(FirebaseService firebaseService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        _configuration = configuration;
    }

    public async Task<User> Register(RegisterDto dto)
    {
        // Primero verificamos que no exista un usuario con ese correo
        var collection = _firebaseService.GetCollection("users");
        var existing = await collection
            .WhereEqualTo(fieldPath: "Email", dto.Email)
            .GetSnapshotAsync();

        if (existing.Count > 0)
            throw new Exception("Ya existe un usuario con este correo");
        
        //Nuevo - Agrego tambien el usuario a firebase auth para usar la opcion de recuperacion de correo
        var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
            {
                Email = dto.Email,
                Password =  dto.Password,
                DisplayName = dto.FullName,
            }
        );
        
            
        // Creamos el objeto con la contraseña hasheada

        var user = new User
        {
            Id = firebaseUser.Uid, //Hice un cambio aqui antes era "= Guid.NewGuid().ToString()"
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword (dto.Password),
            Role = UserRole.Guest,
            CreatedAt = DateTime.UtcNow

        };
        
        // Guardamos EN FS usando el Id como nombre del documento 
        await collection.Document(user.Id).SetAsync(new Dictionary<string, object>
        {
            { "Id", user.Id },
            { "FullName", user.FullName },
            { "Email", user.Email },
            { "PasswordHash", user.PasswordHash },
            { "Role", user.Role },
            { "CreatedAt", user.CreatedAt }
        });
        return user;
    }

    public async Task<User> RegisterAdmin(RegisterDto dto)
    {
        var collection = _firebaseService.GetCollection("users");
        var existing = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();
        
        if (existing.Count > 0)
            throw new Exception("Ya existe un usuario con esa credencial");
        
        //Nuevo - Agrego tambien el usuario a firebase auth para usar la opcion de recuperacion de correo
        
        var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
            {
                Email = dto.Email,
                Password =  dto.Password,
                DisplayName = dto.FullName,
            }
        );

        var user = new User
        {
            Id = firebaseUser.Uid, //Hice un cambio aqui antes era "= Guid.NewGuid().ToString()"
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };
        
        await collection.Document(user.Id).SetAsync(new Dictionary<string, object>
            {
                { "Id", user.Id },
                { "FullName", user.FullName },
                { "Email", user.Email },
                { "PasswordHash", user.PasswordHash },
                { "Role", user.Role },
                { "CreatedAt", user.CreatedAt }
            }
        );
        return user;
    }
    
    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
        // Buscar al usuario por correo en FS
        var collection = _firebaseService.GetCollection("users");
        var snapshot = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();
        
        if(snapshot.Count == 0)
            throw new Exception("No existe ningun usuario con esa credencial");
        
        // Si lo encontramos mapeamos manualmente el documento a nuestro objeto
        // Usamos ToDictionary()
        var doc = snapshot.Documents[0];
        var data = doc.ToDictionary();

        var user = new User
        {
            Id = data["Id"].ToString()!,
            FullName = data["FullName"].ToString()!,
            Email = data["Email"].ToString()!,
            PasswordHash = data["PasswordHash"].ToString()!,
            Role = data["Role"].ToString()!,
            // Int64, necesitamos convertirlo
            CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()  
        };
        
        // Verificar si la contraseña esta hasheada
        if(!VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Password incorrecto");
        
        // Se completo exitosamente, generamos un token JWT
        var token = GenerateToken(user);
        return new AuthResponseDto //modificado
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(8)

        };
    }

    // Task para recuperar la contraseña
    public async Task ForgotPassword(ForgotPasswordDto dto)
    {
        // Verificar que el usuario existe en Firestore
        var collection = _firebaseService.GetCollection("users");
        var snapshot = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

      
        if (snapshot.Count == 0)
            throw new Exception("Si el correo está registrado recibirás un email");

        // Firebase genera el link de reset y manda el email automáticamente
        
            var apiKey = _configuration["Firebase:ApiKey"];
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

            var payload = new
            {
                requestType = "PASSWORD_RESET",
                email = dto.Email,
            };
            
            using var httpClient = new HttpClient();
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al enviar el codigo de recuperacion");

    }
    
    
    private string GenerateToken(User user)
    {
        // El Token lleva cierta informacion, Id, Email y Role de usuario que hizo login
        // Para proteccion de los endpoints, se sabe quien los esta llamando 
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
            var token = new JwtSecurityToken(
                    
            issuer: _configuration["Jwt:Issuer"], //Quien lo genera, nuestro token lo genera la app
            audience: _configuration["Jwt:Issuer"], // Para quien lo genera, clientes / front-end
            claims: claims, // Estos son los datos del usuario
            expires: DateTime.UtcNow.AddHours(8), //Tiempo de vida del token
            signingCredentials: creds // Firma de seguridad
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string dtoPassword, string userPasswordHash)
    {
        return HashPassword(dtoPassword) == userPasswordHash;
    }


    // PARA ENCRIPTAR LA CONTRASEÑA
    private string HashPassword(string password)
    {
       //SHA256 - tipo de encriptacion
       var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
       return Convert.ToBase64String(bytes);
    }
}