using System.Collections.Generic;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using System;

namespace Oizys
{
    public class OizysThinker : AbstractThinker
    {
        // Maximum search depth.
        private const int maxDepth = 3;

        // Displays AI name and current version as "G09_OIZYS_V(X)".
        public override string ToString() => "G09_OIZYS" + "_V2";

        // Executes a move.
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            (FutureMove move, float score) conclusion = Negamax(
                board, ct, board.Turn, 0, float.NegativeInfinity, 
                float.PositiveInfinity);

            return conclusion.move;
        }

        // Negamax with alpha beta pruning.
        private (FutureMove move, float score) Negamax(
            Board board, CancellationToken ct, PColor turn, int depth, 
            float alpha, float beta)
        {
            (FutureMove move, float score) currentMove;

            Winner winner;

            // In case of cancellation request, skips rest of algorithm.
            if (ct.IsCancellationRequested)
            {
                // Makes no move.
                currentMove = (FutureMove.NoMove, float.NaN);
            }

            // If game has ended, returns final score.
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                // Oizys wins, returns maximum score.
                if (winner.ToPColor() == turn)
                {
                    currentMove = (FutureMove.NoMove, float.PositiveInfinity);
                }

                // Opponent wins, returns minimum score.
                else if(winner.ToPColor() == turn.Other())
                {
                    currentMove = (FutureMove.NoMove, float.NegativeInfinity);
                }

                // A draw board, return 0.
                else
                {
                    currentMove = (FutureMove.NoMove, 0f);
                }
            }

            // If maximum depth has been reached, get heuristic value.
            else if (depth == maxDepth)
            {
                currentMove = (FutureMove.NoMove, Heuristic(board, turn));
            }

            // Board isn't final and maximum depth hasn't been reached.
            else
            {
                // Set up currentMove for future maximizing.
                currentMove = (FutureMove.NoMove, float.NegativeInfinity);

                // Iterate each column.
                for (int c = 0; c < Cols; c++)
                {
                    // If column is full, skip to next column.
                    if (board.IsColumnFull(c)) continue;

                    // Try both shapes.
                    for (int s = 0; s < 2; s++)
                    {
                        // Store current shape.
                        PShape shape = (PShape)s;

                        // If player doesn't have this piece, skip.
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move.
                        board.DoMove(shape, c);

                        // Call Negamax and register board's score.
                        float score = -Negamax(
                            board, ct, turn.Other(), depth +1, -beta, 
                            -alpha).score;

                        // Undo move.
                        board.UndoMove();

                        // If this move has the best score yet, keep it.
                        if (score > currentMove.score)
                        {
                            currentMove = (new FutureMove(c, shape), score);
                        }

                        // Update alpha value.
                        if (score > alpha)
                        {
                            alpha = score;
                        }

                        // Beta pruning.
                        if (alpha >= beta)
                        {
                            return currentMove;
                        }
                    }
                }
            }

            // Returns move and its value.
            return currentMove;
        }

        // Heuristic that iterates win corridors and changes its score 
        // based on the pieces in each of those corridors.
        private float Heuristic(Board board, PColor turn)
        {
            // Heuristic score.
            float score = 0;

            // Iterate every win corridor in the board.
            foreach (IEnumerable<Pos> corridor in board.winCorridors)
            {
                // Iterate every position in the corridor.
                foreach (Pos pos in corridor)
                {
                    // Try to get piece in current board position.
                    Piece? piece = board[pos.row, pos.col];

                    // Check if there is a piece.
                    if (piece.HasValue)
                    {
                        // If its the same color as our AI's,
                        if (piece.Value.color == turn)
                        {
                            // Add 1 point.
                            score += 1;
                        }
                        
                        // If not,
                        else 
                        {
                            // Remove 1 point
                            score -= 1;
                        }

                        // If its the same shape as our AI's,
                        if (piece.Value.shape == turn.Shape())
                        {
                            // Add 2 points, because shape > color.
                            score += 2;
                        }
                        
                        // If not,
                        else 
                        {
                            // Remove 2 points.
                            score -= 2;
                        }
                    }
                }
            }

            // Return the final heuristic score.
            return score;
        }
    }
}
