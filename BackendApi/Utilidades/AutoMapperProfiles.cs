using AutoMapper;
using BackendApi.DTOs;
using BackendApi.Entidades;

namespace BackendApi.Utilidades
{
    public class AutoMapperProfiles: Profile

    {
        public AutoMapperProfiles() {
            ConfigurarMapeoEmpleaos();
        }

        public void ConfigurarMapeoEmpleaos()
        {
            CreateMap<EmpleadoCreacionDTO, Empleados>();
            CreateMap<Empleados, EmpleadoDTO>();
        }
    }
}
