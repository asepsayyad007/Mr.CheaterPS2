namespace MrCheater.Domain.Models;

public class ExecutionProgress
{
    public string SequenceName { get; set; } = string.Empty;
    public int CurrentStepIndex { get; set; }
    public int TotalSteps { get; set; }
    public string CurrentAction { get; set; } = string.Empty;
    public bool IsPrerequisiteStep { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
    public double ProgressPercentage => TotalSteps > 0 ? (double)CurrentStepIndex / TotalSteps * 100.0 : 0.0;
}

public class ExecutionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalStepsExecuted { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public bool WasCancelled { get; set; }

    public static ExecutionResult Succeeded(string message, int steps, TimeSpan elapsed) =>
        new() { Success = true, Message = message, TotalStepsExecuted = steps, ElapsedTime = elapsed };

    public static ExecutionResult Cancelled(string message, int steps, TimeSpan elapsed) =>
        new() { Success = false, WasCancelled = true, Message = message, TotalStepsExecuted = steps, ElapsedTime = elapsed };

    public static ExecutionResult Failed(string message) =>
        new() { Success = false, Message = message };
}
