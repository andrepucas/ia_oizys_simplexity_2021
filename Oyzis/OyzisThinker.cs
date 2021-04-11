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
        public override string ToString() => "G09_OYZIS" + "_V0";

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
                // winning score

                // losing score

                // draw score
            }

            // If maximum depth has been reached, get heuristic value.
            else if (depth == maxDepth)
            {
                currentMove = (FutureMove.NoMove, Heuristic(board, turn));
            }

            // Board isn't final and maximum depth hasn't been reached.
            else
            {

            }

            // Returns move and its value.
            return currentMove;
        }

        // Heuristic function
        private float Heuristic(Board board, PColor turn)
        {

        }

    }
}
