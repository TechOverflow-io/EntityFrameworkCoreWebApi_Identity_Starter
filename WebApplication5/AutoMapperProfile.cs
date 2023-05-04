
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebApplication5.Dtos.User;
using WebApplication5.Model;

namespace WebApplication5
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<User, RegisterDto>();
        }
    }
}
