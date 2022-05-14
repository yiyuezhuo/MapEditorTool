# Map Editor Tool

## Usage

## Benchmark (second)

```
Godot backend:

France: 0.14, 0.15, 0.14
Japan: 0.85, 0.86, 0.87
Tokyo: 0.33, 0.33, 0.33

Godot (HTML5) backend:

France: 2.77, 2.58, 2.74
Japan: 16.78, 16.72, 16.86
Tokyo: 6.22, 6.23, 6.24

ImageSharp bakcend:

France: 0.36, 0.36, 0.36
Japan: 4.68, 4.57, 4.56
Tokyo: 0.88, 0.87, 0.87

ImageSharp (HTML5) backend:

France: 6.37, (In HTML5, ImageSharp will break game after the first run unknown reason)
Japan: 1:32.13
Tokyo: 21.13
```

Maybe `ImageSharp` require some tunnings, but I would rather just remove the `ImageSharp` backend support.

HTML5 is 20x slower in general thougn...