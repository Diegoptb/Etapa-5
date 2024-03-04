using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BankAPI.Services; // Para AccountService
using BankAPI.Data; // Si necesitas acceder directamente a entidades de DbContext
using BankAPI.Data.DTOs; // Para los DTOs que estés utilizando



[Route("api/[controller]")]
[ApiController]
public class BankTransactionController : ControllerBase
{
    private readonly AccountService _accountService;

    public BankTransactionController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("PerformTransaction")]
    public async Task<IActionResult> PerformTransaction([FromBody] TransactionDto transactionDto)
    {
        var result = await _accountService.PerformTransaction(transactionDto);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }
}
