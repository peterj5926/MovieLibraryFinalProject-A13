using Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;
using BetterConsoleTables;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using MovieLibraryEntities.Dao;

namespace MovieLibrary
{
    public class MainService : IMainService
    {
        private MovieContext _dbContext;
        private readonly ILogger<MainService> _logger;
        private readonly IRepository _repository;
        private readonly IDbContextFactory<MovieContext> _dbContextFactory;
        public MainService(ILogger<MainService> logger, IDbContextFactory<MovieContext> dbContextFactory, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
            _dbContextFactory = dbContextFactory;
            _dbContext = _dbContextFactory.CreateDbContext();
        }

        public void Invoke()
        {
            bool exit = false;
            int choice = 0;
            Console.WriteLine("Welcome to Blockbuster!");
            do
            {
                choice = _repository.MovieMenu();

                if (choice == 1)
                {
                    _logger.LogInformation("Adding new or updating record to database");
                    _repository.AddUpdateNewMovie();
                }
                else if (choice == 2)
                {
                    _logger.LogInformation("Editing a record in database");
                    _repository.EditMovie();
                }
                else if (choice == 3)
                {
                    _logger.LogInformation("Displaying Movies from database");
                    _repository.DisplayMovies();
                }
                else if (choice == 4)
                {
                    _logger.LogInformation("Searching for a movie in database");
                    _repository.SearchMovie();
                }
                else if (choice == 5)
                {
                    _logger.LogInformation("Adding a new user to database");
                    _repository.NewUser();
                }
                else if (choice == 6)
                {
                    _logger.LogInformation("Rating a movie from database");
                    _repository.RateMovie();
                }
                else if (choice == 7)
                {
                    _logger.LogInformation("Listing top rated movies by occupation and rating");
                    _repository.TopRated();
                }
                else if (choice == 8)
                {
                    _logger.LogInformation("Exiting the program");
                    exit = true;
                }
            } while (!exit);
            Console.WriteLine();
            Console.WriteLine("Thank you for visiting Blockbuster, Good Bye!");
        }
        
    }


}

//Code trash can
//var movieadd = db.Movies.Select(x => x.Title).ToList();
//var movieadd = db.Movies.AsEnumerable().Select(x=> x.Title.Split('(')).Where(x=> string.Equals(x,moviet,StringComparison.OrdinalIgnoreCase)).ToList();
//var movieadd = db.Movies.Select(x => x.Title.Split('(')).Select(a => new {mo = });
//var movieadd = db.Movies.Select(x => x.Title.Split('(')).Select(x=> new {movietitle = x[0] }).Where(x=> x.movietitle == moviet);
//var movieadd = db.Movies.AsEnumerable().Select(x => x.Title).Select(x=> x.Split('(')).Select(x => new { movietitle = x[0] }).Where(x => x.movietitle == moviet).ToList();
//var movieadd = db.Movies.FirstOrDefault(x => x.Title.Split('(').Select(x => new { movietitle = x[0] }).Where(x => x.movietitle == moviet);
//var movieadd = db.Movies.FirstOrDefault(x => x.Title == movietitle);

