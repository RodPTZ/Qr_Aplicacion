using Mapster;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Exceptions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace SistemaDeBoleteria.Services
{
    public class LoginService : ILoginService
    {
        private readonly ITokenRepository tokenRepository;
        private readonly ILoginRepository loginRepository;
        private readonly IConfiguration configuration;
        public LoginService(ITokenRepository tokenRepository, ILoginRepository loginRepository, IConfiguration configuration)
        {
            this.tokenRepository = tokenRepository;
            this.loginRepository = loginRepository;
            this.configuration = configuration;
        }
        public Usuario Register(RegisterRequest registerRequest) => loginRepository.Insert(registerRequest.Adapt<Usuario>());
        public LoginResponse? Login(LoginRequest loginRequest)
        {
            var user = loginRepository.SelectByEmailAndPass(loginRequest.Email, loginRequest.Contraseña);

            if(user is null)
                throw new NotFoundException("No se encontró el usuario especificado.");

            var claims = new[] {
                new Claim(ClaimTypes.Email, loginRequest.Email),
                new Claim(ClaimTypes.Role, user.Rol.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            var refreshToken = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );
            if (string.IsNullOrEmpty(new JwtSecurityTokenHandler().WriteToken(refreshToken)))
                {
                        throw new InvalidOperationException("El refresh token generado es nulo o vacío.");
                }

            tokenRepository.InsertToken(user.IdUsuario, new JwtSecurityTokenHandler().WriteToken(refreshToken), refreshToken.ValidTo);
            
            return new LoginResponse
            {
                Email = user.Email,
                Rol = user.Rol.ToString(),
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken)
            };
        }
        public string RefreshToken(string token)
        {
            var isRevoked = tokenRepository.IsRevoced(token);

            if(isRevoked == default)
                throw new NotFoundException("Token no encontrado");
            if (isRevoked.Revocado)
                throw new BusinessException("Token revocado");
            if (isRevoked.Expiración < DateTime.Now)
                throw new BusinessException("Token expirado");

            var user = tokenRepository.SelectUserByToken(token);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user!.Email),
                new Claim(ClaimTypes.Role, user!.Rol.ToString())
            };

            var key = System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var newAccessToken = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(newAccessToken);
        }
        public bool Logout(string token) => tokenRepository.InvalidateToken(token);
        public ViewMe? Me(string email) => loginRepository.SelectMe(email).Adapt<ViewMe>();
        public bool ChangeRol(int idUsuario, string rol) => loginRepository.UpdateRol(idUsuario, rol);
        
    }
}