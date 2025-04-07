using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpMyMom_PROJ.models;

namespace HelpMyMom_PROJ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MothersController : ControllerBase
    {
        private readonly SoftwareBirbContext _context;

        public MothersController(SoftwareBirbContext context)
        {
            _context = context;
        }

        // GET: api/Mothers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mother>>> GetMothers()
        {
            return await _context.Mothers.ToListAsync();
        }

        // GET: api/Mothers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mother>> GetMother(int id)
        {
            var mother = await _context.Mothers.FindAsync(id);

            if (mother == null)
            {
                return NotFound();
            }

            return mother;
        }

        // PUT: api/Mothers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMother(int id, Mother mother)
        {
            if (id != mother.Id)
            {
                return BadRequest();
            }

            _context.Entry(mother).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Mothers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Mother>> PostMother(Mother mother)
        {
            _context.Mothers.Add(mother);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MotherExists(mother.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMother", new { id = mother.Id }, mother);
        }

        // DELETE: api/Mothers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMother(int id)
        {
            var mother = await _context.Mothers.FindAsync(id);
            if (mother == null)
            {
                return NotFound();
            }

            _context.Mothers.Remove(mother);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MotherExists(int id)
        {
            return _context.Mothers.Any(e => e.Id == id);
        }
    }
}
