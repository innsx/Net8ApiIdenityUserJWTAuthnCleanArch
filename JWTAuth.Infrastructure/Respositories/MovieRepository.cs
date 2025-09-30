using JWTAuth.Application.Interfaces;
using JWTAuth.Domain.Entities;
using JWTAuth.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.Infrastructure.Respositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _movieDbContext;

        public MovieRepository(MovieDbContext movieDbContext)
        {
            _movieDbContext = movieDbContext;
        }

        public async Task<int> CreateMovieAsync(Movie movie)
        {
            var movieAlreadyExisted = await _movieDbContext.Movies.FirstOrDefaultAsync(m => m.Name == movie.Name);

            if (movieAlreadyExisted != null)
            {
                return 1;
            }

            Movie newMovie = new Movie();
            newMovie.Name = movie.Name;
            newMovie.Description = movie.Description;
            newMovie.ActorId = movie.ActorId;
            _movieDbContext.Movies.Add(newMovie);

            await _movieDbContext.SaveChangesAsync();

            return 0;
        }

        public async Task<int> DeleteMovieAsync(int id)
        {
            var movie = await _movieDbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return 1;
            }

            _movieDbContext.Movies.Remove(movie);
            await _movieDbContext.SaveChangesAsync();

            return 0;
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            var movie = await _movieDbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return null!;
            }

            return movie;
        }


        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            return await _movieDbContext.Movies.ToListAsync();
        }

        public async Task<int> UpdateMovieAsync(Movie movie)
        {
            var movieToUpdate = await _movieDbContext.Movies.FirstOrDefaultAsync(x => x.Id == movie.Id);

            if (movieToUpdate == null)
            {
                return 1;
            }

            movieToUpdate.Name = movie.Name;
            movieToUpdate.Description = movie.Description;
            _movieDbContext.Movies.Update(movieToUpdate);

            await _movieDbContext.SaveChangesAsync();

            return 0;

        }
    }
}
