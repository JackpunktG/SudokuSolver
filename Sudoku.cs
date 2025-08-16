using System.Diagnostics.Contracts;
using System.Drawing;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Sudoku;

public class SudokuPuzzle
{
    string name;
    public int[] Sudoku { get; set; }
    int?[,] sudokuPossibilities = new int?[81, 9];
    bool sudokuSolved = false;
    int loopCounter = 0;

    public SudokuPuzzle(string name, int[] puzzle)
    {
        this.name = name;
        Sudoku = puzzle;
        SudokuSetter();
    }

    public void Solve()
    {
        
        int noChange = 0;
        Console.WriteLine("\n" + name + "Solving...\n");

        while (!sudokuSolved)
        {
            loopCounter++;
            int?[,] startchecker = new int?[81, 9];     //saving start of loop state
            for (int i = 0; i < 81; i++)
                for (int k = 0; k < 9; k++)
                    startchecker[i, k] = sudokuPossibilities[i, k];

            RemovePossibilitesCheck();
            SquareChecker();
            PossibilityCheck();
            IsSolved();
            bool change = StateChecker(startchecker);

            if (!change)
            {
                CheckingPair();
                noChange++;
            }
            if (change) noChange = 0; //resting flag on change 
            

            if (noChange > 1)
            {
                Console.WriteLine($"Could not find Solution... :(\n{loopCounter} loops deep before getting stumped");
                return;
            }

        }
        Console.WriteLine($"Found solution in {loopCounter} loops");
    }

