using PokerEngine.Interfaces;
using System.Security.Cryptography;

namespace PokerEngine.Cards;

public sealed class Deck : IDeck
{
    private static readonly char[] Ranks =
    [
        '2', '3', '4', '5', '6', '7',
        '8', '9', 'T', 'J', 'Q', 'K', 'A'
    ];

    private static readonly char[] Suits =
    [
        'c', 'd', 'h', 's'
    ];

    private readonly List<string> _cards;

    public IReadOnlyList<string> Cards => _cards;

    public int RemainingCount => _cards.Count;

    public Deck()
    {
        _cards = CreateCards();
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);

            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public string Deal()
    {
        if (_cards.Count == 0)
        {
            throw new InvalidOperationException(
                "В колоде больше нет карт.");
        }

        string card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);

        return card;
    }

    public IReadOnlyList<string> Deal(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Количество карт не может быть отрицательным.");
        }

        if (count > _cards.Count)
        {
            throw new InvalidOperationException(
                $"Недостаточно карт. Запрошено: {count}, осталось: {_cards.Count}.");
        }

        if (count == 0)
        {
            return [];
        }

        var dealtCards = new List<string>(count);

        for (int i = 0; i < count; i++)
        {
            dealtCards.Add(Deal());
        }

        return dealtCards;
    }

    private static List<string> CreateCards()
    {
        var cards = new List<string>(52);

        foreach (char rank in Ranks)
        {
            foreach (char suit in Suits)
            {
                cards.Add($"{rank}{suit}");
            }
        }

        return cards;
    }
}