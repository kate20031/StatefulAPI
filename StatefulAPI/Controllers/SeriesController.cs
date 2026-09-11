using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatefulAPI.Models.EntityFramework;

[Route("api/[controller]")]
[ApiController]
public class SeriesController : ControllerBase
{
    private readonly SeriesDbContext _context;
    public SeriesController(SeriesDbContext context)
    {
        _context = context;
    }

    // GET: api/Serie
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Serie>>> GetSerie()
    {
        return await _context.Series.ToListAsync();
    }

    // GET: api/Serie/5
    [HttpGet("{serieid}")]
    public async Task<ActionResult<Serie>> GetSerie(int serieid)
    {
        var serie = await _context.Series.FindAsync(serieid);

        if (serie == null)
        {
            return NotFound();
        }

        return serie;
    }

    // PUT: api/Serie/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{serieid}")]
    public async Task<IActionResult> PutSerie(int? serieid, Serie serie)
    {
        if (serieid != serie.Serieid)
        {
            return BadRequest();
        }

        _context.Entry(serie).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SerieExists(serieid))
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

    // POST: api/Serie
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Serie>> PostSerie(Serie serie)
    {
        _context.Series.Add(serie);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSerie", new { serieid = serie.Serieid }, serie);
    }

    // DELETE: api/Serie/5
    [HttpDelete("{serieid}")]
    public async Task<IActionResult> DeleteSerie(int? serieid)
    {
        var serie = await _context.Series.FindAsync(serieid);
        if (serie == null)
        {
            return NotFound();
        }

        _context.Series.Remove(serie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SerieExists(int? serieid)
    {
        return _context.Series.Any(e => e.Serieid == serieid);
    }
}
