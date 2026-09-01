# Tailwind.Avalonia

Tailwind-style utility classes for Avalonia, applied via `tw:Tw.Class="..."` on any `AvaloniaObject`.

## Utility coverage

Tracks the [Tailwind CSS v4.3](https://tailwindcss.com/docs) documentation's utility sections. Checked = implemented in this library.

### Layout

| Utility                     | Implemented |
| --------------------------- | :---------: |
| aspect-ratio                |             |
| columns                     |             |
| break-after                 |             |
| break-before                |             |
| break-inside                |             |
| box-decoration-break        |             |
| box-sizing                  |             |
| display                     |             |
| float                       |             |
| clear                       |             |
| isolation                   |             |
| object-fit                  |             |
| object-position             |             |
| overflow                    |             |
| overscroll-behavior         |             |
| position                    |             |
| top / right / bottom / left |             |
| visibility                  |             |
| z-index                     |             |

### Flexbox & Grid

| Utility               | Implemented |
| --------------------- | :---------: |
| flex-basis            |             |
| flex-direction        |             |
| flex-wrap             |             |
| flex                  |             |
| flex-grow             |             |
| flex-shrink           |             |
| order                 |             |
| grid-template-columns |             |
| grid-column           |             |
| grid-template-rows    |             |
| grid-row              |             |
| grid-auto-flow        |             |
| grid-auto-columns     |             |
| grid-auto-rows        |             |
| gap                   |             |
| justify-content       |             |
| justify-items         |             |
| justify-self          |             |
| align-content         |             |
| align-items           |             |
| align-self            |             |
| place-content         |             |
| place-items           |             |
| place-self            |             |

### Spacing

| Utility | Implemented |
| ------- | :---------: |
| padding |     ✅      |
| margin  |     ✅      |

### Sizing

| Utility         | Implemented |
| --------------- | :---------: |
| width           |     ✅      |
| min-width       |     ✅      |
| max-width       |     ✅      |
| height          |     ✅      |
| min-height      |     ✅      |
| max-height      |     ✅      |
| inline-size     |             |
| min-inline-size |             |
| max-inline-size |             |
| block-size      |             |
| min-block-size  |             |
| max-block-size  |             |

### Typography

| Utility                   | Implemented |
| ------------------------- | :---------: |
| font-family               |             |
| font-size                 |     ✅      |
| font-smoothing            |             |
| font-style                |             |
| font-weight               |             |
| font-stretch              |             |
| font-variant-numeric      |             |
| font-feature-settings     |             |
| letter-spacing            |             |
| line-clamp                |             |
| line-height               |             |
| list-style-image          |             |
| list-style-position       |             |
| list-style-type           |             |
| text-align                |             |
| color                     |     ✅      |
| text-decoration-line      |             |
| text-decoration-color     |             |
| text-decoration-style     |             |
| text-decoration-thickness |             |
| text-underline-offset     |             |
| text-transform            |             |
| text-overflow             |             |
| text-wrap                 |             |
| text-indent               |             |
| tab-size                  |             |
| vertical-align            |             |
| white-space               |             |
| word-break                |             |
| overflow-wrap             |             |
| hyphens                   |             |
| content                   |             |

### Backgrounds

| Utility               | Implemented |
| --------------------- | :---------: |
| background-attachment |             |
| background-clip       |             |
| background-color      |     ✅      |
| background-image      |             |
| background-origin     |             |
| background-position   |             |
| background-repeat     |             |
| background-size       |             |

### Borders

| Utility        | Implemented |
| -------------- | :---------: |
| border-radius  |     ✅      |
| border-width   |     ✅      |
| border-color   |     ✅      |
| border-style   |             |
| outline-width  |             |
| outline-color  |             |
| outline-style  |             |
| outline-offset |             |

### Effects

| Utility               | Implemented |
| --------------------- | :---------: |
| box-shadow            |             |
| text-shadow           |             |
| opacity               |     ✅      |
| mix-blend-mode        |             |
| background-blend-mode |             |
| mask-clip             |             |
| mask-composite        |             |
| mask-image            |             |
| mask-mode             |             |
| mask-origin           |             |
| mask-position         |             |
| mask-repeat           |             |
| mask-size             |             |
| mask-type             |             |

### Filters

| Utility                      | Implemented |
| ---------------------------- | :---------: |
| filter (blur)                |             |
| filter (brightness)          |             |
| filter (contrast)            |             |
| filter (drop-shadow)         |             |
| filter (grayscale)           |             |
| filter (hue-rotate)          |             |
| filter (invert)              |             |
| filter (saturate)            |             |
| filter (sepia)               |             |
| backdrop-filter (blur)       |             |
| backdrop-filter (brightness) |             |
| backdrop-filter (contrast)   |             |
| backdrop-filter (grayscale)  |             |
| backdrop-filter (hue-rotate) |             |
| backdrop-filter (invert)     |             |
| backdrop-filter (opacity)    |             |
| backdrop-filter (saturate)   |             |
| backdrop-filter (sepia)      |             |

### Tables

| Utility         | Implemented |
| --------------- | :---------: |
| border-collapse |             |
| border-spacing  |             |
| table-layout    |             |
| caption-side    |             |

### Transitions & Animation

| Utility                    | Implemented |
| -------------------------- | :---------: |
| transition-property        |             |
| transition-behavior        |             |
| transition-duration        |             |
| transition-timing-function |             |
| transition-delay           |             |
| animation                  |             |

### Transforms

| Utility             | Implemented |
| ------------------- | :---------: |
| backface-visibility |             |
| perspective         |             |
| perspective-origin  |             |
| rotate              |             |
| scale               |             |
| skew                |             |
| transform           |             |
| transform-origin    |             |
| transform-style     |             |
| translate           |             |
| zoom                |             |

### Interactivity

| Utility           | Implemented |
| ----------------- | :---------: |
| accent-color      |             |
| appearance        |             |
| caret-color       |             |
| color-scheme      |             |
| cursor            |             |
| field-sizing      |             |
| pointer-events    |             |
| resize            |             |
| scroll-behavior   |             |
| scrollbar-color   |             |
| scrollbar-width   |             |
| scrollbar-gutter  |             |
| scroll-margin     |             |
| scroll-padding    |             |
| scroll-snap-align |             |
| scroll-snap-stop  |             |
| scroll-snap-type  |             |
| touch-action      |             |
| user-select       |             |
| will-change       |             |

### SVG

| Utility      | Implemented |
| ------------ | :---------: |
| fill         |             |
| stroke       |             |
| stroke-width |             |

### Accessibility

| Utility             | Implemented |
| ------------------- | :---------: |
| forced-color-adjust |             |

Additionally, `hover:`, `pressed:`, and `focus:` variants are supported for the color (`bg-`, `text-`, `border-`) and `opacity-*` utilities above — see [CHANGELOG.md](CHANGELOG.md).
