using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EHM_API.Services;

namespace EHM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableReservationController : ControllerBase
    {
        private readonly ITableReservationService _tableReservationService;

        public TableReservationController(ITableReservationService tableReservationService)
        {
            _tableReservationService = tableReservationService;
        }

        [HttpDelete("{reservationId}")]
        public async Task<IActionResult> DeleteTableReservation(int reservationId)
        {
            var result = await _tableReservationService.DeleteTableReservationByReservationIdAsync(reservationId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

  
    }
}