//var didParse = DateTime.TryParse(releaseDate, out var date);
//string releaseDate = Input.GetDateWithPrompt("Please enter a release date in YYYY-MM-DD format, Please try again:");
//string date;
//foreach (var movie in movieadd)
//{
//(movieToUpdate == movie.Id)
//var movie = new Movie();
//_dbContext.Movies.Add(movie);
// }
//_dbContext.Remove(deleteConfirm);
//_dbContext.SaveChanges();
//Console.WriteLine();
//Console.WriteLine($"Id: {deleteConfirm.Id},Movie Title: {deleteConfirm.Title} has been Deleted");
//Console.WriteLine("Returning to the main menu.");
//Console.WriteLine();
//doneadd = true;  //var moviel = _dbContext.Movies.OrderBy(x => Guid.NewGuid()).Take(10).Select(x => new { Id = x.Id, Title = x.Title, ReleaseDate = x.ReleaseDate }).();
// var moviel = _dbContext.Movies.OrderBy(x => Guid.NewGuid()).Take(10).Select(x => new Movie { Id = x.Id, Title = x.Title, ReleaseDate = x.ReleaseDate }).ToList();
//var newList = moviel.Select(i=> new { Ii.Id,i.Title,i.ReleaseDate }).ToList();
// Table table1 = new Table(TableConfiguration.MySqlSimple());
//Table table = new Table(TableConfiguration.Unicode());
//foreach (var movie in randommovies)
//{ 
//    Console.WriteLine($"ID: {movie.Id}, Movie Title:{movie.Title}, Release Date:{movie.ReleaseDate}"); 
//}
//Table table2 = new Table("ID", "Title", "ReleaseDate");
//table2.AddRows($"{randommovies.}");    //foreach( var u in users ) 
//{
//    Console.WriteLine("{0}{1}{2}{3}", u.Age, u.Gender,u.ZipCode,u.Occupation); 
//}
//_dbContext.Dispose();
//var selectedoccupation =  _dbContext.Occupations.Where(o=> o.Id == occupationId).FirstOrDefault();
// Console.WriteLine("What Movie would you like to search for?")   //This would work for allowing a null entry to return all movies
// var movieSearch = Console.ReadLine(); 
// var genresList = _dbContext.MovieGenres.Where(g=> g.Movie.Id == movie.Id).ToList();
//foreach (var g in movie?.MovieGenres ?? new List<MovieGenre>())
//{
//    Console.WriteLine($"\t{}");
//   // Console.Write("                         {0}", g.Movie.MovieGenres);
//}
//foreach (var genres in movielist)
//{
//    Console.WriteLine($"");
//   // Console.WriteLine("                         {0}", genres.Genre.Name);
//    //Console.WriteLine("{0}", genres.Movie.Title);
//}
//var genresList = _dbContext.MovieGenres.Where(g => g.Id == movie.Id).ToList();
//Console.WriteLine("Moive ID: {0}", movie.Id);
//Console.WriteLine("Movie Title: {0} ", movie.Title);
//Console.WriteLine("Genres:");
//foreach (var genres in genresList)
//{
//    Console.WriteLine("        {0}", genres.Genre.Name);
//    Console.WriteLine("{0}", genres.Movie.Title);
//}
//public void TopRated()
//{
//    //var topRated = _dbContext.Users.Include(x => x.Occupation).GroupBy(x=>x.Occupation.Name)
//    //    .Include(x => x.UserMovies).OrderBy(x => x.)
//    //    .ThenInclude(x => x.Movie)
//    //    .GroupBy(x => x.Occupation);

//    // var topRated1 = _dbContext.Users.Include(x => x.Occupation)
//    //     .Include(x => x.UserMovies)
//    //     .ThenInclude(x => x.Movie);
//    //// .OrderBy(x => x.Occupation.Name).GroupBy(x => x.Occupation.Name ).ToList();
//    // var topp = topRated1.Take(10);
//    // var rate = _dbContext.UserMovies.Include(x => x.User).ThenInclude(x => x.Occupation)
//    //     .GroupBy(x=>x.Movie)

//    //     .Select(g=> new {MovieID = g.Key, RateCount = g.Count()})
//    //     .OrderBy(m=>m.);
//    //var selectedUser = _dbContext.Users.Where(x => x.Id == 2);
//    //var users = selectedUser.Include(x => x.UserMovies).ThenInclude(x => x.Movie).ToList();

//    //foreach (var user in users)
//    //{
//    //    System.Console.WriteLine($"Added user: ({user.Id}) {user.Gender} {user.Occupation.Name}");

//    //    foreach (var movie in user.UserMovies.OrderBy(x => x.Rating))
//    //    {
//    //        System.Console.WriteLine($"\t\t\t{movie.Rating} {movie.Movie.Title}");
//    //    }
//    //}
//    var users = _dbContext.Users
//    .Include(x => x.Occupation)
//    .Include(x => x.UserMovies)
//    .ThenInclude(x => x.Movie);

