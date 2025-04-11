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
    public class HelpersController : ControllerBase
    {
        private readonly SoftwareBirbContext _context;

        public HelpersController(SoftwareBirbContext context)
        {
            _context = context;
        }

        // GET: api/Helpers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Helper>>> GetHelpers()
        {
            return await _context.Helpers.ToListAsync();
        }

        // GET: api/Helpers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Helper>> GetHelper(int id)
        {
            var helper = await _context.Helpers.FindAsync(id);

            if (helper == null)
            {
                return NotFound();
            }

            return helper;
        }

        // PUT: api/Helpers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHelper(int id, Helper helper)
        {
            if (id != helper.Id)
            {
                return BadRequest();
            }

            _context.Entry(helper).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HelperExists(id))
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

        // POST: api/Helpers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Helper>> PostHelper(Helper helper)
        {
            _context.Helpers.Add(helper);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (HelperExists(helper.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetHelper", new { id = helper.Id }, helper);
        }

        // DELETE: api/Helpers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHelper(int id)
        {
            var helper = await _context.Helpers.FindAsync(id);
            if (helper == null)
            {
                return NotFound();
            }

            _context.Helpers.Remove(helper);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HelperExists(int id)
        {
            return _context.Helpers.Any(e => e.Id == id);
        }
    }
}
