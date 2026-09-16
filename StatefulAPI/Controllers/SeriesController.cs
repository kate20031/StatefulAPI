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

    /// <summary>
    /// Retrieves all TV series.
    /// </summary>
    /// <returns>A list of all TV series.</returns>
    // GET: api/Serie
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Serie>>> GetSerie()
    {
        return await _context.Series.ToListAsync();
    }

    /// <summary>
    /// Retrieves a TV series by its ID.
    /// </summary>
    /// <param name="serieid">The ID of the series.</param>
    /// <returns>The requested TV series.</returns>
    // GET: api/Serie/5
    [HttpGet("{serieid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Serie>> GetSerie(int serieid)
    {
        var serie = await _context.Series.FindAsync(serieid);

        if (serie == null)
        {
            return NotFound();
        }

        return serie;
    }

    /// <summary>
    /// Updates an existing TV series.
    /// </summary>
    /// <param name="serieid">The ID of the series to update.</param>
    /// <param name="serie">The updated series information.</param>
    /// <returns>No content if the update is successful.</returns>
    // PUT: api/Serie/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{serieid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Creates a new TV series.
    /// </summary>
    /// <param name="serie">The series to create.</param>
    /// <returns>The newly created TV series.</returns>
    // POST: api/Serie
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Serie>> PostSerie(Serie serie)
    {
        _context.Series.Add(serie);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSerie", new { serieid = serie.Serieid }, serie);
    }

    /// <summary>
    /// Deletes a TV series.
    /// </summary>
    /// <param name="serieid">The ID of the series to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    // DELETE: api/Serie/5
    [HttpDelete("{serieid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
