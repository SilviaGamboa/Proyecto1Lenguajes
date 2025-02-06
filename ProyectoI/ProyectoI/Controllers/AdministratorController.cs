using Microsoft.AspNetCore.Mvc;
using ProyectoI.Repositories;
using System.Security.Cryptography;
using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace ProyectoI.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AdministratorController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;

        }

        // Método para mostrar la vista de gestión de usuarios
        public IActionResult GestionUsuario()
        {
            return View();
        }

        // Crear usuario manualmente
        [HttpPost]
        public async Task<IActionResult> GestionUsuario(string nombre, string correo, string contrasenna)
        {
            bool resultado = await _userRepository.CreateUserAsync(nombre, correo, contrasenna);

            if (resultado)
            {
                ViewBag.Mensaje = "Usuario creado exitosamente.";
            }
            else
            {
                ViewBag.ErrorMessage = "Error al crear usuario.";
            }

            return RedirectToAction("GestionUsuario");
        }
    }
}   
