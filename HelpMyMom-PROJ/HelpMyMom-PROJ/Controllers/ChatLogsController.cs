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
    public class ChatLogsController : ControllerBase
    {
        private readonly SoftwareBirbContext _context;

        public ChatLogsController(SoftwareBirbContext context)
        {
            _context = context;
        }

        // GET: api/ChatLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatLog>>> GetChatLogs()
        {
            return await _context.ChatLogs.ToListAsync();
        }

        // GET: api/ChatLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatLog>> GetChatLog(long id)
        {
            var chatLog = await _context.ChatLogs.FindAsync(id);

            if (chatLog == null)
            {
                return NotFound();
            }

            return chatLog;
        }

        // PUT: api/ChatLogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatLog(long id, ChatLog chatLog)
        {
            if (id != chatLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(chatLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatLogExists(id))
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

        // POST: api/ChatLogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChatLog>> PostChatLog(ChatLog chatLog)
        {
            _context.ChatLogs.Add(chatLog);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ChatLogExists(chatLog.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetChatLog", new { id = chatLog.Id }, chatLog);
        }

        // DELETE: api/ChatLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatLog(long id)
        {
            var chatLog = await _context.ChatLogs.FindAsync(id);
            if (chatLog == null)
            {
                return NotFound();
            }

            _context.ChatLogs.Remove(chatLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatLogExists(long id)
        {
            return _context.ChatLogs.Any(e => e.Id == id);
        }
    }
}
