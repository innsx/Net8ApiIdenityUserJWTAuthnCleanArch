using JWTAuth.Application.Interfaces;
using JWTAuth.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        //DISABLED Authorize attribute because we implemented a GLOBAL Authorization in program.cs
        //[Authorize]
        [HttpGet("GetMovies")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _movieRepository.GetMoviesAsync();
            return Ok(movies);
        }


        //DISABLED Authorize attribute because we implemented a GLOBAL Authorization in program.cs
        //[Authorize]
        [HttpGet("GetMovie/{id}")]
        public async Task<ActionResult<Movie>> GetMovieById(int id)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(id);

            if (movie is null)
            {
                return NotFound();
            }

            return Ok(movie);
        }


        //DISABLED Authorize attribute because we implemented a GLOBAL Authorization in program.cs
        //[Authorize]
        [HttpPut("Update/{id}")]
        public async Task<ActionResult<Movie>> UpdateMovie(Movie movie)
        {
            var isUpdate = await _movieRepository.UpdateMovieAsync(movie);

            if (isUpdate == 0)
            {
                return Ok(movie);
            }

            return BadRequest();
        }


        //DISABLED Authorize attribute because we implemented a GLOBAL Authorization in program.cs
        //[Authorize]
        [HttpPost("CreateMovie")]
        public async Task<ActionResult<Movie>> CreateMovie(Movie movie)
        {
            var isCreated = await _movieRepository.CreateMovieAsync(movie);

            if (isCreated == 0)
            {
                return Ok(movie);
            }

            return BadRequest();


        }
    }
}
