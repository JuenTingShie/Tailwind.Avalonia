# Tailwind.Avalonia

Tailwind-style utility classes for Avalonia, applied via `tw:Tw.Class="..."` on any `AvaloniaObject`.

## Utility coverage

Tracks the [Tailwind CSS v4.3](https://tailwindcss.com/docs) documentation's utility sections. Checked = implemented in this library. The Notes column says how a utility maps to Avalonia and which platforms it works on (every implemented utility is built on core Avalonia properties, so desktop, Browser and mobile targets are all supported), or why a utility is not implemented: *No Avalonia equivalent* means the concept does not exist in Avalonia, *Not applicable* means it belongs to web-only layout, and *Feasible* / *Not implemented* mark utilities that could be added later.

### Layout

| Utility                     | Implemented | Notes                                                                                                      |
| --------------------------- | :---------: | ---------------------------------------------------------------------------------------------------------- |
| aspect-ratio                |             | No Avalonia equivalent (no intrinsic-ratio layout property)                                                |
| columns                     |             | No Avalonia equivalent (no CSS multi-column layout)                                                        |
| break-after                 |             | No Avalonia equivalent (no fragmentation)                                                                  |
| break-before                |             | No Avalonia equivalent (no fragmentation)                                                                  |
| break-inside                |             | No Avalonia equivalent (no fragmentation)                                                                  |
| box-decoration-break        |             | No Avalonia equivalent (no fragmentation)                                                                  |
| box-sizing                  |             | Not applicable: Avalonia's layout model has no box-sizing switch                                           |
| display                     |     ✅      | All platforms                                                                                              |
| float                       |             | No Avalonia equivalent                                                                                     |
| clear                       |             | No Avalonia equivalent                                                                                     |
| isolation                   |             | Not applicable: no stacking contexts beyond ZIndex                                                         |
| object-fit                  |     ✅      | Image.Stretch                                                                                              |
| object-position             |             | No Avalonia equivalent for Image                                                                           |
| overflow                    |     ✅      | All platforms                                                                                              |
| overscroll-behavior         |     ✅      | ScrollViewer.IsScrollChainingEnabled                                                                       |
| position                    |             | Partial: top/right/bottom/left position Canvas children; relative/absolute/fixed/sticky have no equivalent |
| top / right / bottom / left |     ✅      | Canvas children only                                                                                       |
| visibility                  |             | Not implemented: IsVisible collapses layout, so invisible cannot be mapped faithfully                      |
| z-index                     |     ✅      | All platforms                                                                                              |

### Flexbox & Grid

| Utility               | Implemented | Notes                                                                            |
| --------------------- | :---------: | -------------------------------------------------------------------------------- |
| flex-basis            |             | No Avalonia equivalent (StackPanel has no flex algorithm; use Grid star sizing)  |
| flex-direction        |     ✅      | StackPanel orientation                                                           |
| flex-wrap             |             | Use WrapPanel; there is no per-element property                                  |
| flex                  |             | No Avalonia equivalent (use Grid star sizing)                                    |
| flex-grow             |             | No Avalonia equivalent (use Grid star sizing)                                    |
| flex-shrink           |             | No Avalonia equivalent                                                           |
| order                 |             | No Avalonia equivalent                                                           |
| grid-template-columns |     ✅      | All platforms                                                                    |
| grid-column           |     ✅      | All platforms                                                                    |
| grid-template-rows    |     ✅      | All platforms                                                                    |
| grid-row              |     ✅      | All platforms                                                                    |
| grid-auto-flow        |             | No Avalonia equivalent (Grid has no implicit tracks)                             |
| grid-auto-columns     |             | No Avalonia equivalent (Grid has no implicit tracks)                             |
| grid-auto-rows        |             | No Avalonia equivalent (Grid has no implicit tracks)                             |
| gap                   |     ✅      | StackPanel, WrapPanel and Grid spacing                                           |
| justify-content       |             | Container-level alignment is not available; use the *-self utilities on children |
| justify-items         |             | Container-level alignment is not available; use the *-self utilities on children |
| justify-self          |     ✅      | All platforms                                                                    |
| align-content         |             | Container-level alignment is not available; use the *-self utilities on children |
| align-items           |             | Container-level alignment is not available; use the *-self utilities on children |
| align-self            |     ✅      | All platforms                                                                    |
| place-content         |             | Container-level alignment is not available; use the *-self utilities on children |
| place-items           |             | Container-level alignment is not available; use the *-self utilities on children |
| place-self            |     ✅      | All platforms                                                                    |

### Spacing

| Utility       | Implemented | Notes                                                                                              |
| ------------- | :---------: | -------------------------------------------------------------------------------------------------- |
| padding       |     ✅      | All platforms; numeric scale and arbitrary values (no p-(--var))                                   |
| margin        |     ✅      | All platforms; m-auto / mx-auto are not supported (use HorizontalAlignment)                        |
| space-between |     ✅      | space-x-* / space-y-* set panel spacing like gap-*; -space-* and space-*-reverse are not supported |

### Sizing

| Utility         | Implemented | Notes                                                                                                               |
| --------------- | :---------: | ------------------------------------------------------------------------------------------------------------------- |
| width           |     ✅      | Numeric scale, arbitrary values, auto and full; no fractions, screen/min/max/fit keywords or named sizes (max-w-md) |
| min-width       |     ✅      | Numeric scale, arbitrary values, auto and full; no fractions, screen/min/max/fit keywords or named sizes (max-w-md) |
| max-width       |     ✅      | Numeric scale, arbitrary values, auto and full; no fractions, screen/min/max/fit keywords or named sizes (max-w-md) |
| height          |     ✅      | Numeric scale, arbitrary values, auto and full; no fractions, screen/min/max/fit keywords or named sizes (max-w-md) |
| min-height      |     ✅      | Numeric scale, arbitrary values, auto and full; no fractions, screen/min/max/fit keywords or named sizes (max-w-md) |
| max-height      |     ✅      | Numeric scale, arbitrary values, auto and full; no fractions, screen/min/max/fit keywords or named sizes (max-w-md) |
| size            |     ✅      | Numeric scale, arbitrary values, auto and full; no fractions, screen/min/max/fit keywords or named sizes (max-w-md) |
| inline-size     |     ✅      | All platforms                                                                                                       |
| min-inline-size |     ✅      | All platforms                                                                                                       |
| max-inline-size |     ✅      | All platforms                                                                                                       |
| block-size      |     ✅      | All platforms                                                                                                       |
| min-block-size  |     ✅      | All platforms                                                                                                       |
| max-block-size  |     ✅      | All platforms                                                                                                       |

### Typography

| Utility                   | Implemented | Notes                                                                                               |
| ------------------------- | :---------: | --------------------------------------------------------------------------------------------------- |
| font-family               |     ✅      | font-sans / serif / mono fallback stacks                                                            |
| font-size                 |     ✅      | All platforms                                                                                       |
| font-smoothing            |             | Not implemented: the text rendering mode is not a public AvaloniaProperty in Avalonia 12            |
| font-style                |     ✅      | All platforms                                                                                       |
| font-weight               |     ✅      | All platforms                                                                                       |
| font-stretch              |     ✅      | All platforms; visible only if the font provides the width variant                                  |
| font-variant-numeric      |     ✅      | All platforms; visible only if the font provides the OpenType feature                               |
| font-feature-settings     |     ✅      | font-features-[tag,tag=0]; visible only if the font provides the feature                            |
| letter-spacing            |     ✅      | All platforms                                                                                       |
| line-clamp                |     ✅      | All platforms                                                                                       |
| line-height               |     ✅      | All platforms                                                                                       |
| list-style-image          |             | No Avalonia equivalent (no list markers)                                                            |
| list-style-position       |             | No Avalonia equivalent (no list markers)                                                            |
| list-style-type           |             | No Avalonia equivalent (no list markers)                                                            |
| text-align                |     ✅      | All platforms                                                                                       |
| color                     |     ✅      | All platforms                                                                                       |
| text-decoration-line      |     ✅      | All platforms                                                                                       |
| text-decoration-color     |     ✅      | All platforms                                                                                       |
| text-decoration-style     |     ✅      | solid / dotted / dashed only; double and wavy have no Avalonia equivalent                           |
| text-decoration-thickness |     ✅      | All platforms                                                                                       |
| text-underline-offset     |     ✅      | All platforms                                                                                       |
| text-transform            |             | No Avalonia equivalent                                                                              |
| text-overflow             |     ✅      | All platforms                                                                                       |
| text-wrap                 |     ✅      | text-wrap / text-nowrap / text-ellipsis / text-clip; text-balance and text-pretty are not supported |
| text-indent               |             | No Avalonia equivalent                                                                              |
| tab-size                  |             | No Avalonia equivalent                                                                              |
| vertical-align            |             | No Avalonia equivalent for inline text                                                              |
| white-space               |             | Partial: whitespace-nowrap / whitespace-normal only (see also text-wrap); pre-* are not supported   |
| word-break                |             | No Avalonia equivalent (only TextWrapping)                                                          |
| overflow-wrap             |             | No Avalonia equivalent (only TextWrapping)                                                          |
| hyphens                   |             | No Avalonia equivalent                                                                              |
| content                   |             | Not applicable: no generated content                                                                |

### Backgrounds

| Utility               | Implemented | Notes                                                                                            |
| --------------------- | :---------: | ------------------------------------------------------------------------------------------------ |
| background-attachment |             | No Avalonia equivalent                                                                           |
| background-clip       |             | No Avalonia equivalent                                                                           |
| background-color      |     ✅      | All platforms                                                                                    |
| background-image      |     ✅      | Gradients (bg-gradient-to-*) and images (bg-[url(avares://...)]); remote URLs are not downloaded |
| background-origin     |             | No Avalonia equivalent: the Background always covers the border box                              |
| background-position   |     ✅      | bg-center / top / bottom / left / right and corners via ImageBrush alignment                     |
| background-repeat     |     ✅      | bg-repeat / bg-no-repeat only (no repeat-x / -y / -round / -space)                               |
| background-size       |     ✅      | bg-cover / bg-contain / bg-auto via ImageBrush.Stretch                                           |

### Borders

| Utility        | Implemented | Notes                                                                              |
| -------------- | :---------: | ---------------------------------------------------------------------------------- |
| border-radius  |     ✅      | All platforms                                                                      |
| border-width   |     ✅      | All platforms                                                                      |
| border-color   |     ✅      | All platforms                                                                      |
| border-style   |             | Wontfix (#58): Avalonia borders are always solid                                   |
| outline-width  |             | Not implemented: no outline primitive; ring-* covers the common use                |
| outline-color  |             | Not implemented: no outline primitive; ring-* covers the common use                |
| outline-style  |             | Not implemented: no outline primitive; ring-* covers the common use                |
| outline-offset |             | Not implemented: no outline primitive; ring-* covers the common use                |
| divide-width   |             | Not implemented: no between-children borders (add borders to the children instead) |
| divide-color   |             | Not implemented: no between-children borders (add borders to the children instead) |
| divide-style   |             | Not implemented: no between-children borders; borders are always solid             |

### Effects

| Utility               | Implemented | Notes                                |
| --------------------- | :---------: | ------------------------------------ |
| box-shadow            |     ✅      | All platforms                        |
| inset-shadow          |     ✅      | All platforms                        |
| ring                  |     ✅      | All platforms                        |
| text-shadow           |             | No Avalonia equivalent               |
| opacity               |     ✅      | All platforms                        |
| mix-blend-mode        |             | No Avalonia equivalent               |
| background-blend-mode |             | No Avalonia equivalent               |
| mask-clip             |             | No Avalonia equivalent for CSS masks |
| mask-composite        |             | No Avalonia equivalent for CSS masks |
| mask-image            |             | No Avalonia equivalent for CSS masks |
| mask-mode             |             | No Avalonia equivalent for CSS masks |
| mask-origin           |             | No Avalonia equivalent for CSS masks |
| mask-position         |             | No Avalonia equivalent for CSS masks |
| mask-repeat           |             | No Avalonia equivalent for CSS masks |
| mask-size             |             | No Avalonia equivalent for CSS masks |
| mask-type             |             | No Avalonia equivalent for CSS masks |

### Filters

| Utility                      | Implemented | Notes                                                            |
| ---------------------------- | :---------: | ---------------------------------------------------------------- |
| filter (blur)                |     ✅      | BlurEffect; verified in the Browser build                        |
| filter (brightness)          |             | No Avalonia equivalent (only blur and drop-shadow effects exist) |
| filter (contrast)            |             | No Avalonia equivalent (only blur and drop-shadow effects exist) |
| filter (drop-shadow)         |     ✅      | DropShadowEffect                                                 |
| filter (grayscale)           |             | No Avalonia equivalent (only blur and drop-shadow effects exist) |
| filter (hue-rotate)          |             | No Avalonia equivalent (only blur and drop-shadow effects exist) |
| filter (invert)              |             | No Avalonia equivalent (only blur and drop-shadow effects exist) |
| filter (saturate)            |             | No Avalonia equivalent (only blur and drop-shadow effects exist) |
| filter (sepia)               |             | No Avalonia equivalent (only blur and drop-shadow effects exist) |
| backdrop-filter (blur)       |             | No Avalonia equivalent                                           |
| backdrop-filter (brightness) |             | No Avalonia equivalent                                           |
| backdrop-filter (contrast)   |             | No Avalonia equivalent                                           |
| backdrop-filter (grayscale)  |             | No Avalonia equivalent                                           |
| backdrop-filter (hue-rotate) |             | No Avalonia equivalent                                           |
| backdrop-filter (invert)     |             | No Avalonia equivalent                                           |
| backdrop-filter (opacity)    |             | No Avalonia equivalent                                           |
| backdrop-filter (saturate)   |             | No Avalonia equivalent                                           |
| backdrop-filter (sepia)      |             | No Avalonia equivalent                                           |

### Tables

| Utility         | Implemented | Notes                           |
| --------------- | :---------: | ------------------------------- |
| border-collapse |             | Not applicable: no table layout |
| border-spacing  |             | Not applicable: no table layout |
| table-layout    |             | Not applicable: no table layout |
| caption-side    |             | Not applicable: no table layout |

### Transitions & Animation

| Utility                    | Implemented | Notes                                                                                                                  |
| -------------------------- | :---------: | ---------------------------------------------------------------------------------------------------------------------- |
| transition-property        |     ✅      | All platforms                                                                                                          |
| transition-behavior        |             | Not applicable: Avalonia transitions have no discrete-behavior mode                                                    |
| transition-duration        |     ✅      | All platforms                                                                                                          |
| transition-timing-function |     ✅      | All platforms                                                                                                          |
| transition-delay           |     ✅      | All platforms                                                                                                          |
| animation                  |     ✅      | animate-spin / ping / pulse / bounce / none, tuned with duration-*, delay-*, ease-*; Avalonia Animation, all platforms |

### Transforms

| Utility             | Implemented | Notes                                                                 |
| ------------------- | :---------: | --------------------------------------------------------------------- |
| backface-visibility |             | No Avalonia equivalent (2D transforms only)                           |
| perspective         |             | No Avalonia equivalent (2D transforms only)                           |
| perspective-origin  |             | No Avalonia equivalent (2D transforms only)                           |
| rotate              |     ✅      | All platforms                                                         |
| scale               |     ✅      | All platforms                                                         |
| skew                |     ✅      | All platforms                                                         |
| transform           |     ✅      | transform-none only; transforms come from rotate/scale/skew/translate |
| transform-origin    |     ✅      | All platforms                                                         |
| transform-style     |             | No Avalonia equivalent (2D transforms only)                           |
| translate           |     ✅      | All platforms                                                         |
| zoom                |             | No Avalonia equivalent (use Viewbox or LayoutTransformControl)        |

### Interactivity

| Utility           | Implemented | Notes                                                                                                  |
| ----------------- | :---------: | ------------------------------------------------------------------------------------------------------ |
| accent-color      |             | No Avalonia equivalent (colors come from theme resources)                                              |
| appearance        |             | No Avalonia equivalent                                                                                 |
| caret-color       |     ✅      | TextBox and SelectableTextBlock                                                                        |
| color-scheme      |     ✅      | RequestedThemeVariant; effective on ThemeVariantScope or Window                                        |
| cursor            |     ✅      | All platforms                                                                                          |
| field-sizing      |             | No Avalonia equivalent                                                                                 |
| pointer-events    |     ✅      | All platforms                                                                                          |
| resize            |             | No Avalonia equivalent                                                                                 |
| scroll-behavior   |             | No Avalonia equivalent                                                                                 |
| scrollbar-color   |             | No Avalonia equivalent (theme-controlled)                                                              |
| scrollbar-width   |             | No Avalonia equivalent (theme-controlled)                                                              |
| scrollbar-gutter  |             | No Avalonia equivalent (theme-controlled)                                                              |
| scroll-margin     |             | No Avalonia equivalent                                                                                 |
| scroll-padding    |             | No Avalonia equivalent                                                                                 |
| scroll-snap-align |     ✅      | snap-start / center / end apply to the whole ScrollViewer, not each child                              |
| scroll-snap-stop  |             | Not implemented: Avalonia sets snap points on the ScrollViewer, not per child, so CSS semantics differ |
| scroll-snap-type  |     ✅      | snap-x / snap-y / snap-both / snap-none; always mandatory; also snaps after wheel/scrollbar scrolling  |
| touch-action      |             | No Avalonia equivalent                                                                                 |
| user-select       |             | No Avalonia equivalent (selection depends on the control type)                                         |
| will-change       |             | Not applicable                                                                                         |

### SVG

| Utility      | Implemented | Notes               |
| ------------ | :---------: | ------------------- |
| fill         |     ✅      | Shape controls only |
| stroke       |     ✅      | Shape controls only |
| stroke-width |     ✅      | Shape controls only |

### Accessibility

| Utility                  | Implemented | Notes                                                                                   |
| ------------------------ | :---------: | --------------------------------------------------------------------------------------- |
| forced-color-adjust      |             | Not applicable                                                                          |
| screen readers (sr-only) |             | Not implemented: no Avalonia equivalent for visually hidden, screen-reader-only content |

Additionally, `hover:`, `pressed:`, `focus:`, `focus-visible:`, `checked:`, `disabled:`, `first:`, `last:`, `odd:`, and `even:` variants are supported for the color (`bg-`, `text-`, `border-`) and `opacity-*` utilities above — see [CHANGELOG.md](CHANGELOG.md).
