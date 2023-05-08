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

namespace MovieLibrary
{
    public class MainService : IMainService
    {
        private MovieContext _dbContext;
        private readonly ILogger<MainService> _logger;
        private readonly IDbContextFactory<MovieContext> _dbContextFactory;
        public MainService(ILogger<MainService> logger, IDbContextFactory<MovieContext> dbContextFactory)
        {
            _logger = logger;
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
                choice = MovieMenu();

                if (choice == 1)
                {
                    _logger.LogInformation("Adding new or updating record to database");
                    AddUpdateNewMovie();
                }
                else if (choice == 2)
                {
                    _logger.LogInformation("Editing a record in database");
                    EditMovie();
                }
                else if (choice == 3)
                {
                    _logger.LogInformation("Displaying Movies from database");
                    DisplayMovies();
                }
                else if (choice == 4)
                {
                    _logger.LogInformation("Searching for a movie in database");
                    SearchMovie();
                }
                else if (choice == 5)
                {
                    _logger.LogInformation("Adding a new user to database");
                    NewUser();
                }
                else if (choice == 6)
                {
                    _logger.LogInformation("Rating a movie from database");
                    RateMovie();
                }
                else if (choice == 7)
                {
                    _logger.LogInformation("Listing top rated movies by occupation and rating");
                    TopRated();
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
        public void AddUserNames()
        {
            var users = _dbContext.Users.ToList();
            foreach (var user in users)
            {
                user.FirstName = Faker.Name.First();
                user.LastName = Faker.Name.Last();
                _dbContext.SaveChanges();
            }
            Console.WriteLine("Users have names updated");
        }
        public int MovieMenu()
        {
            int choice = 0;
            Console.WriteLine("----- Movie Menu -----");
            Console.WriteLine("1) Add new Movie");
            Console.WriteLine("2) Edit a Movie");
            Console.WriteLine("3) Display Movies");
            Console.WriteLine("4) Search for a specific Movie");
            Console.WriteLine("5) Add New User");
            Console.WriteLine("6) Add a Movie Rating");
            Console.WriteLine("7) List top rated movie by occupation");
            Console.WriteLine("8) To exit");
            choice = Input.GetIntWithPrompt("Select an option: ", "Please try again");
            do
            {
                if (choice > 8 || choice < 1)
                {
                    Console.WriteLine("Please select a menu option");
                    choice = Input.GetIntWithPrompt("Select an option: ", "Please try again");
                }
            } while (choice > 8 || choice < 1);
            return choice;
        }
        public void SearchMovie()
        {
            Console.WriteLine();
            string movieSearch = Input.GetStringWithPrompt("What Movie would you like to search for? ", "Please try again, entry can't be null: ");
            var movielist = _dbContext.Movies.Include(x => x.MovieGenres).ThenInclude(x => x.Genre).Where(x => x.Title.Contains(movieSearch)).ToList();
            if (movielist.Count() == 0)
            {
                Console.WriteLine($"There are {movielist.Count()} similar references that match your search");
            }
            else if (movielist.Count() == 1)
            {
                Console.WriteLine($"There is {movielist.Count()} similar reference that matches your search");
                Console.WriteLine();
                foreach (var movie in movielist)
                {
                    Console.WriteLine("Moive ID: {0}", movie.Id);
                    Console.WriteLine("Movie Title: {0} ", movie.Title);
                    Console.WriteLine("Genres:");
                    foreach (var genre in movie?.MovieGenres ?? new List<MovieGenre>())
                    {
                        Console.WriteLine($"\t{genre.Genre.Name}");
                    }
                }
            }
            else if
                 (movielist.Count() > 1)
            {
                Console.WriteLine($"There are {movielist.Count()} similar reference that matches your search");
                Console.WriteLine();
                foreach (var movie in movielist)
                {
                    Console.WriteLine("Moive ID: {0}", movie.Id);
                    Console.WriteLine("Movie Title: {0} ", movie.Title);
                    Console.WriteLine("Genres:");
                    foreach (var genre in movie?.MovieGenres ?? new List<MovieGenre>())
                    {
                        Console.WriteLine($"\t{genre.Genre.Name}");
                    }
                }
            }
            Console.WriteLine();
        }
        public void AddUpdateNewMovie()
        {
            string movieSearch = Input.GetStringWithPrompt("Enter the Movie Title to search if a similar reference already exsist: ", "Please try again, Movie Title can't be blank: ");
            var movieadd = _dbContext.Movies.Where(x => x.Title.Contains(movieSearch)).ToList();
            if (movieadd.Count() > 0)
            {
                if (movieadd.Count() == 1)
                {
                    Console.WriteLine($"There is {movieadd.Count()} similar reference that matches the movie you'd like to add:");
                    Console.WriteLine();
                    foreach (var movie in movieadd)
                    {
                        Console.WriteLine("Moive ID: {0}", movie.Id);
                        Console.WriteLine("     Movie Title: {0} ", movie.Title);
                    }
                }
                else if (movieadd.Count() > 1)
                {
                    Console.WriteLine($"There are {movieadd.Count()} similar references that match the movie you'd like to add:");
                    Console.WriteLine();
                    foreach (var movie in movieadd)
                    {
                        Console.WriteLine("Moive ID: {0}", movie.Id);
                        Console.WriteLine("     Movie Title: {0} ", movie.Title);
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Would you like to:");
                Console.WriteLine("1. Update an exsisting reference");
                Console.WriteLine("2. Add a new reference");
                Console.WriteLine("3. Return to the Main Menu");
                int selection = Input.GetIntWithPrompt("Please select a number: ", "Please try again: ");
                do
                {
                    if (selection > 3 || selection < 1)
                    {
                        selection = Input.GetIntWithPrompt("Please select 1, 2, or 3: ", "Please try again");
                    }
                } while (selection > 3 || selection < 1);
                if (selection == 1)
                {
                    bool doneadd = false;
                    do
                    {
                        Console.WriteLine();
                        Console.WriteLine("Which movie would you like to update? ");
                        foreach (var movie in movieadd)
                        {
                            Console.WriteLine("Moive ID: {0}, {1}", movie.Id, movie.Title);
                        }
                        Console.WriteLine();
                        int movieToUpdate = Input.GetIntWithPrompt("Select a movie by the Id that you'd like to update: ", "Please try again: ");
                        if (movieadd.Any(x => x.Id == movieToUpdate))
                        {
                            bool validSelection = false;
                            do
                            {
                                var updateConfirm = _dbContext.Movies.Where(x => x.Id == movieToUpdate).FirstOrDefault();
                                Console.WriteLine($"Is Id: {updateConfirm.Id}, Title: {updateConfirm.Title} the correct movie you'd like to update?");
                                Console.WriteLine("Y to confirm, N to select again, or E to return to the Main Menu: ");
                                string yesNo = Console.ReadLine();
                                yesNo = yesNo.ToUpper();
                                if (yesNo == "Y")
                                {
                                    bool doneDate = false;
                                    string oldTitle = updateConfirm.Title;
                                    string movieTitle = Input.GetStringWithPrompt("Enter the new Movie Title: ", "The Movie Title can not be blank, Please try again: ");
                                    Console.WriteLine("Please enter the release date in YYYY-MM-DD format:");
                                    var releaseDate = Console.ReadLine();
                                    do
                                    {
                                        var didParse = DateTime.TryParse(releaseDate, out var rdate);
                                        if (didParse)
                                        {
                                            doneDate = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine("That date was not in the correct format: ");
                                            Console.WriteLine("Please enter the release date in YYYY-MM-DD format:");
                                            releaseDate = Console.ReadLine();
                                        }
                                    } while (!doneDate);
                                    updateConfirm.Title = movieTitle;
                                    updateConfirm.ReleaseDate = Convert.ToDateTime(releaseDate);
                                    _dbContext.Movies.Update(updateConfirm);
                                    _dbContext.SaveChanges();
                                    Console.WriteLine();
                                    Console.WriteLine($"Id: {updateConfirm.Id},Movie Title: {oldTitle} has been updated to Id: {updateConfirm.Id}, Movie Title: {updateConfirm.Title}");
                                    Console.WriteLine("Returning to the main menu.");
                                    Console.WriteLine();
                                    validSelection = true;
                                    doneadd = true;
                                }
                                else if (yesNo == "N")
                                {
                                    Console.WriteLine("Please select again");
                                    validSelection = true;
                                }
                                else if (yesNo == "E")
                                {
                                    Console.WriteLine("Returning to the Main Menu");
                                    validSelection = true;
                                    doneadd = true;
                                }
                                else
                                {
                                    Console.WriteLine("That is not a vaild selection, Please try again.");
                                }
                            } while (!validSelection);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("That is not a listed movie id, please try again");
                        }
                    } while (!doneadd);
                }
                else if (selection == 2)
                {
                    NewMovie();
                }
                else if (selection == 3)
                {
                    Console.WriteLine("Returning to the main menu: ");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"There are {movieadd.Count()} similar references that match your search");
                NewMovie();
            }
        }
        public void EditMovie()
        {
            Console.WriteLine();
            Console.WriteLine("How would you like to Edit a move?");
            Console.WriteLine("Enter 1 to Update a movie.");
            Console.WriteLine("Enter 2 to Deleate a movie.");
            Console.WriteLine("Enter 3 to return to the Main Menu.");
            int editSelection = Input.GetIntWithPrompt("Please select 1, 2, or 3: ", "That is not a valid selection, Please try again: ");
            do
            {
                if (editSelection > 3 || editSelection < 1)
                {
                    editSelection = Input.GetIntWithPrompt("Please select 1, 2, or 3: ", "Please try again");
                }
            } while (editSelection > 3 || editSelection < 1);
            if (editSelection == 1)
            {
                UpdateMovie();
            }
            else if (editSelection == 2)
            {
                DeleteMovie();
            }
            else if (editSelection == 3)
            {
                Console.WriteLine("Returning to the main menu: ");
                Console.WriteLine();
            }
        }
        public void NewMovie()
        {
            bool doneDate = false;
            string movieTitle = Input.GetStringWithPrompt("Enter the new Movie Title: ", "The Movie Title can not be blank, Please try again: ");
            Console.WriteLine("Please enter the release date in YYYY-MM-DD format:");
            var releaseDate = Console.ReadLine();
            do
            {
                var didParse = DateTime.TryParse(releaseDate, out var rdate);
                if (didParse)
                {
                    doneDate = true;
                }
                else
                {
                    Console.WriteLine("That date was not in the correct format: ");
                    Console.WriteLine("Please enter the release date in YYYY-MM-DD format:");
                    releaseDate = Console.ReadLine();
                }
            } while (!doneDate);
            var movie = new Movie();
            movie.Title = movieTitle;
            movie.ReleaseDate = Convert.ToDateTime(releaseDate);
            Console.WriteLine($"{movie.Title} has been added");
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            Console.WriteLine();
        }
        public void DisplayMovies()
        {
            Console.WriteLine();
            int maxIndex = 0;
            int i = 0;
            int j = 200;
            bool done = false;
            Console.WriteLine("How many movies would you like to see?");
            Console.WriteLine("Enter 1 for all of the Movies.");
            Console.WriteLine("Enter 2 to select a number of random movies.");
            Console.WriteLine("Enter 3 to return to the Main Menu.");
            int displaySelection = Input.GetIntWithPrompt("Please select 1, 2, or 3: ", "That is not a valid selection, Please try again: ");
            do
            {
                if (displaySelection > 3 || displaySelection < 1)
                {
                    displaySelection = Input.GetIntWithPrompt("Please select 1, 2, or 3: ", "Please try again");
                }
            } while (displaySelection > 3 || displaySelection < 1);
            if (displaySelection == 1)
            {
                var allmovies = _dbContext.Movies.Select(x => new { x.Id, x.Title, x.ReleaseDate }).ToList();
                do
                {
                    done = false;
                    maxIndex = (allmovies.Count);
                    var movielist = allmovies.GetRange(i, j);
                    Table table1 = new Table(TableConfiguration.Unicode());
                    table1.From(movielist);
                    Console.Write(table1.ToString());

                    if ((i + j) < (maxIndex - j))
                    {

                        i += j;
                        Console.WriteLine("Press Enter For More Movies: ");
                        Console.ReadLine();
                    }
                    else if ((i + j) > (maxIndex - j))
                    {
                        i += j;
                        j = (maxIndex - i);
                        if (j == 0)
                        {
                            Console.WriteLine("Done listing all of the movies");
                            i = 0;
                            j = 200;
                            done = true;
                        }
                    }
                } while (!done);
            }
            else if (displaySelection == 2)
            {

                int numberOfMovies = Input.GetIntWithPrompt("How many movies would you like to see? ", "Please try again. ");

                var newlist2 = _dbContext.Movies.Select(x => new { x.Id, x.Title, x.ReleaseDate }).OrderBy(x => Guid.NewGuid()).Take(numberOfMovies).ToList();
                var randommovies = _dbContext.Movies.Take(10).
                    OrderBy(x => Guid.NewGuid()).Take(10).ToList();
                Table table = new Table(TableConfiguration.Unicode());

                table.From(newlist2);
                Console.Write(table.ToString());

            }
            else if (displaySelection == 3)
            {
                Console.WriteLine("Returning to the main menu: ");
                Console.WriteLine();
            }
        }
        public void NewUser()
        {
            string firstName = Input.GetStringWithPrompt("Please enter your First Name:", "Entry can not be Blank, Please try again: ");
            string lastName = Input.GetStringWithPrompt("Please enter your Last Name:", "Entry can not be Blank, Please try again: ");
            int age = Input.GetIntWithPrompt("Please enter your age: ", "That is not a number, Please try again: ");
            bool genderCompleate = false;
            string gender = "N";
            int occupationId = 0;
            do
            {
                gender = Input.GetStringWithPrompt("Please enter you gender M/F: ", "Please try again.");
                gender = gender.ToUpper();
                if (gender == "M" || gender == "F")
                {

                    genderCompleate = true;
                }
                else
                {
                    Console.WriteLine("That is not a vaild entry, Please try again.");
                }
            } while (!genderCompleate);
            string zipcode = Input.GetStringWithPrompt("Enter your zipcode: ", "Please try again.");
            var occupation = _dbContext.Occupations.ToList();
            var users = _dbContext.Users.ToList();
            foreach (var job in occupation)
            {
                Console.WriteLine("{0}. {1}", job.Id, job.Name);
            }
            do
            {
                occupationId = Input.GetIntWithPrompt("Please enter an occupation ID:", "Please try again.");
            } while (occupationId <= 0 && occupationId >= 22);
            var user = new User();
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Age = age;
            user.Gender = gender;
            user.ZipCode = zipcode;
            user.Occupation = occupation.Where(o => o.Id == occupationId).FirstOrDefault();
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            Console.WriteLine();
            Console.WriteLine("New User Added");
            Console.WriteLine($"User Id:{user.Id}");
            Console.WriteLine($"\tFirst Name: {user.FirstName}");
            Console.WriteLine($"\tLast Name: {user.LastName}");
            Console.WriteLine($"\tAge: {user.Age}");
            Console.WriteLine($"\tGender: {user.Gender}");
            Console.WriteLine($"\tZipcode: {user.ZipCode}");
            Console.WriteLine($"\tOccupation: {user.Occupation.Name}");
            Console.WriteLine();
            Console.WriteLine($"Welcome to Blockbuster {user.FirstName}! ");
            Console.WriteLine("Would you like to:");
            Console.WriteLine("1. Rate a Movie");
            Console.WriteLine("2. Return to the Main Menu");
            int selection = Input.GetIntWithPrompt("Please select a number: ", "Please try again: ");
            do
            {
                if (selection > 2 || selection < 1)
                {
                    selection = Input.GetIntWithPrompt("Please select 1 or 2: ", "Please try again");
                }
            } while (selection > 2 || selection < 1);
            if (selection == 1)
            {
                string movieSearch = Input.GetStringWithPrompt("Enter the Movie Title to search for to Rate: ", "Please try again, Movie Title can't be blank: ");
                var movieadd = _dbContext.Movies.Where(x => x.Title.Contains(movieSearch)).ToList();
                if (movieadd.Count() > 0)
                {
                    if (movieadd.Count() == 1)
                    {
                        Console.WriteLine($"There is {movieadd.Count()} similar reference that matches the movie you'd like to Rate:");
                        Console.WriteLine();
                        foreach (var movie in movieadd)
                        {
                            Console.WriteLine("Moive ID: {0}", movie.Id);
                            Console.WriteLine("     Movie Title: {0} ", movie.Title);
                        }
                    }
                    else if (movieadd.Count() > 1)
                    {
                        Console.WriteLine($"There are {movieadd.Count()} similar references that match the movie you'd like to Rate:");
                        Console.WriteLine();
                        foreach (var movie in movieadd)
                        {
                            Console.WriteLine("Moive ID: {0}", movie.Id);
                            Console.WriteLine("     Movie Title: {0} ", movie.Title);
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("Would you like to:");
                    Console.WriteLine("1. Rate reference");
                    Console.WriteLine("2. Return to the Main Menu");
                    int rSelection = Input.GetIntWithPrompt("Please select a number: ", "Please try again: ");
                    do
                    {
                        if (rSelection > 2 || rSelection < 1)
                        {
                            rSelection = Input.GetIntWithPrompt("Please select 1 or 2: ", "Please try again");
                        }
                    } while (rSelection > 2 || rSelection < 1);
                    if (rSelection == 1)
                    {
                        bool doneRateing = false;
                        do
                        {
                            Console.WriteLine();
                            Console.WriteLine("Which movie would you like to Rate? ");
                            foreach (var movie in movieadd)
                            {
                                Console.WriteLine("Moive ID: {0}, {1}", movie.Id, movie.Title);
                            }
                            Console.WriteLine();
                            int movieToRate = Input.GetIntWithPrompt("Select a movie by the Id that you'd like to rate: ", "Please try again: ");
                            if (movieadd.Any(x => x.Id == movieToRate))
                            {
                                bool validSelection = false;
                                do
                                {
                                    var rateConfirm = _dbContext.Movies.Where(x => x.Id == movieToRate).FirstOrDefault();
                                    Console.WriteLine($"Is Id: {rateConfirm.Id}, Title: {rateConfirm.Title} the correct movie you'd like to rate?");
                                    Console.WriteLine("Y to confirm, N to select again, or E to return to the Main Menu: ");
                                    string yesNo = Console.ReadLine();
                                    yesNo = yesNo.ToUpper();
                                    if (yesNo == "Y")
                                    {
                                        int uRating = Input.GetIntWithPrompt($"Please Rate the move: {rateConfirm.Title} on a scale of 1-5: ", "That is not a valid entry, Please try again");
                                        do
                                        {
                                            if (uRating > 5 || uRating < 1)
                                            {
                                                uRating = Input.GetIntWithPrompt("Please select 1-5: ", "Please try again");
                                            }
                                        } while (uRating > 6 || uRating < 1);
                                        var userMovie = new UserMovie()
                                        {
                                            Rating = uRating,
                                            RatedAt = DateTime.Now
                                        };
                                        userMovie.User = user;
                                        userMovie.Movie = rateConfirm;
                                        _dbContext.UserMovies.Add(userMovie);
                                        _dbContext.SaveChanges();
                                        validSelection = true;
                                        doneRateing = true;
                                        Console.WriteLine();
                                        Console.WriteLine($"Thank you {user.FirstName} for giving {rateConfirm.Title} a rating of {userMovie.Rating}");
                                        Console.WriteLine();
                                    }
                                    else if (yesNo == "N")
                                    {
                                        Console.WriteLine("Please select again");
                                        validSelection = true;
                                    }
                                    else if (yesNo == "E")
                                    {
                                        Console.WriteLine("Returning to the Main Menu");
                                        validSelection = true;
                                        doneRateing = true;
                                        Console.WriteLine();
                                    }
                                    else
                                    {
                                        Console.WriteLine("That is not a vaild selection, Please try again.");
                                    }
                                } while (!validSelection);
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("That is not a listed movie id, please try again");
                            }
                        } while (!doneRateing);
                    }
                    else if (rSelection == 2)
                    {
                        Console.WriteLine("Returning to the main menu: ");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine($"There are {movieadd.Count()} similar references that match your search");

                }
            }
        }
        public void RateMovie()
        {
            int userId = 0;
            Console.WriteLine();
            Console.WriteLine("Thank you for Rating Movies in our inventory!");
            Console.WriteLine("Please select one of the options.");
            Console.WriteLine("1.) Enter your User Id.");
            Console.WriteLine("2.) Look up your User Id.");
            Console.WriteLine("3.) Create a new User");
            Console.WriteLine("4.) Return to the Main Menu");
            int uSelect = Input.GetIntWithPrompt("Which option would you like? ", "Please select a number option: ");
            do
            {
                if (uSelect > 4 || uSelect < 1)
                {
                    uSelect = Input.GetIntWithPrompt("Please select a vaild option, 1-4: ", "Please try again");
                }
            } while (uSelect > 4 || uSelect < 1);
            if (uSelect == 1)
            {
                userId = Input.GetIntWithPrompt("Please enter your Id: ", "User Id's are numbers and can't be blank, Please try again: ");
                var user = _dbContext.Users.Where(x => x.Id == userId).FirstOrDefault();
                if (user == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("The User Id you entered did not match any in our system.");
                    Console.WriteLine("Lets try another way to look up your Id");
                    Console.WriteLine();
                    string fName = Input.GetStringWithPrompt("Please Enter your First Name: ", "First Name can not be blank, please try again: ");
                    string lName = Input.GetStringWithPrompt("Please Enter your Last Name: ", "Last Name can not be blank, Please try again: ");
                    string zip = Input.GetStringWithPrompt("Please Enter your Zipcode: ", "Zipcode can not be blank, Please try again: ");
                    user = _dbContext.Users.Where(f => f.FirstName == fName).Where(l => l.LastName == lName).Where(z => z.ZipCode == zip).FirstOrDefault();

                }
                if (user != null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"User Id: {user.Id}, {user.FirstName} {user.LastName} has been found!");
                    Console.WriteLine("Is this the correct information?");
                    Console.WriteLine("1.) for Yes");
                    Console.WriteLine("2.) to return to the Main Menu");
                    int validSelect = Input.GetIntWithPrompt("Which option would you like? ", "Please try again");
                    do
                    {
                        if (validSelect > 2 || validSelect < 1)
                        {
                            validSelect = Input.GetIntWithPrompt("Please select a vaild option, 1-2: ", "Please try again");
                        }
                    } while (validSelect > 2 || validSelect < 1);
                    if (validSelect == 1)
                    {
                        string movieSearch = Input.GetStringWithPrompt("Enter the Movie Title to search for to Rate: ", "Please try again, Movie Title can't be blank: ");
                        var movieadd = _dbContext.Movies.Where(x => x.Title.Contains(movieSearch)).ToList();
                        if (movieadd.Count() > 0)
                        {
                            if (movieadd.Count() == 1)
                            {
                                Console.WriteLine($"There is {movieadd.Count()} similar reference that matches the movie you'd like to Rate:");
                                Console.WriteLine();
                                foreach (var movie in movieadd)
                                {
                                    Console.WriteLine("Moive ID: {0}", movie.Id);
                                    Console.WriteLine("     Movie Title: {0} ", movie.Title);
                                }
                            }
                            else if (movieadd.Count() > 1)
                            {
                                Console.WriteLine($"There are {movieadd.Count()} similar references that match the movie you'd like to Rate:");
                                Console.WriteLine();
                                foreach (var movie in movieadd)
                                {
                                    Console.WriteLine("Moive ID: {0}", movie.Id);
                                    Console.WriteLine("     Movie Title: {0} ", movie.Title);
                                }
                            }
                            Console.WriteLine();
                            Console.WriteLine("Would you like to:");
                            Console.WriteLine("1. Rate reference");
                            Console.WriteLine("2. Return to the Main Menu");
                            int rSelection = Input.GetIntWithPrompt("Please select a number: ", "Please try again: ");
                            do
                            {
                                if (rSelection > 2 || rSelection < 1)
                                {
                                    rSelection = Input.GetIntWithPrompt("Please select 1 or 2: ", "Please try again");
                                }
                            } while (rSelection > 2 || rSelection < 1);
                            if (rSelection == 1)
                            {
                                bool doneRateing = false;
                                do
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("Which movie would you like to Rate? ");
                                    foreach (var movie in movieadd)
                                    {
                                        Console.WriteLine("Moive ID: {0}, {1}", movie.Id, movie.Title);
                                    }
                                    Console.WriteLine();
                                    int movieToRate = Input.GetIntWithPrompt("Select a movie by the Id that you'd like to rate: ", "Please try again: ");
                                    if (movieadd.Any(x => x.Id == movieToRate))
                                    {
                                        bool validSelection = false;
                                        do
                                        {
                                            var rateConfirm = _dbContext.Movies.Where(x => x.Id == movieToRate).FirstOrDefault();
                                            Console.WriteLine($"Is Id: {rateConfirm.Id}, Title: {rateConfirm.Title} the correct movie you'd like to rate?");
                                            Console.WriteLine("Y to confirm, N to select again, or E to return to the Main Menu: ");
                                            string yesNo = Console.ReadLine();
                                            yesNo = yesNo.ToUpper();
                                            if (yesNo == "Y")
                                            {
                                                int uRating = Input.GetIntWithPrompt($"Please Rate the move: {rateConfirm.Title} on a scale of 1-5: ", "That is not a valid entry, Please try again");
                                                do
                                                {
                                                    if (uRating > 5 || uRating < 1)
                                                    {
                                                        uRating = Input.GetIntWithPrompt("Please select 1-5: ", "Please try again");
                                                    }
                                                } while (uRating > 6 || uRating < 1);
                                                var userMovie = new UserMovie()
                                                {
                                                    Rating = uRating,
                                                    RatedAt = DateTime.Now
                                                };
                                                userMovie.User = user;
                                                userMovie.Movie = rateConfirm;
                                                _dbContext.UserMovies.Add(userMovie);
                                                _dbContext.SaveChanges();
                                                validSelection = true;
                                                doneRateing = true;
                                                Console.WriteLine();
                                                Console.WriteLine($"Thank you {user.FirstName} for giving {rateConfirm.Title} a rating of {userMovie.Rating}");
                                                Console.WriteLine();
                                            }
                                            else if (yesNo == "N")
                                            {
                                                Console.WriteLine("Please select again");
                                                validSelection = true;
                                            }
                                            else if (yesNo == "E")
                                            {
                                                Console.WriteLine("Returning to the Main Menu");
                                                validSelection = true;
                                                doneRateing = true;
                                                Console.WriteLine();
                                            }
                                            else
                                            {
                                                Console.WriteLine("That is not a vaild selection, Please try again.");
                                            }
                                        } while (!validSelection);
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine("That is not a listed movie id, please try again");
                                    }
                                } while (!doneRateing);
                            }
                            else if (rSelection == 2)
                            {
                                Console.WriteLine("Returning to the main menu: ");
                                Console.WriteLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"There are {movieadd.Count()} similar references that match your search");

                        }
                    }
                    else if (validSelect == 2)
                    {
                        Console.WriteLine("Returning to the Main Menu");
                        Console.WriteLine();
                    }
                }
                else if (user == null)
                {
                    Console.WriteLine("Im sorry but we could not find any record of your user information.");
                    Console.WriteLine("What would you like to do?");
                    Console.WriteLine("1.) Create a new User");
                    Console.WriteLine("2.) Return to the Main Menu");
                    int rOptions = Input.GetIntWithPrompt("Please choose how you would like to proceed, 1 or 2? ", "Please try again: ");
                    do
                    {
                        if (rOptions > 2 || rOptions < 1)
                        {
                            rOptions = Input.GetIntWithPrompt("Please select a vaild option, 1-2: ", "Please try again");
                        }
                    } while (rOptions > 2 || rOptions < 1);
                    if (rOptions == 1)
                    {
                        NewUser();
                    }
                    else if (rOptions == 2)
                    {
                        Console.WriteLine("Returning to the Main Menu");
                        Console.WriteLine();
                    }
                }
            }
            else if (uSelect == 2)
            {
                LookupUserRating();
            }
            else if (uSelect == 3)
            {
                NewUser();
            }
            else if (uSelect == 4)
            {
                Console.WriteLine("Returning to the main menu: ");
                Console.WriteLine();
            }
        }
        public void LookupUserRating()
        {
            string fName = Input.GetStringWithPrompt("Please Enter your First Name: ", "First Name can not be blank, please try again: ");
            string lName = Input.GetStringWithPrompt("Please Enter your Last Name: ", "Last Name can not be blank, Please try again: ");
            string zip = Input.GetStringWithPrompt("Please Enter your Zipcode: ", "Zipcode can not be blank, Please try again: ");
            var user = _dbContext.Users.Where(f => f.FirstName == fName).Where(l => l.LastName == lName).Where(z => z.ZipCode == zip).FirstOrDefault();
            if (user != null)
            {
                Console.WriteLine();
                Console.WriteLine($"User Id: {user.Id}, {user.FirstName} {user.LastName} has been found!");
                Console.WriteLine("Is this the correct information?");
                Console.WriteLine("1.) for Yes");
                Console.WriteLine("2.) to return to the Main Menu");
                int validSelect = Input.GetIntWithPrompt("Which option would you like? ", "Please try again");
                do
                {
                    if (validSelect > 2 || validSelect < 1)
                    {
                        validSelect = Input.GetIntWithPrompt("Please select a vaild option, 1-2: ", "Please try again");
                    }
                } while (validSelect > 2 || validSelect < 1);
                if (validSelect == 1)
                {
                    string movieSearch = Input.GetStringWithPrompt("Enter the Movie Title to search for to Rate: ", "Please try again, Movie Title can't be blank: ");
                    var movieadd = _dbContext.Movies.Where(x => x.Title.Contains(movieSearch)).ToList();
                    if (movieadd.Count() > 0)
                    {
                        if (movieadd.Count() == 1)
                        {
                            Console.WriteLine($"There is {movieadd.Count()} similar reference that matches the movie you'd like to Rate:");
                            Console.WriteLine();
                            foreach (var movie in movieadd)
                            {
                                Console.WriteLine("Moive ID: {0}", movie.Id);
                                Console.WriteLine("     Movie Title: {0} ", movie.Title);
                            }
                        }
                        else if (movieadd.Count() > 1)
                        {
                            Console.WriteLine($"There are {movieadd.Count()} similar references that match the movie you'd like to Rate:");
                            Console.WriteLine();
                            foreach (var movie in movieadd)
                            {
                                Console.WriteLine("Moive ID: {0}", movie.Id);
                                Console.WriteLine("     Movie Title: {0} ", movie.Title);
                            }
                        }
                        Console.WriteLine();
                        Console.WriteLine("Would you like to:");
                        Console.WriteLine("1. Rate reference");
                        Console.WriteLine("2. Return to the Main Menu");
                        int rSelection = Input.GetIntWithPrompt("Please select a number: ", "Please try again: ");
                        do
                        {
                            if (rSelection > 2 || rSelection < 1)
                            {
                                rSelection = Input.GetIntWithPrompt("Please select 1 or 2: ", "Please try again");
                            }
                        } while (rSelection > 2 || rSelection < 1);
                        if (rSelection == 1)
                        {
                            bool doneRateing = false;
                            do
                            {
                                Console.WriteLine();
                                Console.WriteLine("Which movie would you like to Rate? ");
                                foreach (var movie in movieadd)
                                {
                                    Console.WriteLine("Moive ID: {0}, {1}", movie.Id, movie.Title);
                                }
                                Console.WriteLine();
                                int movieToRate = Input.GetIntWithPrompt("Select a movie by the Id that you'd like to rate: ", "Please try again: ");
                                if (movieadd.Any(x => x.Id == movieToRate))
                                {
                                    bool validSelection = false;
                                    do
                                    {
                                        var rateConfirm = _dbContext.Movies.Where(x => x.Id == movieToRate).FirstOrDefault();
                                        Console.WriteLine($"Is Id: {rateConfirm.Id}, Title: {rateConfirm.Title} the correct movie you'd like to rate?");
                                        Console.WriteLine("Y to confirm, N to select again, or E to return to the Main Menu: ");
                                        string yesNo = Console.ReadLine();
                                        yesNo = yesNo.ToUpper();
                                        if (yesNo == "Y")
                                        {
                                            int uRating = Input.GetIntWithPrompt($"Please Rate the move: {rateConfirm.Title} on a scale of 1-5: ", "That is not a valid entry, Please try again");
                                            do
                                            {
                                                if (uRating > 5 || uRating < 1)
                                                {
                                                    uRating = Input.GetIntWithPrompt("Please select 1-5: ", "Please try again");
                                                }
                                            } while (uRating > 6 || uRating < 1);
                                            var userMovie = new UserMovie()
                                            {
                                                Rating = uRating,
                                                RatedAt = DateTime.Now
                                            };
                                            userMovie.User = user;
                                            userMovie.Movie = rateConfirm;
                                            _dbContext.UserMovies.Add(userMovie);
                                            _dbContext.SaveChanges();
                                            validSelection = true;
                                            doneRateing = true;
                                            Console.WriteLine();
                                            Console.WriteLine($"Thank you {user.FirstName} for giving {rateConfirm.Title} a rating of {userMovie.Rating}");
                                            Console.WriteLine();
                                        }
                                        else if (yesNo == "N")
                                        {
                                            Console.WriteLine("Please select again");
                                            validSelection = true;
                                        }
                                        else if (yesNo == "E")
                                        {
                                            Console.WriteLine("Returning to the Main Menu");
                                            validSelection = true;
                                            doneRateing = true;
                                            Console.WriteLine();
                                        }
                                        else
                                        {
                                            Console.WriteLine("That is not a vaild selection, Please try again.");
                                        }
                                    } while (!validSelection);
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("That is not a listed movie id, please try again");
                                }
                            } while (!doneRateing);
                        }
                        else if (rSelection == 2)
                        {
                            Console.WriteLine("Returning to the main menu: ");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"There are {movieadd.Count()} similar references that match your search");

                    }
                }
                else if (validSelect == 2)
                {
                    Console.WriteLine("Returning to the Main Menu");
                    Console.WriteLine();
                }
            }
            else if (user == null)
            {
                Console.WriteLine("Im sorry but we could not find any record of your user information.");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1.) Create a new User");
                Console.WriteLine("2.) Return to the Main Menu");
                int rOptions = Input.GetIntWithPrompt("Please choose how you would like to proceed, 1 or 2? ", "Please try again: ");
                do
                {
                    if (rOptions > 2 || rOptions < 1)
                    {
                        rOptions = Input.GetIntWithPrompt("Please select a vaild option, 1-2: ", "Please try again");
                    }
                } while (rOptions > 2 || rOptions < 1);
                if (rOptions == 1)
                {
                    NewUser();
                }
                else if (rOptions == 2)
                {
                    Console.WriteLine("Returning to the Main Menu");
                    Console.WriteLine();
                }
            }
        }
        public void UpdateMovie()
        {
            string movieSearch = Input.GetStringWithPrompt("Enter a movie title to search for that you'd like to update: ", "Please try again, Movie Title can't be blank: ");
            var movieUpdate = _dbContext.Movies.Where(x => x.Title.Contains(movieSearch)).ToList();
            if (movieUpdate.Count() > 0)
            {
                if (movieUpdate.Count() == 1)
                {
                    Console.WriteLine($"There is {movieUpdate.Count()} similar reference that matches the movie you'd like to update:");
                    Console.WriteLine();
                    foreach (var movie in movieUpdate)
                    {
                        Console.WriteLine("Moive ID: {0}", movie.Id);
                        Console.WriteLine("     Movie Title: {0} ", movie.Title);
                    }
                }
                else if
                     (movieUpdate.Count() > 1)
                {
                    Console.WriteLine($"There are {movieUpdate.Count()} similar references that match the movie you'd like to update:");
                    Console.WriteLine();
                    foreach (var movie in movieUpdate)
                    {
                        Console.WriteLine("Moive ID: {0}", movie.Id);
                        Console.WriteLine("     Movie Title: {0} ", movie.Title);
                    }
                }
                Console.WriteLine("");
                Console.WriteLine("Would you like to:");
                Console.WriteLine("1. Update an exsisting reference.");
                Console.WriteLine("2. Return to the Main Menu.");
                int selection = Input.GetIntWithPrompt("Please select a number: ", "Please try again: ");
                do
                {
                    if (selection > 2 || selection < 1)
                    {
                        selection = Input.GetIntWithPrompt("Please select 1 or 2: ", "Please try again");
                    }
                } while (selection > 2 || selection < 1);
                if (selection == 1)
                {
                    bool doneadd = false;
                    do
                    {
                        Console.WriteLine();
                        Console.WriteLine("Which movie would you like to update? ");
                        foreach (var movie in movieUpdate)
                        {
                            Console.WriteLine("Moive ID: {0}, {1}", movie.Id, movie.Title);

                        }
                        Console.WriteLine();
                        int movieToUpdate = Input.GetIntWithPrompt("Select a movie by the Id that you'd like to update: ", "Please try again: ");
                        if
                        (movieUpdate.Any(x => x.Id == movieToUpdate))
                        {
                            bool validSelection = false;
                            do
                            {
                                var updateConfirm = _dbContext.Movies.Where(x => x.Id == movieToUpdate).FirstOrDefault();
                                Console.WriteLine($"Is Id: {updateConfirm?.Id}, Title: {updateConfirm?.Title} the correct movie you'd like to update?");
                                Console.WriteLine("Y to confirm or N to return to the Main Menu:");
                                string yesNo = Console.ReadLine();
                                yesNo = yesNo.ToUpper();
                                if (yesNo == "Y")
                                {
                                    bool doneDate = false;
                                    string oldTitle = updateConfirm.Title;
                                    string movieTitle = Input.GetStringWithPrompt("Enter the new Movie Title: ", "The Movie Title can not be blank, Please try again: ");
                                    Console.WriteLine("Please enter the release date in YYYY-MM-DD format:");
                                    var releaseDate = Console.ReadLine();
                                    do
                                    {
                                        var didParse = DateTime.TryParse(releaseDate, out var rdate);
                                        if (didParse)
                                        {
                                            doneDate = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine("That date was not in the correct format: ");
                                            Console.WriteLine("Please enter the release date in YYYY-MM-DD format:");
                                            releaseDate = Console.ReadLine();
                                        }
                                    } while (!doneDate);
                                    updateConfirm.Title = movieTitle;
                                    updateConfirm.ReleaseDate = Convert.ToDateTime(releaseDate);
                                    _dbContext.Update(updateConfirm);
                                    _dbContext.SaveChanges();
                                    Console.WriteLine();
                                    Console.WriteLine($"Id: {updateConfirm.Id},Movie Title: {oldTitle} has been updated to Id: {updateConfirm.Id}, Movie Title: {updateConfirm.Title}");
                                    Console.WriteLine("Returning to the main menu.");
                                    Console.WriteLine();
                                    validSelection = true;
                                    doneadd = true;
                                }
                                else if (yesNo == "N")
                                {
                                    Console.WriteLine("Returning to the Main Menu");
                                    Console.WriteLine();
                                    validSelection = true;
                                    doneadd = true;
                                }
                                else
                                {
                                    Console.WriteLine("Selection not valid, Please try again.");
                                }
                            } while (!validSelection);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("That is not a listed movie id, please try again");
                        }
                    } while (!doneadd);
                }
                else if (selection == 2)
                {
                    Console.WriteLine("Returning to the main menu: ");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"There are {movieUpdate.Count()} similar references that match your search");
                Console.WriteLine();
            }
        }
        public void DeleteMovie()
        {
            string movieSearch = Input.GetStringWithPrompt("Enter a movie title to search for that you'd like to Delete: ", "Please try again, Movie Title can't be blank: ");
            var movieDelete = _dbContext.Movies.Where(x => x.Title.Contains(movieSearch)).ToList();
            if (movieDelete.Count() > 0)
            {
                if (movieDelete.Count() == 1)
                {
                    Console.WriteLine($"There is {movieDelete.Count()} similar reference that matches the movie you'd like to delete:");
                    Console.WriteLine();
                    foreach (var movie in movieDelete)
                    {
                        Console.WriteLine("Moive ID: {0}", movie.Id);
                        Console.WriteLine("     Movie Title: {0} ", movie.Title);
                    }
                }
                else if
                     (movieDelete.Count() > 1)
                {
                    Console.WriteLine($"There are {movieDelete.Count()} similar references that match the movie you'd like to delete:");
                    Console.WriteLine();
                    foreach (var movie in movieDelete)
                    {
                        Console.WriteLine("Moive ID: {0}", movie.Id);
                        Console.WriteLine("     Movie Title: {0} ", movie.Title);
                    }
                }
                Console.WriteLine("");
                Console.WriteLine("Would you like to:");
                Console.WriteLine("1. Delete an exsisting reference.");
                Console.WriteLine("2. Return to the Main Menu.");
                int selection = Input.GetIntWithPrompt("Please select a number: ", "Please try again: ");
                do
                {
                    if (selection > 2 || selection < 1)
                    {
                        selection = Input.GetIntWithPrompt("Please select 1 or 2: ", "Please try again");
                    }
                } while (selection > 2 || selection < 1);
                if (selection == 1)
                {
                    bool doneDelete = false;
                    do
                    {
                        Console.WriteLine();
                        Console.WriteLine("Which movie would you like to Delete? ");
                        foreach (var movie in movieDelete)
                        {
                            Console.WriteLine("Moive ID: {0}, {1}", movie.Id, movie.Title);

                        }
                        Console.WriteLine();
                        int movieToDelete = Input.GetIntWithPrompt("Select a movie by the Id that you'd like to Delete: ", "Please try again: ");
                        if
                        (movieDelete.Any(x => x.Id == movieToDelete))
                        {
                            bool validSelection = false;
                            do
                            {
                                var deleteConfirm = _dbContext.Movies.Where(x => x.Id == movieToDelete).FirstOrDefault();
                                Console.WriteLine($"Is Id: {deleteConfirm.Id}, Title: {deleteConfirm.Title}, the correct movie you'd like to Delete?");
                                Console.WriteLine("Y to confrim or N to return to Main Menu:");
                                string yesNo = Console.ReadLine();
                                yesNo = yesNo.ToUpper();
                                if (yesNo == "Y")
                                {
                                    bool confirmValidation = false;
                                    validSelection = true;
                                    do
                                    {
                                        Console.WriteLine("Are you sure you want to Delete this Reference?");
                                        Console.WriteLine("Y to confirm or N to return to the Main Menu");
                                        yesNo = Console.ReadLine();
                                        yesNo = yesNo.ToUpper();
                                        if (yesNo == "Y")
                                        {
                                            _dbContext.Remove(deleteConfirm);
                                            _dbContext.SaveChanges();
                                            Console.WriteLine();
                                            Console.WriteLine($"Id: {deleteConfirm.Id},Movie Title: {deleteConfirm.Title} has been Deleted");
                                            Console.WriteLine("Returning to the main menu.");
                                            Console.WriteLine();
                                            confirmValidation = true;

                                            doneDelete = true;
                                        }
                                        else if (yesNo == "N")
                                        {
                                            Console.WriteLine("Returning to the Main Menu");
                                            Console.WriteLine();
                                            confirmValidation = true;

                                            doneDelete = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Selection not valid, Please try again.");
                                        }
                                    } while (!confirmValidation);
                                }
                                else if (yesNo == "N")
                                {
                                    Console.WriteLine("Returning to the Main Menu");
                                    Console.WriteLine();
                                    validSelection = true;
                                    doneDelete = true;
                                }
                                else
                                {
                                    Console.WriteLine("Selection not valid, Please try again.");
                                }
                            } while (!validSelection);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("That is not a listed movie id, please try again");
                        }
                    } while (!doneDelete);
                }
                else if (selection == 2)
                {
                    Console.WriteLine("Returning to the main menu: ");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"There are {movieDelete.Count()} similar references that match your search");
                Console.WriteLine();
            }
        }
        public void TopRated()
        {
            Console.WriteLine();
            Console.WriteLine("   Please wait as we search for Top Rated Movies");
            Console.WriteLine("  There are over 100,000 reviews and 1600 Movies!");
            Console.WriteLine("--------- Top Rated Movie By Occupation ----------");
            var users = _dbContext.Users.ToList();
            var occ = users.GroupBy(x => x.Occupation)
                .Select(x => new { Occupation = x.Key });
            foreach (var o in occ.OrderBy(x=>x.Occupation.Name))
            {
                Console.WriteLine($"Occupation: {o.Occupation.Name}");
                var userMovies = _dbContext.UserMovies.ToList();
                var um = userMovies
                    .Where(x => x.User.Occupation.Name == o.Occupation.Name)
                    .GroupBy(x => x.Movie.Title)
                    .Select(x => new { MovieTitle = x.Key, AvgOfRatings = x.Average(x => x.Rating),RatingCount = x.Count() })
                    .ToList();
                foreach (var m in um.OrderByDescending(x=>x.AvgOfRatings * x.RatingCount).ThenBy(x=>x.MovieTitle).Take(1))
                {
                  
                    Console.WriteLine($"\tMovie: {m.MovieTitle}, Average Rating: {m.AvgOfRatings:N1}, with {m.RatingCount} Ratings ");
                }               
            }
            Console.WriteLine();

         

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