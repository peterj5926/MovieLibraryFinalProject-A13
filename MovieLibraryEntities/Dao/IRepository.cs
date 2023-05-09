using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public interface IRepository
    {
        void AddUpdateNewMovie();
        void DisplayMovies();
        void EditMovie();
        IEnumerable<Movie> GetAll();
        int MovieMenu();
        void NewUser();
        void RateMovie();
        IEnumerable<Movie> Search(string searchString);
        void SearchMovie();
        void TopRated();
    }
}
