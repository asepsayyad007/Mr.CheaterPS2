# Rule: High-Resolution Vector Assets & Professional Iconography

## Scope
Applies to all UI development, graphic assets, icon design, diagrams, and UI components across WPF, Web, and desktop frameworks.

## Directives
1. **Prefer Vector Assets**: Always use resolution-independent vector geometry (XAML `Path` Data / `Geometry`, SVG paths, or vector icons) instead of raster images (PNG, JPG, BMP) or emoji glyphs for icons, logos, and UI symbols.
2. **Scalability & Crispness**: Ensure all icons scale cleanly without pixelation across high-DPI displays (4K, Retina, 125%-200% Windows display scaling).
3. **Resource Dictionaries**: Centralize reusable vector icons in dedicated resource dictionaries (e.g., `Styles/Icons.xaml` in WPF or SVG icon component sets in Web apps).
4. **Professional Aesthetic**: Maintain a cohesive visual language with consistent stroke widths, geometric balance, and theme-aware brush bindings.
