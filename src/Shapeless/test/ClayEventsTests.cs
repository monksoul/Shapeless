// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayEventsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var clay = new Clay();
        var events = new string[4];

        clay.Changing += (_, _) =>
        {
            events[0] = nameof(clay.Changing);
        };
        clay.Changed += (_, _) =>
        {
            events[1] = nameof(clay.Changed);
        };
        clay.Removing += (_, _) =>
        {
            events[2] = nameof(clay.Removing);
        };
        clay.Removed += (_, _) =>
        {
            events[3] = nameof(clay.Removed);
        };

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }

    [Fact]
    public void TryInvoke_ReturnOK()
    {
        var clay = new Clay();
        clay.TryInvoke(null, "name");
    }

    [Fact]
    public void SetWithIndex_WithEvents_ReturnOK()
    {
        var clay = new Clay.Array();
        var events = new string[4];

        clay.Changing += (_, _) =>
        {
            events[0] = nameof(clay.Changing);
        };
        clay.Changed += (_, _) =>
        {
            events[1] = nameof(clay.Changed);
        };
        clay.Removing += (_, _) =>
        {
            events[2] = nameof(clay.Removing);
        };
        clay.Removed += (_, _) =>
        {
            events[3] = nameof(clay.Removed);
        };

        clay.Set(^0, "OK");
        clay.Remove(^1);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }

    [Fact]
    public void RemoveWithRange_WithEvents_ReturnOK()
    {
        var clay = new Clay.Array { [0] = 1, [1] = 2, [2] = 3, [3] = 4 };
        var events = new List<string>();

        clay.Removing += (_, _) =>
        {
            events.Add(nameof(clay.Removing));
        };
        clay.Removed += (_, _) =>
        {
            events.Add(nameof(clay.Removed));
        };

        clay.Remove(1..^1);

        Assert.Equal("[1,4]", clay.ToJsonString());
        Assert.Equal(["Removing", "Removed", "Removing", "Removed"], events);
    }

    [Fact]
    public void AddEvent_Invalid_Parameters()
    {
        var clay = new Clay();

        Assert.Throws<ArgumentNullException>(() => clay.AddEvent(null!, (ClayEventHandler)null!));
        Assert.Throws<ArgumentException>(() => clay.AddEvent(string.Empty, (ClayEventHandler)null!));
        Assert.Throws<ArgumentException>(() => clay.AddEvent(" ", (ClayEventHandler)null!));

        Assert.Throws<ArgumentNullException>(() => clay.AddEvent("Changing", (ClayEventHandler)null!));
        var exception =
            Assert.Throws<ArgumentException>(() => clay.AddEvent("Test", new ClayEventHandler((_, _) => { })));
        Assert.Equal("Unknown event name: `Test`. (Parameter 'Test')", exception.Message);

        Assert.Throws<ArgumentException>(() => clay.AddEvent(null!, (Action<dynamic, ClayEventArgs>)null!));
    }

    [Fact]
    public void AddEvent_ReturnOK()
    {
        var clay = new Clay();
        var events = new string[4];

        clay.AddEvent(nameof(Clay.Changing), new ClayEventHandler((_, _) =>
        {
            events[0] = nameof(clay.Changing);
        })).AddEvent(nameof(Clay.Changed), new ClayEventHandler((_, _) =>
        {
            events[1] = nameof(clay.Changed);
        })).AddEvent(nameof(Clay.Removing), new ClayEventHandler((_, _) =>
        {
            events[2] = nameof(clay.Removing);
        })).AddEvent(nameof(Clay.Removed), new ClayEventHandler((_, _) =>
        {
            events[3] = nameof(clay.Removed);
        }));

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }

    [Fact]
    public void AddEvent_WithAction_ReturnOK()
    {
        var clay = new Clay();
        var events = new string[4];

        clay.AddEvent(nameof(Clay.Changing), new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events[0] = nameof(clay.Changing);
        })).AddEvent(nameof(Clay.Changed), new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events[1] = nameof(clay.Changed);
        })).AddEvent(nameof(Clay.Removing), new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events[2] = nameof(clay.Removing);
        })).AddEvent(nameof(Clay.Removed), new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events[3] = nameof(clay.Removed);
        }));

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }

    [Fact]
    public void RemoveEvent_Invalid_Parameters()
    {
        var clay = new Clay();

        Assert.Throws<ArgumentNullException>(() => clay.RemoveEvent(null!, (ClayEventHandler)null!));
        Assert.Throws<ArgumentException>(() => clay.RemoveEvent(string.Empty, (ClayEventHandler)null!));
        Assert.Throws<ArgumentException>(() => clay.RemoveEvent(" ", (ClayEventHandler)null!));

        Assert.Throws<ArgumentNullException>(() => clay.RemoveEvent("Changing", (ClayEventHandler)null!));
        var exception =
            Assert.Throws<ArgumentException>(() => clay.RemoveEvent("Test", new ClayEventHandler((_, _) => { })));
        Assert.Equal("Unknown event name: `Test`. (Parameter 'Test')", exception.Message);

        Assert.Throws<ArgumentException>(() => clay.RemoveEvent(null!, (Action<dynamic, ClayEventArgs>)null!));
    }

    [Fact]
    public void RemoveEvent_ReturnOK()
    {
        var clay = new Clay();
        var events = new List<string>();

        var changingHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Changing));
        });
        var changedHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Changed));
        });
        var removingHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Removing));
        });
        var removedHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Removed));
        });

        clay.AddEvent(nameof(Clay.Changing), changingHandler).AddEvent(nameof(Clay.Changed), changedHandler)
            .AddEvent(nameof(Clay.Removing), removingHandler).AddEvent(nameof(Clay.Removed), removedHandler);

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);

        clay.RemoveEvent(nameof(Clay.Changing), changingHandler).RemoveEvent(nameof(Clay.Changed), changedHandler)
            .RemoveEvent(nameof(Clay.Removing), removingHandler).RemoveEvent(nameof(Clay.Removed), removedHandler);

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }

    [Fact]
    public void RemoveEvent_WithAction_ReturnOK()
    {
        var clay = new Clay();
        var events = new List<string>();

        var changingHandler = new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events.Add(nameof(clay.Changing));
        });
        var changedHandler = new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events.Add(nameof(clay.Changed));
        });
        var removingHandler = new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events.Add(nameof(clay.Removing));
        });
        var removedHandler = new Action<dynamic, ClayEventArgs>((_, _) =>
        {
            events.Add(nameof(clay.Removed));
        });

        clay.AddEvent(nameof(Clay.Changing), changingHandler).AddEvent(nameof(Clay.Changed), changedHandler)
            .AddEvent(nameof(Clay.Removing), removingHandler).AddEvent(nameof(Clay.Removed), removedHandler);

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);

        clay.RemoveEvent(nameof(Clay.Changing), changingHandler).RemoveEvent(nameof(Clay.Changed), changedHandler)
            .RemoveEvent(nameof(Clay.Removing), removingHandler).RemoveEvent(nameof(Clay.Removed), removedHandler);

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }

    [Fact]
    public void ClearAllEvents_ReturnOK()
    {
        var clay = new Clay();
        var events = new List<string>();

        var changingHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Changing));
        });
        var changedHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Changed));
        });
        var removingHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Removing));
        });
        var removedHandler = new ClayEventHandler((_, _) =>
        {
            events.Add(nameof(clay.Removed));
        });

        clay.AddEvent(nameof(Clay.Changing), changingHandler).AddEvent(nameof(Clay.Changed), changedHandler)
            .AddEvent(nameof(Clay.Removing), removingHandler).AddEvent(nameof(Clay.Removed), removedHandler);

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);

        clay.ClearAllEvents();

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }
}