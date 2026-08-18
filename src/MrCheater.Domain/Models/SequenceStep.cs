using System.Text.Json.Serialization;
using MrCheater.Domain.Enums;

namespace MrCheater.Domain.Models;

public class SequenceStep
{
    [JsonPropertyName("action")]
    public string ActionName { get; set; } = "UP";

    [JsonPropertyName("actionType")]
    public StepActionType ActionType { get; set; } = StepActionType.Tap;

    [JsonPropertyName("holdDurationMs")]
    public int HoldDurationMs { get; set; } = 70;

    [JsonPropertyName("delayAfterMs")]
    public int DelayAfterMs { get; set; } = 70;

    [JsonPropertyName("customKey")]
    public string? CustomKey { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    public SequenceStep Clone()
    {
        return new SequenceStep
        {
            ActionName = ActionName,
            ActionType = ActionType,
            HoldDurationMs = HoldDurationMs,
            DelayAfterMs = DelayAfterMs,
            CustomKey = CustomKey,
            Comment = Comment
        };
    }
}
