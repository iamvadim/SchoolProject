// Vadim Minchuk, BIT 143, Winter 2016, A3.0

using System;
using System.Collections.Generic;
using System.Text;

namespace MulitList_Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Vadim's Library");
            (new UserInterface()).RunProgram();
        }
    }

    enum MenuOptions
    {
        QUIT = 1,
        ADD_BOOK,
        PRINT_BY_AUTHOR,
        PRINT_BY_TITLE,
        REMOVE_BOOK,
        RUN_TESTS
    }

    class UserInterface
    {
        MultiLinkedListOfBooks theList;
        public void RunProgram()
        {
            int nChoice;
            theList = new MultiLinkedListOfBooks();

            do // main loop
            {
                Console.WriteLine("Your Options:");
                Console.WriteLine("{0} : End the program", (int)MenuOptions.QUIT);
                Console.WriteLine("{0} : Add a book", (int)MenuOptions.ADD_BOOK);
                Console.WriteLine("{0} : Print all books (by author)", (int)MenuOptions.PRINT_BY_AUTHOR);
                Console.WriteLine("{0} : Print all books (by title)", (int)MenuOptions.PRINT_BY_TITLE);
                Console.WriteLine("{0} : Remove a Book", (int)MenuOptions.REMOVE_BOOK);
                Console.WriteLine("{0} : RUN TESTS", (int)MenuOptions.RUN_TESTS);
                if (!Int32.TryParse(Console.ReadLine(), out nChoice))
                {
                    Console.WriteLine("You need to type in a valid, whole number!");
                    continue;
                }
                switch ((MenuOptions)nChoice)
                {
                    case MenuOptions.QUIT:
                        Console.WriteLine("Thank you for using the multi-list program!");
                        break;
                    case MenuOptions.ADD_BOOK:
                        this.AddBook();
                        break;
                    case MenuOptions.PRINT_BY_AUTHOR:
                        theList.PrintByAuthor();
                        break;
                    case MenuOptions.PRINT_BY_TITLE:
                        theList.PrintByTitle();
                        break;
                    case MenuOptions.REMOVE_BOOK:
                        this.RemoveBook();
                        break;
                    case MenuOptions.RUN_TESTS:
                        AllTests tester = new AllTests();
                        tester.RunTests();
                        break;
                    default:
                        Console.WriteLine("I'm sorry, but that wasn't a valid menu option");
                        break;

                }
            } while (nChoice != (int)MenuOptions.QUIT);
        }

        public void AddBook()
        {
            Console.WriteLine(); //Spacing/aesthetics

            Console.WriteLine("ADD A BOOK!");

            Console.WriteLine("Author name?");
            string author = Console.ReadLine();

            Console.WriteLine("Title?");
            string title = Console.ReadLine();

            double price = -1;
            while (price < 0)
            {
                Console.WriteLine("Price?");
                if (!Double.TryParse(Console.ReadLine(), out price))
                {
                    Console.WriteLine("Sorry, your entry is not a valid price!");
                    price = -1;
                }
                else if (price < 0)
                    Console.WriteLine("Sorry, you must enter a number greater than or equal to zero!");
            }

            ErrorCode ec = theList.Add(author, title, price);

            if(ec == ErrorCode.DuplicateBook)
                Console.WriteLine("This book is already in the library!");
            else
                Console.WriteLine("This book was successfully added to the library.");
        }

        public void RemoveBook()
        {
            Console.WriteLine(); //Spacing/aesthetics

            Console.WriteLine("REMOVE A BOOK!");

            Console.WriteLine("Author name?");
            string author = Console.ReadLine();

            Console.WriteLine("Title?");
            string title = Console.ReadLine();

            ErrorCode ec = theList.Remove(author, title);

            if(ec == ErrorCode.BookNotFound)
                Console.WriteLine("This book is not in the library!");
            else if (ec == ErrorCode.OK)
                Console.WriteLine("This book was successfully removed from the library.");
        }
    }

    enum ErrorCode
    {
        OK,
        DuplicateBook,
        BookNotFound
    }

    class MultiLinkedListOfBooks
    {
        Book authorFront;
        Book titleFront;

        private class Book
        {
            public string author;
            public string title;
            public double price;
            public Book nextAuthor;
            public Book nextTitle;

            public Book(string a, string t, double p)
            {
                author = a;
                title = t;
                price = p;
            }

            public void PrintBook()
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine("Book Title: {0}", title);
                Console.WriteLine("Book Author: {0}", author);
                Console.WriteLine("Price: {0}", price);
            }

            //These methods will compare the strings ignoring case. if the authors are the same, then it will compare the
            //the titles and vice versa.
            public int CompareByAuthor(Book otherBook)
            {
                if (string.Compare(author, otherBook.author) == 0)
                    return string.Compare(title, otherBook.title);

                return string.Compare(author, otherBook.author, true); 
            }

            public int CompareByTitle(Book otherBook)
            {
                if (string.Compare(title, otherBook.title) == 0)
                    return string.Compare(author, otherBook.author);

                return string.Compare(title, otherBook.title, true); 
            }
           
        }

        //I simplified a huge amount of code in this section. Your comment about "factoring" things out and modifying my CompareBy functions
        //definitely helped me do so.
        public ErrorCode Add(string author, string title, double price)
        {
            Book newBook = new Book(author, title, price);

            if(authorFront == null) //Add book to front if an author doesnt exist.
            {
                authorFront = newBook;
                titleFront = newBook;
                return ErrorCode.OK;
            }

            ErrorCode check = AddAuthor(newBook);

            if (check == ErrorCode.DuplicateBook)
                return ErrorCode.DuplicateBook;

            check = AddTitle(newBook);

            if (check == ErrorCode.DuplicateBook)
                return ErrorCode.DuplicateBook;

            return ErrorCode.OK;
        }

        private ErrorCode AddAuthor(Book newBook)
        {

            //First two if-statements compare the first author.
            if (newBook.CompareByAuthor(authorFront) == 0)
                return ErrorCode.DuplicateBook;
            
            if(newBook.CompareByAuthor(authorFront) == -1)
            {
                newBook.nextAuthor = authorFront;
                authorFront = newBook;
                return ErrorCode.OK;
            }

            Book cur = authorFront; //Temp reference variable

            while(cur.nextAuthor != null) //Traverse the author list to figure out where to put our new book.
            {
                if (newBook.CompareByAuthor(cur.nextAuthor) == 0) //Duplicate
                    return ErrorCode.DuplicateBook;

                if (newBook.CompareByAuthor(cur.nextAuthor) == -1) //New book's author precedes cur.nexAuthor
                {
                    newBook.nextAuthor = cur.nextAuthor;
                    cur.nextAuthor = newBook;
                    return ErrorCode.OK;
                }

                cur = cur.nextAuthor; //Advance cur
            }

            cur.nextAuthor = newBook; //If we get here, we've reached the end of our list. Tack on the new book to the end.
            return ErrorCode.OK;
        }

        private ErrorCode AddTitle(Book newBook)
        {

            //First two if-statements compare the first book
            if (newBook.CompareByTitle(titleFront) == 0)
                return ErrorCode.DuplicateBook;

            if (newBook.CompareByTitle(titleFront) == -1)
            {
                newBook.nextTitle = titleFront;
                titleFront = newBook;
                return ErrorCode.OK;
            }

            Book cur = titleFront;

            while (cur.nextTitle != null) 
            {
                if (newBook.CompareByTitle(cur.nextTitle) == 0) //Duplicate
                    return ErrorCode.DuplicateBook;

                if (newBook.CompareByTitle(cur.nextTitle) == -1) //New book's title precedes cur.nexTitle
                {
                    newBook.nextTitle = cur.nextTitle;
                    cur.nextTitle = newBook;
                    return ErrorCode.OK;
                }

                cur = cur.nextTitle; //Advance cur
            }

            cur.nextTitle = newBook; //If we get here, we've reached the end of our list. Tack on the new book to the end.
            return ErrorCode.OK;
        }

        public void PrintByAuthor()
        {
            // if there are no books, then print out a message saying that the list is empty

            if (authorFront == null)
                Console.WriteLine("There are no books in the system.");
            else
            {
                Book cur = authorFront;
                while(cur != null)
                {
                    cur.PrintBook();
                    cur = cur.nextAuthor;
                }
                Console.WriteLine("----------------------------------");
            }
        }

        public void PrintByTitle()
        {
            // if there are no books, then print out a message saying that the list is empty

            if (titleFront == null)
                Console.WriteLine("There are no books in the system.");
            else
            {
                Book cur = titleFront;
                while (cur != null)
                {
                    cur.PrintBook();
                    cur = cur.nextTitle;
                }
                Console.WriteLine("----------------------------------");
            }
        }

        public ErrorCode Remove(string author, string title)
        {
            
            if(authorFront == null)
                return ErrorCode.BookNotFound;

            Book RemoveBook = new Book(author, title, 0); //Temp book for comparisons

            ErrorCode check = RemoveAuthor(RemoveBook);

            if (check == ErrorCode.BookNotFound)
                return ErrorCode.BookNotFound;

            check = RemoveTitle(RemoveBook);

            if (check == ErrorCode.BookNotFound)
                return ErrorCode.BookNotFound;

            return ErrorCode.OK;
        }

        private ErrorCode RemoveAuthor(Book RemoveBook)
        {

            if (RemoveBook.CompareByAuthor(authorFront) == 0) //Checks the first book.
            {
                authorFront = authorFront.nextAuthor;
                return ErrorCode.OK;
            }

            Book cur = authorFront; //Temp reference variable

            while(cur.nextAuthor != null) //traverse list.
            {
                if(RemoveBook.CompareByAuthor(cur.nextAuthor) == 0) //Check cur.Next
                {
                    cur.nextAuthor = cur.nextAuthor.nextAuthor;
                    return ErrorCode.OK;
                }

                cur = cur.nextAuthor; //Advance cur
            }

            return ErrorCode.BookNotFound; //If we get here, we havent found the book.
        }

        private ErrorCode RemoveTitle(Book RemoveBook)
        {

            if (RemoveBook.CompareByTitle(titleFront) == 0) //Checks the first book.
            {
                titleFront = titleFront.nextTitle;
                return ErrorCode.OK;
            }

            Book cur = titleFront; //Temp reference variable

            while(cur.nextTitle != null) //traverse list 
            {
                if(RemoveBook.CompareByTitle(cur.nextTitle) == 0) //Checks cur.Next
                {
                    cur.nextTitle = cur.nextTitle.nextTitle;
                    return ErrorCode.OK;
                }

                cur = cur.nextTitle; //Advance cur
            }

            return ErrorCode.BookNotFound; //If we get here, we havent found the book.
        }
        
    }

    class AllTests
    {
        public void RunTests()
        {
        }  
    }
}