﻿/// @file
/// @brief This file contains a simple example for testing an AI thinker in
/// isolation.
///
/// @author Nuno Fachada
/// @date 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;
using Oyzis;

namespace TestOyzis
{
    /// <summary>
    /// This is an example class for testing our Oyzis thinker in isolation.
    /// </summary>
    public static class Program
    {
        private static void Main(string[] args)
        {
            // ////////////////////////////////////////////////////////////// //
            // Create an instance of our Oyzis thinker via ThinkerPrototype. //
            // If we created directly with new, it would not be properly      //
            // configured.                                                    //
            // ////////////////////////////////////////////////////////////// //

            // Create a configuration for a default ColorShapeLinks match
            MatchConfig mc = new MatchConfig();

            // Get the fully qualified name of our basic Oyzis thinker
            string oyzisFullName =  typeof(OyzisThinker).FullName;

            // Create a prototype for our thinker
            ThinkerPrototype tp = new ThinkerPrototype(oyzisFullName, "", mc);

            // Create an instance of our basic Oyzis thinker
            IThinker oyzisThinker = tp.Create();

            // //////////////////////////////////////////////////////// //
            // Create a board so we can test how our thinker will play. //
            // //////////////////////////////////////////////////////// //

            // A cancellation token, will be ignored
            CancellationToken ct = new CancellationToken();

            // Create a ColorShapeLinks board with default size
            Board board = new Board();

            // Show initial board
            Console.WriteLine("\n=== Initial board ===\n");
            ShowBoard(board);

            // Make some moves manually
            board.DoMove(PShape.Round, 0);  // White plays round piece in col 0
            board.DoMove(PShape.Square, 4); // Red plays square piece in col 4
            board.DoMove(PShape.Square, 5); // White plays round piece in col 5

            // Show board after our three manual moves
            Console.WriteLine("\n=== Board after three manual moves ===\n");
            ShowBoard(board);

            // Starts timer
            DateTime startTime = DateTime.Now;

            // What move would Oyzis make at this moment?
            FutureMove oyzisMove = oyzisThinker.Think(board, ct);

            // Show move and time
            Console.WriteLine(string.Format(
                "-> Oyzis will play {0} after {1} ms.", 
                oyzisMove, (DateTime.Now - startTime).TotalMilliseconds));

            // Make the move selected by Oyzis
            board.DoMove(oyzisMove.shape, oyzisMove.column);

            // Show board after Oyzis made its move
            Console.WriteLine("\n=== Board after Oyzis made move ===\n");
            ShowBoard(board);
        }

        // Helper method to show a board
        private static void ShowBoard(Board board)
        {
            for (int r = board.rows - 1; r >= 0; r--)
            {
                for (int c = 0; c < board.cols; c++)
                {
                    char pc = '.';
                    Piece? p = board[r, c];
                    if (p.HasValue)
                    {
                        if (p.Value.Is(PColor.White, PShape.Round))
                        {
                            pc = 'w';
                        }
                        else if (p.Value.Is(PColor.White, PShape.Square))
                        {
                            pc = 'W';
                        }
                        else if (p.Value.Is(PColor.Red, PShape.Round))
                        {
                            pc = 'r';
                        }
                        else if (p.Value.Is(PColor.Red, PShape.Square))
                        {
                            pc = 'R';
                        }
                        else
                        {
                            throw new ArgumentException(
                                $"Invalid piece '{p.Value}'");
                        }
                    }
                    Console.Write(pc);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
