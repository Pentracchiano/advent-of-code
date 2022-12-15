# Advent of Code
My solutions for the Advent Of Code 2022. I am trying to learn a brand new language for me, C#. Let's see where it goes.

Structure partly inspired by https://github.com/ShootMe/AdventOfCode/.

## Usage

All of the puzzles inherit from PuzzleSolution, offering a Setup to be initialized with the input and two different Solve methods.

Runner is the main file that runs the entire project: it's configurable from the terminal by optionally passing the day of the puzzle to run:

```powershell
dotnet run [day]
```

If `day` is not provided, it will run all available puzzles. 
A puzzle is run by specifying the title of the problem, the name of the class, and the amount of time elapsed in milliseconds for each 
method run. The two Solve methods also come with a description, which is taken from a method attribute `Description`.

### Example
This is an example with the day `7` specified:
```powershell
PS C:\Users\Davide\Documents\advent-of-code-2022> dotnet run 7
Day 7 specified through command line.

Running Puzzle07: No Space Left On Device.
Setup completed. Elapsed: 6 ms.
Part one. Find all of the directories with a total size of at most 100000. What is the sum of the total sizes of those directories?
Result: "2104783". Elapsed: 4 ms.
Part two. Find the smallest directory that, if deleted, would free up enough space on the filesystem to run the update. What is the total size of that directory?
Result: "5883165". Elapsed: 1 ms.
```

Without specifying a day, the program will just launch all the solutions that are in the directory.

## Implementation

In order to find a puzzle, Runner looks in the current directory for any `Puzzle[day]` subdirectories and then runs the corresponding class by
leveraging the `CodeDOM`. It assumes that the classes are already loaded in the assembly: that is, it will not read the file and then compile it, 
effectively using the name of the directory just to know which classes are loaded, and to discern which `input.txt` file to feed the `Solution` object with. 

### Possible improvements

`Assembly.GetTypes()` could be used in order to enumerate the `PuzzleSolution` classes instead of relying on the directory structure.