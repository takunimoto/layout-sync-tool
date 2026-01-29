"""Coordinate conversion helpers for Maya exporter."""
from __future__ import annotations
from typing import Iterable, Tuple


def maya_to_unity_position(position: Iterable[float]) -> Tuple[float, float, float]:
    x, y, z = position
    return (x, y, -z)


def maya_to_unity_rotation_euler(rotation: Iterable[float]) -> Tuple[float, float, float]:
    rx, ry, rz = rotation
    return (-rx, -ry, rz)
