using System.Collections.Generic;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using System;

namespace Oyzis
{
    public class OyzisThinker : AbstractThinker
    {
        // Maximum search depth.
        private const int maxDepth = 2;

        // Displays AI name and current version as "G09_OYZIS_V(X)".
        public override string ToString() => "G09_OYZIS" + "_V1";

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
                // Oyzis wins, returns maximum score.
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

        // Heuristic function used by Minimax AI (temp)
        private float Heuristic(Board board, PColor color)
        {
            // Distance between two points
            float Dist(float x1, float y1, float x2, float y2)
            {
                return (float)Math.Sqrt(
                    Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }

            // Determine the center row
            float centerRow = board.rows / 2;
            float centerCol = board.cols / 2;

            // Maximum points a piece can be awarded when it's at the center
            float maxPoints = Dist(centerRow, centerCol, 0, 0);

            // Current heuristic value
            float h = 0;

            // Loop through the board looking for pieces
            for (int i = 0; i < board.rows; i++)
            {
                for (int j = 0; j < board.cols; j++)
                {
                    // Get piece in current board position
                    Piece? piece = board[i, j];

                    // Is there any piece there?
                    if (piece.HasValue)
                    {
                        // If the piece is of our color, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.color == color)
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                        // If the piece is of our shape, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.shape == color.Shape())
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                    }
                }
            }
            // Return the final heuristic score for the given board
            return h;
        }

    }
}
