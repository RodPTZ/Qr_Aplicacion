using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Repositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;

namespace SistemaDeBoleteria.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository loginRepository = new LoginRepository();
        public Usuario Register(RegisterRequest registerRequest) => loginRepository.Insert(registerRequest.Adapt<Usuario>());
        public LoginResponse? Login(LoginRequest loginRequest) { }
        public string RefreshToken(string token) { }
        public bool Logout(string token) { }
        public Usuario? Me(string token) { }
        public bool ChangeRol(int idUsuario, string rol) { }
    }
}