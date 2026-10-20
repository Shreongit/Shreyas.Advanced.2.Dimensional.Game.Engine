# 📘 Theory — Texture UV Selection in 2D Rendering

UV coordinates determine which part of a texture is mapped onto a rendered object. Instead of always displaying the entire texture, SA2DGE can use a UV offset and UV size to select a specific region of an image.

A texture uses normalized UV coordinates, where `(0, 0)` represents one corner and `(1, 1)` represents the opposite corner. The exact visual orientation depends on the texture coordinate convention and how the image is uploaded.

For example, if a texture contains four different images arranged in a 2×2 grid, UV selection allows the sprite renderer to display only the top-left, top-right, bottom-left, or bottom-right quadrant without creating separate textures for each image.

## How It Works

The **UV offset** specifies where the selected region begins in the texture, while the **UV size** specifies the width and height of the region to select. The sprite's vertex UV coordinates determine how that region is mapped across the geometry, and the GPU samples the texture using those coordinates during rendering.

For a texture divided into a 2×2 grid, each quadrant occupies half the texture's width and half its height, so the UV size is typically `(0.5, 0.5)`. The UV offset changes according to the selected quadrant.

## Why It Matters in SA2DGE

UV selection changes which part of a texture is displayed without changing the sprite's geometry, position, size, or rotation. This allows the same rectangle to display different regions of a texture by changing its UV coordinates.

In SA2DGE, this functionality provides a foundation for **sprite sheets, animation frames, tile maps, and texture atlases**, where multiple images are stored in a single texture and selected using UV coordinates.
