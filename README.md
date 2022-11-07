# Overview

## Overall Structure

### Adding Methods & Properties

#### Adding a Propery

Register new properties in the `SetupProperties()` function thats in `DahlDesignPlugin.c`.  This uses a simplified technique for managing properties.

#### Adding computation for a Property

Presumably you dont want to just add a property, you want to update it based on calculations.

Simply add a new method in `Calcs.cs` follwing the existing examples.  Make sure to decorate it with an attribute that defines how often it should run.  See the example below.

Also note that there are a number of information sources that will be available to all functions by default, and are updated on every execution of the `DataUpdate` function for you.  These include:

* `data`: The current `GameData` object that SimHub gives us.
* `irData`: The raw `DataSampleEx` from iRacing telemtry from the same cycle.
* `prevData`: This indexed `List` contains the last 10 `GameData` objects, so you can read backwards in time for comparing values over time.  It contains the 10 most recent snapshots at all times.
* `properties`: A simplified method for updating & creating. SimHub properties.

Do your calculations, and update the propert with `properties.SetProperyValue()`.

Example method definition:

```c#
        [TargetHz(1)]
        public void DoSomeUpdates()
        {
            var MyValue = irData.Telemetry.RandomValue;
            properties.SetPropertyValue("Random", MyValue);

        }
```

### Execution Structure

This plugin handles execution strategies a little differently that most to accomodate all the varying rates at which different update functions need to run.

As a result, it uses a technique called [`Reflection`](https://en.wikipedia.org/wiki/Reflective_programming) that allows an application to inspect *itself*.

The Calcs class inspects **itself**, looking for `public` methods that have a `CustomAttribute` called `TargetHzAttribute`.  Any method found that matches both these conditions will be included into a list of methods called `IntervalMethodMapping`, grouped (indexed) by how often (`Hz`).

Then the core `DataUpdate` method in `DahlDesignPlugin.cs` iterates through each method in the list on every call, and only executes the ones that are 'on deck' in this execution cycle.  It does this by running `Invoke` on the `MethodInfo` stored in `IntervalMethodMapping`.

As a result, we end up with a system that keeps `DataUpdate` small and reasonable, but allows for effectively unlimited numbers of functions to be managed easily, with no changes required to the core scheduling system.

**Caveats**: Caveat to this method is that it is currently not possible to pass arguments to the methods.  This is probably not an issue, as the methods all have the core stuff they need as part of the instance variables.  However, if its needed, it could be added.

**Future**: This model allows for a future where update methods are not simply called sequentially, but instead can be run in separate threads to `DataUpdate` even faster.