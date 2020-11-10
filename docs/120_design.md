# Design

## IME State

1. Monitor current IME mode every 100 milliseconds.
1. Ignore any errors of getting the state.

## Execution

1. When click the try icon, exit this program.
1. Don't show icon on window changing panel.
1. Don't duplicated running.

## Configuration

Colors are express as six hexadecimal numbers start with '#' symbol.
1. OffColor: Off color. Default: #ff0000
1. OnColor: On color. Default: #00ff00
1. Position: Position of screen. Values: top, bottom, left, right. Default: top
1. SizePx: Width or height pixel size. If position is top or bottom, this is height. Otherwise, this is width.

