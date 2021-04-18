# Oizys - AI for the Simplexity Board Game

This AI was created to take part in the [ColorShapeLinks AI competition].

## Work Division

### Afonso Lage

* Defined base of heuristic scores (+2/+1/-1/-2) for pieces depending on their shape and color.
* Helped discuss further heuristics implementations and problems with sequences.
* Created draft for this report.

### André Santos

* Implemented code.
* Added Negamax with alpha beta pruning.
* Combined results of previous AI versions on an excel sheet for easier analysis.
* Showcased sequence counting problems in heuristics for discussion.
* Tried improving sequence counting heuristics.
* Finished this report.

### Nelson Milheiro

* Suggested first draft of theoretical sequence counting addition to heuristics.
* Helped discuss further heuristics implementations and problems with sequences.

## AI Description

### Search Algorithm

Oyzis AI analyzes the board using Negamax with alpha beta pruning, a variant of Minimax, on a set search depth of 3.

### Heuristics Method

1. Every time the heuristics method is called, `score: 0`.

2. It iterates through all `winCorridors` in the board (an outside variable that contains all possible winning corridors in a list, depending on the board size).

3. Each corridor always start with:
   1. `sequence: 0`.
   2. `range: 0`.

4. It then iterates through every board position that is inside the corridor.

5. Each position always starts with:
   1. Trying to get a piece at that position.
   2. `range: +1`.

6. If we found a piece in this position, we then check if it has either:
   1. Same shape and color:
      1. `score: +2`.
      2. `sequence: +1`.
   2. Same shape but different color:
      1. `score: +1`.
   3. Different shape but same color:
      1. `score: -1`.
   4. Different shape and different color:
      1. `score: -2`.
      2. `range: 0`.
      3. if (*range < WinSequence*) `sequence: 0`.

7. Next, we check if our current `range` == `WinSequence`. If so:
   1. `sequence` == `WinSequence`:
      1. `score: +50`.
   2. `sequence` == `(WinSequence - 1)`:
      1. `score: +10`.
   3. `sequence` == `(WinSequence - 2)`:
      1. `score: +5`.

### Problems found

The use of this heuristic resulted in good tests performance and consequently good results at the daily competition standings, but after some analysis we noticed that most times the sequence counter wouldn't actually work as we had originally intended.

After further analysis we noticed that our `range` was actually quite useless because when we looked for sequences (step 7) we were only looking in positions where `range` == `WinSequence` (4 in a base board), like so:

![BadHeuristic](SupportImages/badHeuristic.jpg)

This meant that in this case, when `range: 4` -> `sequence: 2`. So we only get the score for a sequence of size 2, and in the next position (`range: 5`) we wouldn't even check for a sequence. This problem could get even worse on other examples.

Also, because we were only increasing the `sequence` value on pieces with the same shape and color, this heuristics didn't account for sequences with one shape but 2 colors and vice-versa.

### The solution

After discovering this, we started working on a new version (V5), looking for a way to fix this problem. We improved the heuristics above to also:

* Count empty spaces in a corridor;
* Remember the shape and color of the last piece checked;
* When a "good" piece is found, check if there already was an ongoing sequence within range:
  * If yes: `sequence: +1`.
  * If not: `sequence: 1`, `range: 1`.
* When a "bad" piece is found:
  * `sequence: 0`, `range: 0`.
* When checking sequences:
   * `score` is only given if there is enough space to complete the sequence.
   * Bigger sequences get more `score`.
   * Sequences together get more `score` than sequences separated by empty spaces.

For more detailed information on this heuristic, visit branch [V5] of this repository.

## References

* [TicTacToe Minimax], by [Nuno Fachada].

## Metadata

* Students: [Afonso Lage (a21901381)], [André Santos (a21901767)], [Nelson Milheiro (a21904365)]

* Professor: [Nuno Fachada]

* Subject: Artificial Intelligence

* Second year Videogames students @ [Universidade Lusófona de Humanidades e Tecnologias][ULHT]

[ColorShapeLinks AI competition]:https://github.com/VideojogosLusofona/color-shape-links-ai-competition
[V5]:https://github.com/andrepucas/ia_oizys_simplexity_2021/tree/V5
[TicTacToe Minimax]:https://github.com/fakenmc/AIUnityExamples

[Afonso Lage (a21901381)]:https://github.com/AfonsoLage-boop
[André Santos (a21901767)]:https://github.com/andrepucas
[Nelson Milheiro (a21904365)]:https://github.com/Mikapuccino
[Nuno Fachada]:https://github.com/fakenmc
[ULHT]:https://www.ulusofona.pt/
