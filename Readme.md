# Rx.Net.Plus

ReactiveX gains popularity and is widely used in several platforms and languages.

If **you** have used Rx.Net (System.Reactive), you probably already wished to transform regular variable (like *int*, *bool* ) to become reactive.

What do we mean? 

We would like to:

1. Get a notification whenever the value of a regular variable is changed (**push** model vs **pull** model)
2. Connect variables to observable sources
3. Not receive notification if the data is set with the same value again and again
4. Continue to use these variables as before by **polling**. Compare them to values, convert them...

**Rx.Net.Plus** is a small but smart library which introduces **two** new types for this purpose (+ several extension methods):

| Type             | Purpose                                                      |
| :--------------- | ------------------------------------------------------------ |
| ***RxVar***      | replace basic types (as int, bool...)                        |
| ***RxProperty*** | to replace view-model *properties* (*NotifyPropertyChanged* pattern) |

### RxVar

Let's start with an example:

Suppose you have in your code, some status of a connection named *isConnected*.

It may be defined as following:

```c#
bool IsConnected { get; set; }
```

You may want to react to change of connection status. One way is to poll this status periodically.

ReactiveX introduces the push model using the *Observer* pattern.

One have to define an observable and update its state.

Other have to listen to events related to this observable as following:

```c#
IObservable<bool> IsConnected { get; set; }
....
IsConnected.Subscribe (_isConnected => _isConnected ? doSomething() : doAnotherThing());
```

Some questions arrive:

1. How do we create this observable in a straightforward way?
2. How do we updated the state of the observable?
3. How can I use it as an ordinary variable - by polling its values - `if (IsConnected) doSomething()` ?
4. How can I chain information from another source to update the IsConnected status?

The answer is: **Rx.Net.Plus** !!

With **Rx.Net.Plus**, it super simple to define this kind of variable.

```c#
 var isConnected = false.ToRx(); // Create a reactive variable
```

We can easily update its state.

```c#
isConnected.Value = true;
```

Suppose we have an <u>sequential</u> algorithm, no problem to write this kind of code:

```c#
if (isConnected)		// Note we don't use .Value of isConnected
{
	StartWork();
}
else
{
	StopWork();
}
```

Let say, connectionStatus is of type Observable

```c#
IObservable<bool> connectionStatus;
```

we can write:

```c#
connectionStatus.Subscribe (isConnected);
```

or using our method:

```c#
	isConnected.ListenTo (connectionStatus);
or
	connectionStatus.RedirectTo (isConnected);
or
	connectionStatus.Notify (isConnected);
```

**Rx.Net.Plus** provides full comparison support:

```c#
var anotherRxBool = true.ToRx();

if (isConnected == anotherRxBool)
{
	// Do some action
}
```

With numeric variables (*int*, *double*...)

```c#
var intRx = 10.ToRx();               // Create a rxvar

if (intRx > 5)
{
	// React !!
}
```

RxVar could be used as part of **state machines**, and instead of polling statuses, flags, counters...

Just react upon change in this manner:

```c#
isConnected.When(true).Notify (v => reactToEvent());
```

It could be used to hold information of a system:

```c#
class SystemInfo
{
    RxVar<DateTime> CurrentTime;
    RxVar<bool>		AreHeadphonesConnected;
}
...

    systemInfo.CurrentTime.Notify (dt => DisplayToScreen (dt));
....
   
   DateTime deadlineDate;
   if (systemInfo.CurrentTime > deadlineDate)
   {
       SendNotification();
   }
```

RxVar implements the following interfaces:

```c#
ISubject, IConvertible, IDisposable, IComparable, IEquatable
```

### Distinct mode

By default, **RxVar** propagates its data (on update) only when a new **distinct** value is set.

That means that if some observer is subscribed to RxVar and the same value is assigned to RxVar, it will not be published.

In order to allow every update to be propagated, RxVar provide the following property:

```C#
IsDistinctMode
```

**Note**: *distinct mode* may be changed at any time even after subscription of observers.

### DisposableBaseClass

**Rx.Net.Plus** provides a base class which implements *IDisposable*, called ***DisposableBaseClass***.

*DisposableBaseClass* has a built-in *CancellationToken* used by RxVar and RxProperty to automatically unsubscribe (in case they are used as observers) on disposing.

This is very convenient as there no need to handle IDisposable...when subscribing RxVar to observables.

```c#
// Classic style
IDisposable subscription = observable.Subscribe (rxVar);
subscription.Dispose();

// Thanks to DisposabeBaseClass (used by RxVar)
rxVar.ListenTo (observable);
....
// RxVar is automatically unsubscribed from observable when disposed
```



