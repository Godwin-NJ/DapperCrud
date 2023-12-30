using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private IConfiguration _config;

        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllheroes(connection);
            return Ok(heroes);
        }

       

        [HttpGet("heroId")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var heroes = await connection.QueryFirstAsync<SuperHero>($"select * from superheroes Where Id = {heroId}");
            return Ok(heroes);
        }

        [HttpPost]
        public async Task<ActionResult<SuperHero>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into superheroes (name, firstName,lastName, place) " +
                "values(@Name,@FirstName,@LastName,@Place)", hero);
            return Ok(await SelectAllheroes(connection));
        }

        [HttpPut]
        public async Task<ActionResult<SuperHero>> UpdateHero( SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update superheroes set name = @Name, firstName = @FirstName,lastName = @LastName, place = @Place where id = @Id", hero);
            return Ok(await SelectAllheroes(connection));
        }

        [HttpDelete("heroId")]
        public async Task<ActionResult<SuperHero>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync($"delete from superheroes where Id = {heroId}");
            return Ok(await SelectAllheroes(connection));
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllheroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("select * from superheroes");
        }
    }
}