//    var users1 = _dbContext.Users
//    .Include(x => x.Occupation)
//    .Include(x => x.UserMovies.Where(x => x.Rating == 5))
//    .ThenInclude(x => x.Movie);

//    var topMovies = _dbContext.UserMovies.Where(x => x.Rating > 3)
//        .Include(x => x.Movie)
//        .Include(x => x.User)
//        .ThenInclude(x => x.Occupation).ToList();

//    //var topMovies2 = _dbContext.UserMovies.Where(x => x.Rating > 3)

//    //    .Include(x => x.Movie)
//    //    .Include(x => x.User)
//    //    .ThenInclude(x => x.Occupation)
//    //    .GroupBy(x => x.Movie.Title,)
//    //    .Select(x => new { MovieTitle = x.Key, CountOfRatings = x.Count() })
//    //    .ToList();
//    // var top1 = topMovies
//    //.GroupBy(x => new { x.Movie.Title, x.User.Occupation.Name,x})
//    // .Select(x => new { Occ = x.Key.Name, Mov = x.Key.Title, O1 = x.Average(x => x.Rating)})
//    // .OrderByDescending(x => x.Occ).ThenBy(x => x.O1);

//    //  var top1 = topMovies
//    //.GroupBy(x => new { x.Movie.Title, x.User.Occupation.Name, x.Rating })
//    // .Select(x => new { Occ = x.Key.Name, Mov = x.Key.Title, Rate = x.Key.Rating, O1 = x.OrderByDescending(x => x.User.Occupation.Name).ThenBy(x => x.Rating) });

//    //var top1 = topMovies
//    //.GroupBy(x => new { x.User.Occupation.Name })
//    // .Select(x => new { Occ = x.Key.Name, O1 = x.OrderBy(x => x.User.Occupation.Name).ThenBy(x => x.Movie.Title).ThenBy(x => x.Rating) });

//    // var top1 = topMovies
//    //.GroupBy(x => new { x.User.Occupation.Name,x.Movie.Title })
//    // .Select(x => new { Occ = x.Key.Name, O1 = x.OrderByDescending(x => x.Rating) });

//    //  var top1 = topMovies
//    //.GroupBy(x => new { x.User.Occupation.Name, x.Movie.Title, x.Rating })
//    // .Select(x => new { Occ = x.Key.Name, O1 = x.Key.Title, A = x.Average(x => x.Rating) });
//    //.GroupBy(x => x. User.UserMovies.Average(x => x.Rating)) }); ;

//    //foreach (var job in top1)
//    //{


//    //    Console.WriteLine($"Occupation: {job.Occ}");
//    //    foreach (var item in job.Mov)
//    //    {
//    //        Console.WriteLine($"Movie Title: {} {item.Rating} ");
//    //    }
//    //}

//    //foreach (var job in top1.OrderBy(x => x.Occ))
//    //{


//    //    Console.WriteLine($"Occupation: {job.Occ}");

//    //    foreach (var item in job)
//    //    {
//    //        Console.WriteLine($"Movie Title: {ite    item.Key Movie.Title} {item.Rating} ");
//    //    }
//    //}
//    //foreach (var job in top1.OrderBy(x => x.Occ))
//    //{


//    //    Console.WriteLine($"Occupation: {job.Occ}");
//    //    foreach (var item in job.O1.Distinct().Take(3))
//    //    {
//    //        Console.WriteLine($"Movie Title: {item.Key Movie.Title} {item.Rating} ");
//    //    }
//    //}



//    //var top1 = topMovies
//    //.GroupBy(x => new { x.User.Occupation.Name, x.Movie.Title })
//    //    .Select(x => new { Occ = x.Key.Name, Mov = x.Key.Title, O1 = x.OrderByDescending(x => x.User.Occupation.Name).ThenBy(x => x.Rating) });
//    //foreach (var occ in top1)
//    //{
//    //    Console.WriteLine($"Occupation: {occ.Occ}");
//    //    foreach (var item in occ.Occ)
//    //        Console.WriteLine($"Movie:{item}");
//    //}

