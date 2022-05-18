# Map Editor Tool

<img src="https://img.itch.zone/aW1hZ2UvMTUzMDI2Mi84OTU3Mjc1LmdpZg==/347x500/8e8eah.gif">

## Usage

The tool is an editor used to edit area membership and names from a Paradox-style province pixel map:

https://eu4.paradoxwikis.com/Map_modding

For example:

<img src="https://img.itch.zone/aW1nLzg5NDAxOTgucG5n/original/Gp9fIV.png">

The result is a JSON that an area movement mechanism game (like Paradox grand strategy, Axis & Allies, etc) shall consume. The tool comes with 3 premade maps: France Généralité, Japan, and Tokyo and you can use your own image as well. There's a JSON file combined with the France map to replicate the [France Généralité](https://fr.wikipedia.org/wiki/G%C3%A9n%C3%A9ralit%C3%A9_(France)#:~:text=Une%20g%C3%A9n%C3%A9ralit%C3%A9%20est%20une%20circonscription,avec%20l'%C3%A9dit%20de%20Cognac.) map.

Drag using the right mouse button and scroll your wheel to control the camera. Left-click area to set region attributes (membership, id, and name). Press "Edit Side" to modify, add and remove sides.

Export JSON to save your work and import JSON in the future to restore your work (the importing takes advantage of a fuzzy matching method, so tiny map revision will not affect the result ideally). 

Exported JSON and remap texture shall be consumed by a game engine.

## Notes:

HTML5 version processing is 20x slower than desktop in the map initial processing (compute a remap texture, center and neighbor relationship). However, processing of premade map should be done in a few seconds.

The Jam version was submitted to The Tool Jam 2 which lacks a part of export and import functionalities.

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