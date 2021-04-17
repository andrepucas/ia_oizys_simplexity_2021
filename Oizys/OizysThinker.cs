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
        public override string ToString() => "G09_OIZYS" + "_V5";

        // Executes a move.
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            (FutureMove move, float score) conclusion = Negamax(
                board, ct, board.Turn, 0, float.NegativeInfinity, 
                float.PositiveInfinity);

            // OnThinkingInfo(string.Format("FINAL Move: {0} has {1} score",conclusion.move, conclusion.score));

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

                        // OnThinkingInfo(string.Format("Move: {0} at col{1} has {2} score",shape, c, score));

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

        // Heuristic that iterates every win corridor and gives each a score 
        // based on its pieces and sequences.
        private float Heuristic(Board board, PColor turn)
        {
            // OnThinkingInfo($"HEURISTIC AS A {turn} {turn.Shape()}");
            
            // Heuristic score.
            float score = 0;

            // Iterate every win corridor in the board.
            foreach (IEnumerable<Pos> corridor in board.winCorridors)
            {
                // Size of current sequence and its board range, 
                // starting at the first element.
                int sequence = 0, range = 0;
                
                // Saves number of empty spaces before a sequence.
                int emptyBehindSeq = 0, emptyRecent = 0;

                // Saves info about the previous piece in the corridor
                bool lastShape = false, lastColor = false;

                // Iterate every position in the corridor.
                foreach (Pos pos in corridor)
                {
                    // Try to get piece in current board position.
                    Piece? piece = board[pos.row, pos.col];

                    // Increase range.
                    range += 1;

                    // If there is a piece in this position.
                    if (piece.HasValue)
                    {
                        // Has same shape and color as player.
                        if (piece.Value.shape == turn.Shape() && 
                            piece.Value.color == turn)
                        {
                            // Add 2 points.
                            score += 2;

                            // If it's within range of a sequence or if there is
                            // no active sequence before it.
                            if (range <= WinSequence && sequence != 0)
                            {
                                // Increment sequence.
                                sequence    += 1;
                            }

                            // If it's out of range or there's no active sequence.
                            else
                            {
                                // Start a new sequence.
                                sequence    = 1;
                                range       = 1;
                            }

                            // Remember this piece.
                            lastShape = true;
                            lastColor = true;
                        }
                        
                        // Has same shape but different color as player.
                        else if (piece.Value.shape == turn.Shape() && 
                                 piece.Value.color != turn)
                        {
                            // Add 1 point.
                            score += 1;

                            // If the previous piece had the same shape and this 
                            // piece is within a sequence's range.
                            if (lastShape && range <= WinSequence)
                            {
                                // Increment sequence.
                                sequence    += 1;
                            }
                            
                            // If not, this is the start of a new sequence.
                            else
                            {
                                // If there was another sequence before.
                                if (sequence != 0)
                                {
                                    // Check for empty positions immediately behind 
                                    // this piece and set it as new empty behind sequence.
                                    emptyBehindSeq = emptyRecent;
                                }
                                
                                // Start a new sequence.
                                sequence    = 1;
                                range       = 1;
                            }
                            
                            // Remember this piece.
                            lastShape = true;
                            lastColor = false;
                        }

                        // Has different shape but same color as player.
                        else if (piece.Value.shape != turn.Shape() && 
                                 piece.Value.color == turn)
                        {
                            // Remove 1 point.
                            score -= 1;

                            // If the previous piece had the same color and this 
                            // piece is within a sequence's range.
                            if (lastColor && range <= WinSequence)
                            {
                                // Increment sequence.
                                sequence    += 1;
                            }
                            
                            // If not, this is the start of a new sequence.
                            else
                            {
                                // If there was another sequence before.
                                if (sequence != 0)
                                {
                                    // Check for empty positions immediately behind 
                                    // this piece and set it as new empty behind ( this sequence).
                                    emptyBehindSeq = emptyRecent;
                                }

                                // Start a new sequence.
                                sequence    = 1;
                                range       = 1;
                            }

                            // Remember this piece.
                            lastShape = false;
                            lastColor = true;
                        }
                        
                        // Has different shape and color as player.
                        else if (piece.Value.shape != turn.Shape() && 
                                 piece.Value.color != turn)
                        {
                            // Remove 2 points.
                            score -= 2;

                            // Reset any possible existing sequences and ranges.
                            sequence    = 0;
                            range       = 0;

                            // Because we no longer have a sequence, we don't
                            // need to remember how many empty pieces were behind it.
                            emptyBehindSeq = 0;

                            // Remember this piece.
                            lastShape = false;
                            lastColor = false;
                        }

                        // We found a piece, so there won't be empty positions 
                        // behind the next piece.
                        emptyRecent = 0;
                    }

                    // If we didn't find a piece.
                    else
                    {
                        // Keep track of this empty position.
                        emptyRecent += 1;

                        // If there isn't an active sequence.
                        if (sequence == 0)
                        {
                            // Keep track of this empty position.
                            emptyBehindSeq += 1;
                        }
                    }

                    // If we're on a sequence higher than 1 and within it's range.
                    if (sequence > 1 && range <= WinSequence)
                    {
                        // Cycles through all possible sequence sizes (2+).
                        for (int i = 2; i <= WinSequence; i++)
                        {
                            // Check for sequence with i size.
                            if (sequence == i)
                            {
                                // If it has equal range, the pieces are after
                                // eachother, which is good, but this should only be valuable
                                // if we have enough space to complete the sequence.
                                // Because we still don't know what will be in the next  
                                // position, we only consider spaces behind the sequence.
                                if (range == i && emptyBehindSeq >= (WinSequence - i))
                                {
                                    // Increase score based on the sequence size.
                                    score += (i * 3);
                                }

                                // If the range isn't equal, we check empty positions. 
                                // Because range < sequence is impossible, we know 
                                // this position has to be after the sequence,
                                // so we can check if the sequence will have any space after.
                                else if (!piece.HasValue)
                                {
                                    // Check if there is enough space ahead of 
                                    // the sequence OR if there is enough combined 
                                    // space before and after the sequence.
                                    if (emptyRecent >= (WinSequence - i) || 
                                        (emptyRecent + emptyBehindSeq) >= (WinSequence - i))
                                    {
                                        // Increase score based on the sequence size.
                                        score += (i * 3);
                                    }
                                }

                                // The final alternative is that we have a sequence
                                // with empty positions in between.
                                else
                                {
                                    // The bigger the distance in between the 
                                    // sequence, the less it will be worth.
                                    score += (i * 3 - range);
                                }
                            }
                        }
                    }
                }
            }

            // Return the final heuristic score.
            return score;
        }
    }
}
