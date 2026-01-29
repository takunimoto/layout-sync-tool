# Coordinate Systems

Maya is right-handed. Unity is left-handed (Z-forward).

The exporter converts Maya coordinates to Unity by flipping the Z axis.
For rotations, X and Y are inverted to keep orientation consistent.

Conversion (export side):
- Position: (x, y, z) -> (x, y, -z)
- Rotation: (rx, ry, rz) -> (-rx, -ry, rz)

Adjust these rules if your pipeline differs.
