

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Sudoku;


//string option = args[0].ToLower();
string option = "ui";

switch (option)
{
    case "cvs":
        SudokuFromCVS(args[1]);
        break;
    case "ui":
        SolverInterface();
        break;
}



void SolverInterface()
{
    Console.WriteLine("\n***Welcome to Jack's Sudoku Solver***");
    while (true)
    {
        Console.WriteLine("Options:\n1 - input Sudoku\n5 - exit");
        string input = Console.ReadLine().ToLower();

        switch (input)
        {
            case "1":
                UserInputSudoku();
                break;
            case "5":
                Console.WriteLine("\nB, bye :)");
                return;
            default:
                Console.WriteLine("Invalid Input... try again\n");
                break;
        }
    }
}

void UserInputSudoku()      //Logic for user inputs and testing
{
    bool userSudokuCorret = false;

    int[] puzzle = new int[81];
    do
    {
        Console.WriteLine("\nInput the Sudoku in one line with a comma after each number, with unknowns as 0\nLike: 1, 2, 0, 2, 0... etc");
        string userPuzzle = Console.ReadLine();

        try
        {
            puzzle = userPuzzle.Split(',').Select(s => int.Parse(s)).ToArray();
            SudokuException.ValidateUserPuzzle(puzzle);
            userSudokuCorret = true;
        }
        catch (SudokuException ex)
        {
            Console.WriteLine($"Invaild Sudoku: {ex.Message}");
            userSudokuCorret = false;
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid Sudoku: not all entries are numbers.");
            userSudokuCorret = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            userSudokuCorret = false;
        }
    } while (!userSudokuCorret);

    var sudoku = new SudokuPuzzle("User Generated", puzzle);

    Stopwatch sw = Stopwatch.StartNew();
    sudoku.Solve();
    sw.Stop();
    sudoku.PrintSudoku();
    Console.WriteLine($"Execution Time: {sw.Elapsed.TotalMilliseconds} ms\n"); 
}

void SudokuFromCVS(string fileName) //Logic using args and a .cvs Testfile 
{
    string[] lines = File.ReadAllLines(fileName);

    foreach (var line in lines)
    {
        string[] p = line.Split(',');
        string name = p[0];
        int[] puzzle = p[1..].Select(s => int.Parse(s)).ToArray();

        var sudoku = new SudokuPuzzle(name, puzzle);

        Stopwatch sw = Stopwatch.StartNew();
        sudoku.Solve();
        sw.Stop();
        sudoku.PrintSudoku();
        Console.WriteLine($"Execution Time: {sw.Elapsed.TotalMilliseconds} ms\n");  
    }
}