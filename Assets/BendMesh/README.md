Bending a mesh with a shader in Unity
=====================================
![gif](https://i.imgur.com/ndl30QA.gif)

This is a simple shader for Unity to bend a mesh with a sine function.
Useful when you want to simulate the bending caused due to a force being applied to an object.
The sine function is being plotted along the Z axis, if you want to bend along the X axis edit line 36:

```sh
float4 result = (float4(0.0 , ( sin( ( _OffsetSin + ( vertexPos.x * _Frequency ) ) ) * _Amplitude ) , 0.0 , 0.0));
```

The more vertices your mesh has along the axis you want to bend the smoother it will look.
Tested with Unity 2018.2