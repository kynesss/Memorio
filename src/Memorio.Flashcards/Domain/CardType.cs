using System.Text.Json.Serialization;

namespace Memorio.Flashcards.Domain;

[JsonConverter(typeof(JsonStringEnumConverter<CardType>))]
public enum CardType
{
    Basic,
    BasicReversed,
    Cloze
}
