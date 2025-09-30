using JWTAuth.Domain.Entities;

namespace JWTAuth.Application.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task<int> CreateMovieAsync(Movie movie);
        Task<int> UpdateMovieAsync(Movie movie);
        Task<int> DeleteMovieAsync(int id);

    }
}
