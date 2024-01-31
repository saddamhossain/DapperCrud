namespace DapperCrud.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly SqlConnectionFactory sqlConnectionFactory;
    public CustomersController(SqlConnectionFactory sqlConnectionFactory)
    {
        this.sqlConnectionFactory = sqlConnectionFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        using var connection = sqlConnectionFactory.Create();

        const string sql = "SELECT Id, FirstName, LastName, Email, DateOfBirth FROM Customers";

        var customers = await connection.QueryAsync<Customer>(sql);

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var connection = sqlConnectionFactory.Create();

        const string sql = """
                SELECT Id, FirstName, LastName, Email, DateOfBirth
                FROM Customers
                WHERE Id = @CustomerId
                """;

        var customer = await connection.QuerySingleOrDefaultAsync<Customer>(
            sql,
            new { CustomerId = id });

        return customer is not null ? Ok(customer) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        using var connection = sqlConnectionFactory.Create();

        const string sql = """
                INSERT INTO Customers (FirstName, LastName, Email, DateOfBirth)
                VALUES (@FirstName, @LastName, @Email, @DateOfBirth)
            """
        ;

        await connection.ExecuteAsync(sql, customer);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Customer customer)
    {
        using var connection = sqlConnectionFactory.Create();

        customer.Id = id;

        const string sql = """
                UPDATE Customers
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    DateOfBirth = @DateOfBirth
                WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, customer);

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var connection = sqlConnectionFactory.Create();

        const string sql = "DELETE FROM Customers WHERE Id = @CustomerId";

        await connection.ExecuteAsync(sql, new { CustomerId = id });

        return NoContent();
    }
}
