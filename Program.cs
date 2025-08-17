

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku;


//Logic using args and a .cvs Testfile 
string fileName = args[0];

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
    Console.WriteLine($"Execution Time: {sw.Elapsed.TotalSeconds} seconds\n");
}


/*
string test = "Sudoku Medium 3,0,0,0,9,0,0,0,1,5,0,0,1,0,0,0,7,2,9,0,0,0,5,0,0,0,0,0,4,0,0,0,2,0,0,0,0,0,0,8,7,0,9,0,0,0,0,3,0,0,0,0,4,9,7,0,0,3,1,0,6,8,7,0,8,0,0,2,0,3,5,0,1,5,1,6,0,8,0,0,3,0";

string[] p = test.Split(',');
string name = p[0];
int[] puzzle = p[1..].Select(s => int.Parse(s)).ToArray();

 var sudoku = new SudokuPuzzle(name, puzzle);

    Stopwatch sw = Stopwatch.StartNew();
    sudoku.Solve();
    sw.Stop();
    sudoku.PrintSudoku();
    Console.WriteLine($"Execution Time: {sw.Elapsed.TotalSeconds} seconds\n");
*/