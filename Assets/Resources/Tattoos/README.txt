Tattoo Images Folder
====================

Place your tattoo design images in this folder to make them available in the AR system.

Supported formats:
- PNG (recommended for transparency)
- JPG/JPEG
- TGA

Image requirements:
- Recommended resolution: 512x512 to 2048x2048 pixels
- Use transparent background (PNG) for best results
- Square aspect ratio works best
- File naming: Use descriptive names without spaces (e.g., tribal_dragon.png)

How to add tattoos:
1. Copy your tattoo images to this folder
2. In Unity, the images will be automatically imported
3. Select each image and in the Inspector:
   - Set Texture Type to "Sprite (2D and UI)"
   - Set Sprite Mode to "Single"
   - Apply the settings
4. The tattoos will automatically appear in the AR tattoo selection menu

Note: The ARTattooManager will load all sprites from this folder at startup.
