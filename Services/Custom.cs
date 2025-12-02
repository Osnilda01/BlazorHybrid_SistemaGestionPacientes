using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace MauiBlazorHybrid.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        public CustomAuthStateProvider(AuthService authService)
        {
            _authService = authService;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var usuario = _authService.UsuarioActual;
            return Task.FromResult(new AuthenticationState(usuario));
        }

        public void NotifyLogin()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyLogout()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }


}
