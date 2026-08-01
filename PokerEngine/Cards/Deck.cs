using PokerEngine.Interfaces;
using System.Security.Cryptography;

namespace PokerEngine.Cards;

public sealed class Deck : IDeck
{
    private readonly List<string> _cards;

    public IReadOnlyList<string> Cards => _cards;

    public int RemainingCount => _cards.Count;

    public Deck()
    {
        _cards = new List<string>(CardTable.Cards);
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


    public void Take(string card)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(card);

        if (!_cards.Remove(card))
        {
            throw new InvalidOperationException(
                $"Карты {card} нет в колоде.");
        }
    }

    public void Take(IReadOnlyList<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count != cards.Distinct().Count())
        {
            throw new ArgumentException(
                "Список содержит повторяющиеся карты.",
                nameof(cards));
        }

        foreach (string card in cards)
        {
            if (string.IsNullOrWhiteSpace(card))
            {
                throw new ArgumentException(
                    "Список содержит пустую карту.",
                    nameof(cards));
            }

            if (!_cards.Contains(card))
            {
                throw new InvalidOperationException(
                    $"Карты {card} нет в колоде.");
            }
        }

        foreach (string card in cards)
        {
            _cards.Remove(card);
        }
    }
}