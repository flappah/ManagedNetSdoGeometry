# ManagedNetSdoGeometry

ManagedNetSdoGeometry is a modified version of [NetSdoGeometry](https://github.com/mapspiral/NetSdoGeometry/) by [mapspiral](https://github.com/mapspiral). It uses the Managed Oracle DataAccess drivers and has been tested with version 21.7.0. 

Usage is similar to the one in NetSdoGeometry. The code snippet below shows how to use it.

```
var reader = command.ExecuteReader();
var geom = (ManagedNetSdoGeometry) reader["geom"];
```

I've added some WKT generation functions that enable string serialization on the retrieved Oracle geometries. The method Get_WKT() generates a string serialized version of the retrieved geometry.
