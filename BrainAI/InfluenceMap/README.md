Influence map
==========
Influence map based on vector implementation.
Usage of influence map in ai implementation provide easy way to avoid obstacles on the way or prevent colliding with danger objects.

Below is an example of using the `InfluenceMap`. 


```csharp

// Initialize influence map
var influenceMap = new InfluenceMap();

// Add some obstacles that should be avoided but their effect should be on a very close distance
influenceMap.Charges.Add( new InfluenceMap.Charge { Value = -25, Origin = new PointChargeOrigin(new Point(70, 10)), Fading = InfluenceMap.LinearDistanceFading });
influenceMap.Charges.Add( new InfluenceMap.Charge { Value = -25, Origin = new PointChargeOrigin(new Point(30, 50)), Fading = InfluenceMap.LinearDistanceFading });

// Add bonus that should attract bot from everywhere on a map
influenceMap.Charges.Add( new InfluenceMap.Charge { Value = 25, Origin = new PointChargeOrigin(new Point(30, 30)), Fading = InfluenceMap.NoDistanceFading });

// Add map border that is pushing bot away (vectors are just describing perpendicular to the wall and it direction does not metter. Use Value to attract or push away.)
influenceMap.Charges.Add( new InfluenceMap.Charge { Value = -25, Origin = new LineChargeOrigin(new Point(100, 100), new Point(100 + 1, 100)), Fading = InfluenceMap.LinearDistanceFading });
influenceMap.Charges.Add( new InfluenceMap.Charge { Value = -25, Origin = new LineChargeOrigin(new Point(100, 100), new Point(100, 100 + 1)), Fading = InfluenceMap.LinearDistanceFading });
influenceMap.Charges.Add( new InfluenceMap.Charge { Value = -25, Origin = new LineChargeOrigin(new Point(0, 0), new Point(0, 1)), Fading = InfluenceMap.LinearDistanceFading });
influenceMap.Charges.Add( new InfluenceMap.Charge { Value = -25, Origin = new LineChargeOrigin(new Point(0, 0), new Point(1, 0)), Fading = InfluenceMap.LinearDistanceFading });

// calculate the vector
var direction = influenceMap.FindForceDirection( new Point( 3, 4 ) );
```
