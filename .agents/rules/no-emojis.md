# Rule: No Emojis Invariant

## Scope
Applies universally across all code, XAML markup, UI string literals, logs, documentation, comments, commit messages, and assistant responses.

## Directives
1. **Never use emojis**: Do not output or insert unicode emojis (e.g. lightning, rockets, gamepads, charts, checks, crosses, etc.) anywhere in the codebase, UI, or conversation responses.
2. **Text-Based Formatting**: Use clean plain-text indicators (e.g. `[OK]`, `[FAIL]`, `[WARN]`, `[INFO]`, `->`, `*`) instead of graphical emoji glyphs.
3. **UI Elements**: Ensure all WPF buttons, labels, pills, and toast notifications use pure alphanumeric text or vector icons/shapes.
