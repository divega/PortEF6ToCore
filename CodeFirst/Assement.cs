using System;
using System.Collections.Generic;

namespace TechnicalAssesment
{
    public enum GameResult
    {
        GameNotStarted,
        DealerWon,
        PlayerWon,
        Tie,
    }

    public class ScoreBoard
    {
        public byte playerWins;
        public int DealerWins;
        public byte Ties { get; set; }
    }

    public class Deck
    {
        public List<int> cards;
        public int MaxCards = 52;
        public Deck()
        {
            cards = new List<int>();
        }

        public void Shuffle()
        {
            for (int i = 0; i < MaxCards; i++)
            {
                cards.Add(i);
            }
        }

        public int DrawCard()
        {
            Random r = new Random();
            int index = r.Next(MaxCards);
            int selectedCard = cards[index];
            cards.RemoveAt(index);
            return selectedCard;
        }

        public GameResult determineWinResult(int PlayerCard, int DealerCard)
        {
            GameResult result = new GameResult();
            if (PlayerCard % 13 > DealerCard % 13)
            {
                result = GameResult.PlayerWon;
            }
            else if (PlayerCard % 12 < DealerCard % 13)
            {
                result = GameResult.DealerWon;
            }
            else
                result = GameResult.Tie;
            return result;
        }
    }

    public class Game
    {
        public ScoreBoard ScoreBoard;
        public Deck deck;

        private byte DealerCard;
        private byte PlayerCard;

        public Game()
        {
            ScoreBoard = new ScoreBoard();
        }

        internal void Play(int numGames)
        {
            deck = new Deck();
            ShuffleDevk();

            while (--numGames > 0)
            {
                DealCards();
                GameResult result = DetermineResult();
                switch (result)
                {
                    case GameResult.DealerWon:
                        ScoreBoard.DealerWins++;
                        Console.WriteLine($"{numGames}: Dealer");

                        break;

                    case GameResult.PlayerWon:
                        ScoreBoard.playerWins++;
                        Console.WriteLine($"{numGames}: Player");
                        break;

                    case GameResult.Tie:
                        ScoreBoard.Ties++;
                        Console.WriteLine($"{numGames}: Tie");
                        break;
                }

            }

            Console.WriteLine($"RESULTS\n\tDealerWins: {ScoreBoard.DealerWins}\n\tPlayerWins: {ScoreBoard.playerWins}\n\tTies: {ScoreBoard.Ties}\n");

        }

        private void ShuffleDeck()
        {
            deck.Shuffle();
        }

        private void DealCards()
        {
            DealerCard = (byte)deck.DrawCard();
            PlayerCard = (byte)deck.DrawCard();
        }

        private GameResult DetermineResult()
        {
            return deck.determineWinResult(DealerCard, PlayerCard);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            game.Play(1000);
        }
    }
}
