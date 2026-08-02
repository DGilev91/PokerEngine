using System.Security.Cryptography;

namespace PokerEngine.Cards;

public sealed class Deck : IDeck
{
    private readonly List<string> _cards;

    public IReadOnlyList<string> Cards => _cards;

    public int RemainingCount => _cards.Count;

    public Deck()
    {
        _cards = [.. CardTable.Cards];
        Shuffle();
    }

    public void Shuffle()
    {
        for (int index = _cards.Count - 1;
             index > 0;
             index--)
        {
            int randomIndex =
                RandomNumberGenerator.GetInt32(index + 1);

            (_cards[index], _cards[randomIndex]) =
                (_cards[randomIndex], _cards[index]);
        }
    }

    public string Deal()
    {
        if (_cards.Count == 0)
        {
            throw new InvalidOperationException(
                "The deck has no remaining cards.");
        }

        string card = _cards[^1];

        _cards.RemoveAt(
            _cards.Count - 1);

        return card;
    }

    public IReadOnlyList<string> Deal(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                "Card count cannot be negative.");
        }

        if (count > _cards.Count)
        {
            throw new InvalidOperationException(
                $"Not enough cards in the deck. Requested: {count}, remaining: {_cards.Count}.");
        }

        if (count == 0)
        {
            return [];
        }

        var dealtCards =
            new string[count];

        for (int index = 0;
             index < count;
             index++)
        {
            dealtCards[index] = Deal();
        }

        return dealtCards;
    }

    public void Take(string card)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            card);

        if (!_cards.Remove(card))
        {
            throw new InvalidOperationException(
                $"Card {card} is not available in the deck.");
        }
    }

    public void Take(IReadOnlyList<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count == 0)
        {
            return;
        }

        var uniqueCards =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (string card in cards)
        {
            if (string.IsNullOrWhiteSpace(card))
            {
                throw new ArgumentException(
                    "The card list contains an empty value.",
                    nameof(cards));
            }

            if (!uniqueCards.Add(card))
            {
                throw new ArgumentException(
                    $"The card list contains duplicate card {card}.",
                    nameof(cards));
            }

            if (!_cards.Contains(
                    card,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Card {card} is not available in the deck.");
            }
        }

        foreach (string card in cards)
        {
            int cardIndex = _cards.FindIndex(
                existingCard =>
                    string.Equals(
                        existingCard,
                        card,
                        StringComparison.OrdinalIgnoreCase));

            _cards.RemoveAt(cardIndex);
        }
    }
}