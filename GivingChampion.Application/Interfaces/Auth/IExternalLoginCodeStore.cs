using GivingChampion.Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Auth
{
    public interface IExternalLoginCodeStore
    {
        string Store(TokenResponse token);
        TokenResponse? Take(string code);
    }
}
