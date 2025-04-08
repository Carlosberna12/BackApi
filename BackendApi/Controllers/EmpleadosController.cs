using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendApi.DTOs;
using BackendApi.Entidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/empleados")]
    public class EmpleadosController : ControllerBase
    {
        private readonly ApplicationBDContext context;
        private readonly IMapper mapper;

        public EmpleadosController(ApplicationBDContext context,
            IMapper mapper) 
        {
               this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [OutputCache]
        public async Task<List<EmpleadoDTO>> Get()
        {
            return await context.Empleados.ProjectTo<EmpleadoDTO>(mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id:int}", Name = "ObtenerEmpleadoPorId")]
        [OutputCache]
        public async Task<ActionResult<EmpleadoDTO>> Get(int id)
        {
            var empleado = await context.Empleados
                .ProjectTo<EmpleadoDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (empleado is null) 
            { 
                return NotFound();
            }

            return empleado;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmpleadoCreacionDTO empleadoCreacuionDTO)
        {
            var empleado = mapper.Map<Empleados>(empleadoCreacuionDTO);
            context.Add(empleado);
            await context.SaveChangesAsync();
            return CreatedAtRoute("ObtenerEmpleadoPorId", new { id = empleado.Id }, empleado);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] EmpleadoCreacionDTO empleadoCreacionDTO)
        {
            var empleadoex = await context.Empleados.AnyAsync(context => context.Id == id);
            if (!empleadoex)
            {
                return NotFound();
            }

            var empleado = mapper.Map<Empleados>(empleadoCreacionDTO);
            empleado.Id = id;
            context.Update(empleado);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empleado = await context.Empleados.FirstOrDefaultAsync(e => e.Id == id);

            if (empleado is null)
            {
                return NotFound();
            }

            context.Empleados.Remove(empleado);
            await context.SaveChangesAsync();

            return NoContent();
        }


    }
}
