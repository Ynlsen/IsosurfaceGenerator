# Isosurface Generator

This repository contains the source code for a 3D mesh generator built in Godot with C#. The tool creates 3D meshes by implementing various isosurface algorithms such as Marching Cubes.

## Features

* **Marching Cubes**: Generates a mesh by evaluating 3D cells. For each cell, it selects one of 256 possible mesh configurations based on whether the eight corner points are inside or outside the target surface.
* **UI**: All generation parameters can be adjusted in real time via a convenient UI.
* **Density Visualizer**: A highly performant visualizer for displaying the underlying density field using GPU instancing.

## Planned Features

* **More Algorithms**: Implement additional algorithms like Dual Contouring.
* **Performance Optimization**: Explore multithreading or compute shaders to accelerate the generation process for larger grid sizes.
* **Density Distributions**: Add support for different density functions to generate specific shapes, such as planetary spheres or varied terrain types, rather than relying on solely random noise.

## Screenshot
<img width="1704" height="957" alt="Screenshot From 2025-10-01 13-11-37" src="https://github.com/user-attachments/assets/c8d1ceab-3dfa-40bb-ac38-eafc7393ae63" />