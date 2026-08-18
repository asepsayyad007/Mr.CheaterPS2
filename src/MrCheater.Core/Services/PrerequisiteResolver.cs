using MrCheater.Domain.Models;

namespace MrCheater.Core.Services;

public class PrerequisiteResolver
{
    public List<CheatDefinition> ResolveExecutionChain(
        CheatDefinition targetCheat,
        IEnumerable<CheatDefinition> availableCheats,
        bool includeMasterCode = true)
    {
        if (targetCheat == null)
            throw new ArgumentNullException(nameof(targetCheat));

        var cheatMap = availableCheats.ToDictionary(c => c.Id, StringComparer.OrdinalIgnoreCase);
        var nameMap = availableCheats.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        var chain = new List<CheatDefinition>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Traverse(CheatDefinition cheat)
        {
            if (visited.Contains(cheat.Id))
                return; // Cycle prevention

            visited.Add(cheat.Id);

            // 1. If RequiresMasterCode is true and cheat isn't itself a master code, find master code (only if includeMasterCode is true)
            if (cheat.RequiresMasterCode && !IsMasterCode(cheat) && includeMasterCode)
            {
                var masterCheat = availableCheats.FirstOrDefault(IsMasterCode);
                if (masterCheat != null && !visited.Contains(masterCheat.Id))
                {
                    Traverse(masterCheat);
                }
            }

            // 2. Resolve explicit prerequisites
            foreach (var prereqId in cheat.PrerequisiteCheatIds)
            {
                CheatDefinition? prereq = null;
                if (cheatMap.TryGetValue(prereqId, out var p1))
                {
                    prereq = p1;
                }
                else if (nameMap.TryGetValue(prereqId, out var p2))
                {
                    prereq = p2;
                }

                if (prereq != null && !visited.Contains(prereq.Id))
                {
                    // If the prereq is a Master Code and includeMasterCode is false, skip it
                    if (IsMasterCode(prereq) && !includeMasterCode)
                        continue;

                    Traverse(prereq);
                }
            }

            chain.Add(cheat);
        }

        Traverse(targetCheat);
        return chain;
    }

    public static bool IsMasterCode(CheatDefinition cheat)
    {
        return cheat.Id.Equals("master_code", StringComparison.OrdinalIgnoreCase)
               || cheat.Name.Contains("Master", StringComparison.OrdinalIgnoreCase);
    }
}