//    //foreach (var u1 in top1)
//    //{
//    //    foreach(var u2 in u1.Occ)
//    //    {
//    //        Console.WriteLine($"Occupation:{u1.Occ}");
//    //        foreach(var u3 in u1.Occ)
//    //        {
//    //            Console.WriteLine($"Movie{u1.Mov}");
//    //        }

//    //    }
//    //}

//    //foreach (var job in top1)
//    //{
//    //    Console.WriteLine($"Occupation: {job.Occ}");
//    //    foreach (var item in job.O1.Take(2))
//    //    {
//    //        Console.WriteLine($"Movie Title: {item.Movie.Title} Rating of: {item.Rating}");
//    //    }
//    //}




//    //.GroupJoin( user=> user.Occupation.Name);
//    //.Where(x=>x.UserMovies.)
//    // var limitedUsers = users1.Take(10).ToList().Select(x => new { Job = x.Key, OCC = x.Where(x => x.Occupation.Name) }).GroupBy(x => x.Occupation.Name);
//    //var newtop = limitedUsers.GroupBy(o => o.Occupation, o => o.UserMovies.Where(x => x.Movie == x.User.UserMovies && x.Rating == 5)).ToList();
//    // var newTop = limitedUsers.GroupBy(j=> j.Occupation.Name);
//    Console.WriteLine();


//    //var users2 = users1.Where(r=>r.UserMovies)
//    //var limitedUsers = users1.GroupBy(o=>o.Occupation.Name).Select(r=> new {R=r.Key,O = r.Count()});
//    // var limitedUsers = users1.GroupBy(o => o.Occupation.Name).Select(r => new { R = r.Key, O = r.Count() });
//    // var limitedUsers = users1.ToList().Select(x => new { Occ = x.Occupation.Name, Mov = x.UserMovies.Where(r => r.Rating == 5).GroupBy(x => x.Movie).Take(1) });
//    //var limitedUsers = users1.ToList().GroupBy(x=>x.Occupation.Name,(key)=>new {Occupation = key});
//    //var limitedUsers = users1.GroupBy(n=>new {n.Occupation.Name,n.UserMovies.Where(m=>m.Movie.Title)}).)
//    //foreach (var user in limitedUsers)
//    //{
//    //    Console.WriteLine("{0}",user);
//    //}


//    // var limitedUsers = users.Select(x => new { Occ = x.Occupation.Name, Mov = x.UserMovies.Where(r => r.Rating == 5)}).GroupBy(x=>x.Occ);
//    //var groupocc = limitedUsers.GroupBy(x => x.Occ);
//    //foreach (var user in limitedUsers)
//    //{
//    //    Console.WriteLine($"{user}");
//    //}


//    //Console.WriteLine("The users are :");
//    //foreach (var user in limitedUsers.ToList())
//    //{
//    //    Console.WriteLine($"User: {user.Id} {user.Gender} {user.ZipCode}");
//    //    Console.WriteLine($"Occupation: {user.Occupation.Name}");
//    //    Console.WriteLine($"Rated Movies:");
//    //    foreach (var userMovie in user.UserMovies)
//    //    {
//    //        Console.WriteLine(userMovie.Movie.Title);
//    //        Console.WriteLine(userMovie.Rating);
//    //    }
//    //}