    public bool StateChecker(int?[,] start)  //checking the state of current puzzle solution to a saved state
    {
        for (int i = 0; i < 81; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                if (start[i, k] != sudokuPossibilities[i, k]) return true;
            }
        }
        return false;
    }

    public void SudokuSetter()
    {
        for (int i = 0; i < 81; i++)
        {
            for (int k = 0; k < 9; k++)
            {
                sudokuPossibilities[i, k] = k + 1;
            }
        }

        for (int i = 0; i < 81; i++)
        {
            if (Sudoku[i] != 0)
            {
                SquareNullified(i);
            }
        }
    }

    public void CheckingPair()      //finding pairs logic
    {
        for (int i = 0; i < 9; i++) RowPair(i);
        for (int i = 0; i < 9; i++) ColumnPair(i);
    }

    public void SolveFromPair()     //new function for solving from a guess pair
    {
        int noChange = 0;

        while (!sudokuSolved)
        {
            loopCounter++;
            int?[,] startchecker = new int?[81, 9];     //saving the state
            for (int i = 0; i < 81; i++)
                for (int k = 0; k < 9; k++)
                    startchecker[i, k] = sudokuPossibilities[i, k];

            RemovePossibilitesCheck();
            SquareChecker();
            PossibilityCheck();
            IsSolved();
            bool change = StateChecker(startchecker);

            if (!change)
            {
                CheckingPair();     // will keep branching until checking both pairs don't lead to a new state.
                noChange++;
            }
            if (change) noChange = 0;      //reset flag if new state is detected

            if (noChange > 1) return;       //spots the check if no new state is found
            

        }
        
    }


    public void GuessPair(int square, int option1, int option2)     //testing both guesses of a unknown pair
    {
        int?[,] possibilitiesState = new int?[81, 9];       //saving the current states
        for (int i = 0; i < 81; i++)
            for (int k = 0; k < 9; k++)
                possibilitiesState[i, k] = sudokuPossibilities[i, k];

        int[] sudokuState = new int[81];
        for (int i = 0; i < 81; i++) sudokuState[i] = Sudoku[i];

        Sudoku[square] = option1;       //tesing conditions for option1 guess
        SquareNullified(square);
        RemovePossibilites(square, option1);
        SolveFromPair();                

        IsSolved();
        if (!sudokuSolved)
        {
            //reseting the current states
            for (int i = 0; i < 81; i++)
                for (int k = 0; k < 9; k++)
                    sudokuPossibilities[i, k] = possibilitiesState[i, k];

            for (int i = 0; i < 81; i++) Sudoku[i] = sudokuState[i];

            Sudoku[square] = option2;       //testing the conditon for option2 guess if option 1 failed to solve
            SquareNullified(square);
            RemovePossibilites(square, option2);
            SolveFromPair();

            IsSolved();
            if (!sudokuSolved)      //resetting back if not solved by either guess - no change flag will come up and stop the test
            {
                //reseting the current states
                for (int i = 0; i < 81; i++)
                    for (int k = 0; k < 9; k++)
                    sudokuPossibilities[i, k] = possibilitiesState[i, k];

                for (int i = 0; i < 81; i++) Sudoku[i] = sudokuState[i];
            }
        }
    }


    public void ColumnPair(int column)      //finding unsolved pairs of numbers in column
    {
        int square = column;
        int pair = 0;

        for (int i = 0; i < 9; i++)
        {
            if (Sudoku[square] == 0) pair++;
            square += 9;
        }
        if (pair == 2)
        {
            square = column;
            for (int i = 0; i < 9; i++)
            {
                if (Sudoku[square] == 0)
                {
                    int option1 = 0;
                    int option2 = 0;
                    for (int k = 0; k < 9; k++)
                    {
                        if (sudokuPossibilities[square, k] != null)
                        {
                            if (option1 == 0) option1 = (int)sudokuPossibilities[square, k];
                            else if (option1 != 0) option2 = (int)sudokuPossibilities[square, k];
                            else return;
                        }
                    }
                    if (option1 != 0) GuessPair(square, option1, option2);
                }
                square += 9;
            }
        }
    }

    public void RowPair(int row)    //finding unsolved pairs of numbers in Row
    {
        int square = row * 9;
        int pair = 0;

        for (int i = 0; i < 9; i++)
        {
            if (Sudoku[square] == 0) pair++;
            square++;
        }
        if (pair == 2)
        {
            square = row * 9;
            for (int i = 0; i < 9; i++)
            {
                if (Sudoku[square] == 0)
                {
                    int option1 = 0;
                    int option2 = 0;
                    for (int k = 0; k < 9; k++)
                    {
                        if (sudokuPossibilities[square, k] != null)
                        {
                            if (option1 == 0) option1 = (int)sudokuPossibilities[square, k];
                            else if (option1 != 0) option2 = (int)sudokuPossibilities[square, k];
                            else return;
                        }
                    }
                    if (option1 != 0) GuessPair(square, option1, option2);
                }
                square++;
            }
        }
    }

    public void IsSolved()
    {
        for (int i = 0; i < 81; i++)
        {
            if (Sudoku[i] == 0) return;
        }
        sudokuSolved = true;
    }

    public void PossibilityCheck()      //logic for feeding the singular possibility checks
    {
        for (int i = 0; i < 9; i++) RowCheck(i);
        for (int i = 0; i < 9; i++) ColumnCheck(i);
        for (int i = 0; i < 9; i++) QuadrantCheck(i);
    }

    public void QuadrantCheck(int quadrant)      //Checking quadrant for singluar possibility
    {
        var grid = new Grid();

        for (int square = 0; square < 81; square++)
        {
            if (SquareInfo(square, "quadrant") == quadrant)
            {
                int[] squares = new int[9];
                for (int m = 0; m < 9; m++)
                {
                    squares[m] = sudokuPossibilities[square, m] ?? 0;
                }
                grid.SquareCheck(squares);
            }
        }

        int number = grid.SquareFound();
        if (number != -1)
        {
            for (int square = 0; square < 81; square++)
            {
                if (SquareInfo(square, "quadrant") == quadrant)
                {
                    for (int m = 0; m < 9; m++)
                    {
                        if (sudokuPossibilities[square, m] == number)
                        {
                            Sudoku[square] = number;
                            SquareNullified(square);
                            RemovePossibilites(square, number);
                            return;
                        }
                    }
                }
            }
        }

    }

    public void ColumnCheck(int column)  //Checking column for singluar possibility
    {
        int square = column;
        var grid = new Grid();

        for (int k = 0; k < 9; k++)
        {
            int[] squares = new int[9];
            for (int m = 0; m < 9; m++)
            {
                squares[m] = sudokuPossibilities[square, m] ?? 0;
            }
            grid.SquareCheck(squares);
            square += 9;
        }

        int number = grid.SquareFound();
        if (number != -1)
        {
            square = column;
            for (int k = 0; k < 9; k++)
            {
                for (int m = 0; m < 9; m++)
                {
                    if (sudokuPossibilities[square, m] == number)
                    {
                        Sudoku[square] = number;
                        SquareNullified(square);
                        RemovePossibilites(square, number);
                        return;
                    }
                }
                square += 9;
            }
        }
    }

    public void RowCheck(int row)       //Checking row for singluar possibility
    {
        int square = row * 9;
        var grid = new Grid();

        for (int k = 0; k < 9; k++)
        {
            int[] squares = new int[9];
            for (int m = 0; m < 9; m++)
            {
                squares[m] = sudokuPossibilities[square, m] ?? 0;
            }
            grid.SquareCheck(squares);
            square++;
        }

        int number = grid.SquareFound();
        if (number != -1)
        {
            square = row * 9;
            for (int k = 0; k < 9; k++)
            {
                for (int m = 0; m < 9; m++)
                {
                    if (sudokuPossibilities[square, m] == number)
                    {
                        Sudoku[square] = number;
                        SquareNullified(square);
                        RemovePossibilites(square, number);
                        return;
                    }
                }
                square++;
            }
        }
    }

    public void RemovePossibilitesCheck()    //Iterating through the square to find know squares
    {
        for (int i = 0; i < 81; i++)
        {
            if (Sudoku[i] != 0) RemovePossibilites(i, Sudoku[i]);
        }
    }

    public void RemovePossibilites(int square, int number)      //Logic for removing possibilites of a know square 
    {
        RowRemover(square, number);
        ColumnRemover(square, number);
        QuadrantRemover(square, number);
    }

    public void SquareChecker()             //Iterating through the squares for SquareCheck() 
    {
        for (int i = 0; i < 81; i++) SquareCheck(i);

    }

    public void SquareCheck(int square)     //Checking given square for possibilities, setting if only 1
    {
        int check = 0;
        int number = 0;

        for (int i = 0; i < 9; i++)
        {
            if (sudokuPossibilities[square, i] != null)
            {
                check += 1;
                number = sudokuPossibilities[square, i].Value;
            }
        }

        if (check == 1 && Sudoku[square] == 0)          //setting the square
        {
            Sudoku[square] = number;
            SquareNullified(square);            
            RemovePossibilites(square, number);  
        }
    }

    public void SquareNullified(int square)     //after found square filling its represented square in suokuPossibilies with nulls
    {
        for (int i = 0; i < 9; i++)
        {
            sudokuPossibilities[square, i] = null;
        }
    }

    public void QuadrantRemover(int square, int number)     //removing given possibility in a given quadrant
    {
        int quadrant = SquareInfo(square, "quadrant");

        for (int i = 0; i < 81; i++)
        {
            if (SquareInfo(i, "quadrant") == quadrant)          //iteration through all squaures to find the squares of the given quadrant
            {
                for (int k = 0; k < 9; k++)
                {
                    if (sudokuPossibilities[i, k] == number) sudokuPossibilities[i, k] = null;
                }
            }

        }
    }

    public void ColumnRemover(int square, int number)       //removing given possibility in a given Column
    {
        int column = SquareInfo(square, "column");

        while (column < 81)
        {
            for (int i = 0; i < 9; i++)
            {
                if (sudokuPossibilities[column, i] == number) sudokuPossibilities[column, i] = null;
            }
            column += 9;                    //jumping up the column
        }
    }

    public void RowRemover(int square, int number)          //removing given possibility in a given Row
    {
        int row = SquareInfo(square, "row") * 9; // *9 to get the starting square
        int endRow = row + 9;

        for (; row < endRow; row++)
        {
            for (int i = 0; i < 9; i++)
            {
                if (sudokuPossibilities[row, i] == number) sudokuPossibilities[row, i] = null;
            }
        }
    }

    public int SquareInfo(int square, string info)      // returns the given row, column or quadrant for a given square
    {
        if (info == "row") return square / 9;

        if (info == "column")
        {
            int column = square;

            while (column > 8)
            {
                column -= 9;
            }
            return column;
        }

        if (info == "quadrant")
        {
            int quadrant = ((SquareInfo(square, "row") / 3) * 3 + (SquareInfo(square, "column") / 3) + 1);
            return quadrant;
        }
        else return -1;
    }

    

    public void PrintSudoku()
    {
        for (int i = 0; i < 81; i++)
        {
            Console.Write(Sudoku[i] == 0 ? ". " : Sudoku[i] + " ");
            if ((i + 1) % 9 == 0)
                Console.WriteLine();
        }
    }

    public struct Grid         // Struct to asses Possibilities in a 9 grid
    {
        int one;
        int two;
        int three;
        int four;
        int five;
        int six;
        int seven;
        int eight;
        int nine;

        public Grid()
        {
        }

        public void SquareCheck(int[] square)
        {
            for (int i = 0; i < 9; i++)
            {
                switch (square[i])
                {
                    case 1: one++; break;
                    case 2: two++; break;
                    case 3: three++; break;
                    case 4: four++; break;
                    case 5: five++; break;
                    case 6: six++; break;
                    case 7: seven++; break;
                    case 8: eight++; break;
                    case 9: nine++; break;
                }
            }
        }

        public int SquareFound()
        {
            if (one == 1) return 1;
            if (two == 1) return 2;
            if (three == 1) return 3;
            if (four == 1) return 4;
            if (five == 1) return 5;
            if (six == 1) return 6;
            if (seven == 1) return 7;
            if (eight == 1) return 8;
            if (nine == 1) return 9;

            return -1;
        }

    }
}