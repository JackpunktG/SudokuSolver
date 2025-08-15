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

    public SudokuPuzzle(string name, int[] puzzle)
    {
        this.name = name;
        Sudoku = puzzle;
        SudokuSetter();
    }

    public void Solve()
    {
        int counter = 0;
        int noChange = 0;
        Console.WriteLine("\n" + name + "Solving...\n");

        while (!sudokuSolved)
        {
            counter++;
            int?[,] startchecker = new int?[81, 9];
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
            

            if (noChange >= 5)
            {
                Console.WriteLine("Could not find Solution... :(");
                return;
            }

        }
        Console.WriteLine($"Found solution in {counter} loops");
    }

    public bool StateChecker(int?[,] start)
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

    public void CheckingPair()
    {
        for (int i = 0; i < 9; i++) RowPair(i);
        for (int i = 0; i < 9; i++) ColumnPair(i);
    }

    public void SolveFromPair()
    {
        int noChange = 0;

        while (!sudokuSolved)
        {
            int?[,] startchecker = new int?[81, 9];
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
            if (noChange > 5) return;
            

        }
        
    }


    public void GuessPair(int square, int option1, int option2)
    {
        int?[,] possibilitiesState = new int?[81, 9];       //saving the current states
        for (int i = 0; i < 81; i++)
            for (int k = 0; k < 9; k++)
                possibilitiesState[i, k] = sudokuPossibilities[i, k];

        int[] sudokuState = new int[81];
        for (int i = 0; i < 81; i++) sudokuState[i] = Sudoku[i];

        Sudoku[square] = option1;
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

            Sudoku[square] = option2;
            SquareNullified(square);
            RemovePossibilites(square, option2);
            SolveFromPair();

            IsSolved();
            if (!sudokuSolved)
            {
                //reseting the current states
                for (int i = 0; i < 81; i++)
                    for (int k = 0; k < 9; k++)
                    sudokuPossibilities[i, k] = possibilitiesState[i, k];

                for (int i = 0; i < 81; i++) Sudoku[i] = sudokuState[i];
            }
        }
    }


    public void ColumnPair(int column)
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

    public void RowPair(int row)
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

    public void PossibilityCheck()
    {
        for (int i = 0; i < 9; i++) RowCheck(i);
        for (int i = 0; i < 9; i++) ColumnCheck(i);
        for (int i = 0; i < 9; i++) QuadrantCheck(i);
    }

    public void QuadrantCheck(int quadrant)
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

    public void ColumnCheck(int column)
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

    public void RowCheck(int row)
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

    public void RemovePossibilitesCheck()
    {
        for (int i = 0; i < 81; i++)
        {
            if (Sudoku[i] != 0) RemovePossibilites(i, Sudoku[i]);
        }
    }

    public void RemovePossibilites(int square, int number)
    {
        RowRemover(square, number);
        ColumnRemover(square, number);
        QuadrantRemover(square, number);
    }

    public void SquareChecker()
    {
        for (int i = 0; i < 81; i++) SquareCheck(i);

    }

    public void SquareCheck(int square)
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
            SquareNullified(square); // Nullify possibilities after setting
            RemovePossibilites(square, number);  //removes the possibilites from the new square
        }
    }

    public void SquareNullified(int square)
    {
        for (int i = 0; i < 9; i++)
        {
            sudokuPossibilities[square, i] = null;
        }
    }

    public void QuadrantRemover(int square, int number)
    {
        int quadrant = SquareInfo(square, "quadrant");

        for (int i = 0; i < 81; i++)
        {
            if (SquareInfo(i, "quadrant") == quadrant)
            {
                for (int k = 0; k < 9; k++)
                {
                    if (sudokuPossibilities[i, k] == number) sudokuPossibilities[i, k] = null;
                }
            }

        }
    }

    public void ColumnRemover(int square, int number)
    {
        int column = SquareInfo(square, "column");

        while (column < 81)
        {
            for (int i = 0; i < 9; i++)
            {
                if (sudokuPossibilities[column, i] == number) sudokuPossibilities[column, i] = null;
            }
            column += 9; //jumping up the column
        }
    }

    public void RowRemover(int square, int number)
    {
        int row = SquareInfo(square, "row") * 9; // *9 to get the starting value
        int endRow = row + 9;

        for (; row < endRow; row++)
        {
            for (int i = 0; i < 9; i++)
            {
                if (sudokuPossibilities[row, i] == number) sudokuPossibilities[row, i] = null;
            }
        }
    }

    public int SquareInfo(int square, string info)
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

    public struct Grid
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