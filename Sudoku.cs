using System.Diagnostics.Contracts;
using System.Dynamic;

namespace Sudoku;

public class SudokuPuzzle
{
    public int[] Sudoku { get; set; }
    int?[,] sudokuPossibilities = new int?[81, 9];
    bool sudokuSolved = false;

    public SudokuPuzzle(int[] puzzle)
    {
        Sudoku = puzzle;
        SudokuSetter();
    }

    public void Solve()
    {

        while (sudokuSolved == false)
        {
            RemovePossibilitesCheck();
            SquareChecker();
            IsSolved();
        }
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


    public void IsSolved()
    {
        for (int i = 0; i < 81; i++)
        {
            if (Sudoku[i] == 0) return;
        }
        sudokuSolved = true;
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

    //readonly struct SquareInfo(int x, int row, int column, int quadrant);

    public void PrintSudoku()
    {
        for (int i = 0; i < 81; i++)
        {
            Console.Write(Sudoku[i] == 0 ? ". " : Sudoku[i] + " ");
            if ((i + 1) % 9 == 0)
                Console.WriteLine();
        }
    }
}