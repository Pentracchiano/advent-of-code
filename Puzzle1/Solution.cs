 class Calories : PuzzleSolution {
    private PriorityQueue<int, int> best_three = new PriorityQueue<int, int>();

    public void Setup(string? input) {
      int elf = 0;
      foreach (string entry in input!.Split('\n')) {
         if (entry == "") {
            if(this.best_three.Count >= 3) {
               this.best_three.EnqueueDequeue(elf, elf);
            } else {
               this.best_three.Enqueue(elf, elf);
            }

            elf = 0;
            continue;
         }
         elf += int.Parse(entry);
      }
    }

    public string SolvePartOne(string? input) {
        return this.best_three.Peek().ToString();
    }

    public string SolvePartTwo(string? input) {
        return this.best_three.UnorderedItems.Sum(e => e.Element).ToString();
    }
 }   
