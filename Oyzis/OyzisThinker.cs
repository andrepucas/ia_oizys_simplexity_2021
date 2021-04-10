/// @file
/// @brief This file contains the ColorShapeLinks AI presented in the video
/// tutorial at https://youtu.be/ELrsLzX3qBY.
///
/// @author Nuno Fachada
/// @date 2020, 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using System;

namespace Oyzis
{
    /// <summary>
    /// Very basic and minimally functional AI for ColorShapeLinks based on the
    /// tutorial video at https://youtu.be/ELrsLzX3qBY.
    /// </summary>
    public class OyzisThinker : AbstractThinker
    {
        private List<FutureMove> possibleMoves;
        private List<FutureMove> nonLosingMoves;
        private Random random;

        public override void Setup(string str)
        {
            possibleMoves = new List<FutureMove>();
            nonLosingMoves = new List<FutureMove>();
            random = new Random();
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            Winner winner;
            PColor colorOfOurAI = board.Turn;

            possibleMoves.Clear();
            nonLosingMoves.Clear();

            for (int col = 0; col < Cols; col++)
            {
                if (board.IsColumnFull(col)) continue;

                for (int shp = 0; shp < 2; shp++)
                {
                    PShape shape = (PShape)shp;

                    if (board.PieceCount(colorOfOurAI, shape) == 0) continue;

                    possibleMoves.Add(new FutureMove(col, shape));

                    board.DoMove(shape, col);

                    winner = board.CheckWinner();

                    // immediately
                    board.UndoMove();

                    if (winner.ToPColor() == colorOfOurAI)
                    {
                        return new FutureMove(col, shape);
                    }
                    else if (winner.ToPColor() != colorOfOurAI.Other())
                    {
                        nonLosingMoves.Add(new FutureMove(col, shape));
                    }
                }
            }

            if (nonLosingMoves.Count > 0)
                return nonLosingMoves[random.Next(nonLosingMoves.Count)];

            return possibleMoves[random.Next(possibleMoves.Count)];

        }

        public override string ToString() => "G09_" + base.ToString() + "_V0";
    }
}