//    //Console.WriteLine("The users are :");
//    //foreach(MovieContext user in  newTop.ToList()) 
//    ////foreach (var user in newTop.ToList())
//    //{
//    //    Console.WriteLine("{0}", user.Occupations);
//    //    //Console.WriteLine($"User: {user.Id} {user.Gender} {user.ZipCode}");
//    //    //Console.WriteLine($"Occupation: {user.Occupation.Name}");
//    //    //Console.WriteLine($"Rated Movies:");
//    //    foreach (var userMovie in user.UserMovies.Take(1))
//    //    {
//    //        Console.WriteLine(userMovie.Movie.Title);
//    //        Console.WriteLine(userMovie.Rating);
//    //    }
//    //}
//    //Console.WriteLine("The users are :");
//    //foreach (var user in limitedUsers.GroupBy(j=>j.Occupation.Name).ToList())
//    //{
//    //    Console.WriteLine($"{user.}");
//    //    Console.WriteLine($"User: {user.Id} {user.Gender} {user.ZipCode}");
//    //    Console.WriteLine($"Occupation: {user.Occupation.Name}");
//    //    Console.WriteLine($"Rated Movies:");
//    //    foreach (var userMovie in user.UserMovies)
//    //    {
//    //        Console.WriteLine(userMovie.Movie.Title);
//    //    }
//    //}

//    Console.WriteLine();
//    //var top1 = topRated1.Select(x=> new {Ratet = x.})

//    //var top =_dbContext.Movies.Include(x=>x.UserMovies)
//    //    .ThenInclude(x=>x.User)
//    //    .ThenInclude(x=>x.Occupation).GroupBy(x=>x.)

//    var users3 = _dbContext.Users.ToList();
//    //.Include(x=>x.Occupation)
//    //.Include(x=>x.UserMovies).ThenInclude(x=>x.Movie)
//    //.ToList();

//    var occ = users3.GroupBy(x => x.Occupation)
//        .Select(x => new { Occupation = x.Key });

//    foreach (var o in occ.OrderBy(x => x.Occupation.Name))
//    {
//        Console.WriteLine($"Occupation: {o.Occupation.Name}");

//        var userMovies = _dbContext.UserMovies.ToList();

//        var um = userMovies
//            .Where(x => x.User.Occupation.Name == o.Occupation.Name)
//            .GroupBy(x => x.Movie.Title)
//            .Select(x => new { MovieTitle = x.Key, AvgOfRatings = x.Average(x => x.Rating) })
//            .ToList();

//        // var mostRated = um.OrderByDescending(x => x.AvgOfRatings).Take(10);

//        foreach (var m in um.OrderByDescending(x => x.AvgOfRatings).Take(10))
//        {
//            // removing line 42 above would provide all movies and how many times they have been rated
//            // you could then loop over those movie titles and get the avg rating
//            // using the average rating you could calculate the weighted average based on # of ratings and avg rating
//            Console.WriteLine($"\tMovie: {m.MovieTitle}, Number of Ratings: {m.AvgOfRatings}");
//        }
//        //foreach (var userMovie in userMovies.Where(x => x.User.Occupation.Name == o.Occupation.Name)
//        //    .OrderBy(x=>x.Movie.Title)
//        //    .OrderByDescending(x=>x.Rating)
//        //    .Distinct()
//        //    .Take(3))
//        //{
//        //    Console.WriteLine($"\t\t{userMovie.User.Occupation.Name} | {userMovie.Movie.Title} | {userMovie.Rating}");
//        //}
//    }

//    //var limitedUsers = users.Take(1);

//    //Console.WriteLine("The users are :");
//    //foreach (var user in limitedUsers.ToList())
//    //{
//    //    Console.WriteLine($"User: {user.Id} {user.Gender} {user.ZipCode}");
//    //    Console.WriteLine($"Occupation: {user.Occupation.Name}");
//    //    Console.WriteLine($"Rated Movies:");
//    //    foreach (var userMovie in user.UserMovies.Take(10))
//    //    {
//    //        Console.WriteLine($"\t{userMovie.Movie.Title}\t{userMovie.Rating}");
//    //    }
//    //}

//    //var movies = _repository.GetAll();

//    //var userMovies = movies.Where(x=>x.Id==1).Select(x => x.UserMovies);
//    //foreach (var userMovie in userMovies)
//    //{
//    //    foreach (var user in userMovie)
//    //    {
//    //        Console.WriteLine($"{user.Rating}");
//    //    }
//    //}

//}