### Serialization

**RxVar** supports standard serialization (tested for binary, xml, Json).

The following fields are serialized: ***Value, IsDistinct.***

****

### Limitations

As C# **does not** allow overloading of the assignment operator, it is impossible to assign a value directly to a variable.

Therefore the following notation is disallowed:

```c#
var number = 20.3.ToRx();
number = 10; // Not allowed by C#
```

Instead, *RxVar* provides the following syntax:

```c#
number.Value = 10;
number.Set (10);
```



### RxProperty for WPF

**Rx.Net.Plus** also provides means to leverage RxVar to WPF.

The name of class to use for properties is: **RxProperty** and it is directly derived from RxVar.

For instance:

```c#
public class ViewModel :  IPropertyChangedProxy
{
	public RxProperty IsFunctionEnabled { get; } = false.ToRxProperty();
	public RxProperty Counter { get; }
	public RxProperty Message { get; }
}
```

and in XAML:

```XAML
< CheckBox  IsChecked="{Binding IsFunctionEnabled}"/>
< TextBox   Text="{Binding Counter}"/>
< TextBox   Text="{Binding Message}"/>
```

Notes:

1. **Rx.Net.Plus** supports ***TwoWay*** binding mode (where target can update source as for CheckBox).
2. Binding is specified directly to property name (and not to its Value - although it is also allowed).

For *data binding* **of RxProperty** to work as expected,  the view model shall implement a dedicated interface named *IPropertyChangedProxy*.

Assuming the View Model implements *INotifyPropertyChanged*, it shall **also** implements *IPropertyChangedProxy*as following:

```c#
void IPropertyChangedProxy.NotifyPropertyChanged(PropertyChangedEventArgs eventArgs)
{
	OnPropertyChanged (eventArgs);
}
```

Binding shall occur after the view is created and bound to view model.

In *Caliburn* framework, this occurs within the OnViewAttached method as following:

```c#
class ViewModel : Screen, IPropertyChangedProxy
{
    protected override void OnViewAttached(object view, object context)
    {
        base.OnViewAttached(view, context);
        this.BindRxPropertiesToView (view);			// Bind RxProperties to view
    }
}
```

In classic MVVM or other frameworks (like Prism, MVVM Light...), call to BindRxPropertiesToView can be handled in Loaded event handler.

### Extension Methods

| Method                             | Description                                                  | Usage                                                        |
| ---------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ***RedirectTo***                   | an alias of *Subscribe*                                      | `rxVar.RedirectTo(rxVar2)`                                   |
| ***Notify***                       | an alias of *Subscribe*                                      | `rxVar.Notify(rxVar2)`                                       |
| ***When***(value)                  | equivalent to*Where* (v => v.Equals(value))                  | `rxVar.When(true).Notify (rxVar2)`                           |
| ***If*** (value),                  | equivalent to*Where* (v => v.Equals(value))                  | `rxVar.If (10).Notify (rxVar2)`                              |
| ***IfNot*** (value)                | equivalent to*Where* (v => false == v.Equals(value))         | `rxVar.IfNot (5).Notify (rxVar2)`                            |
| ***ToRxVar***                      | creates an *instance* of **RxVar** of the type of **var**    | `var rxVar = 10.ToRxVar()`                                   |
| ***ToRxVarAndSubscribe***          | creates an instance of RxVar of the same type of **observable source** and subscribe the new instance to **source** | `IObservable<int> obs; var rxvar = obs.Where(v => v > 10).ToRxVarAndSubscribe();` |
| ***ToRxProperty***                 | creates an *instance* of **RxProperty** of the type of **var** | `RxProperty<int> Counter => 10.ToRxProperty();`              |
| ***ToRxPropertyAndSubscribe***     | creates an instance of RxProperty of the same type of **observable source** and subscribe the new instance to **source** | `rxvar.ToRxPropertyAndSubscribe();`                          |
| ***BindRxPropertiesToView***       | bind the rxproperties of the view model *vm* (which implements *IPropertyChangedProxy*) to the view. | `this.BindRxPropertiesToView(view)`                          |
| ***True***, ***False***, ***Not*** | pseudo attributes may be used as replacement for *(var == true*, *var == false*) | `isConnected.True,   isConnected.False,  isConnected.Not`    |

### Contact us !

For any comment, suggestion...please contact us at [reactivesw@outlook.com](mailto:reactivesw@outlook.com)

![](Logo.jpg)