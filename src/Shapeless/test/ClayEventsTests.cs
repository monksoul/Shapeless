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

        clay.Changing += (s, e) =>
        {
            events[0] = nameof(clay.Changing);
        };
        clay.Changed += (s, e) =>
        {
            events[1] = nameof(clay.Changed);
        };
        clay.Removing += (s, e) =>
        {
            events[2] = nameof(clay.Removing);
        };
        clay.Removed += (s, e) =>
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

        clay.Changing += (s, e) =>
        {
            events[0] = nameof(clay.Changing);
        };
        clay.Changed += (s, e) =>
        {
            events[1] = nameof(clay.Changed);
        };
        clay.Removing += (s, e) =>
        {
            events[2] = nameof(clay.Removing);
        };
        clay.Removed += (s, e) =>
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

        clay.Removing += (s, e) =>
        {
            events.Add(nameof(clay.Removing));
        };
        clay.Removed += (s, e) =>
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
            Assert.Throws<ArgumentException>(() => clay.AddEvent("Test", new ClayEventHandler((sender, args) => { })));
        Assert.Equal("Unknown event name: `Test`. (Parameter 'Test')", exception.Message);

        Assert.Throws<ArgumentException>(() => clay.AddEvent(null!, (Action<dynamic, ClayEventArgs>)null!));
    }

    [Fact]
    public void AddEvent_ReturnOK()
    {
        var clay = new Clay();
        var events = new string[4];

        clay.AddEvent(nameof(Clay.Changing), new ClayEventHandler((sender, args) =>
        {
            events[0] = nameof(clay.Changing);
        })).AddEvent(nameof(Clay.Changed), new ClayEventHandler((sender, args) =>
        {
            events[1] = nameof(clay.Changed);
        })).AddEvent(nameof(Clay.Removing), new ClayEventHandler((sender, args) =>
        {
            events[2] = nameof(clay.Removing);
        })).AddEvent(nameof(Clay.Removed), new ClayEventHandler((sender, args) =>
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

        clay.AddEvent(nameof(Clay.Changing), new Action<dynamic, ClayEventArgs>((sender, args) =>
        {
            events[0] = nameof(clay.Changing);
        })).AddEvent(nameof(Clay.Changed), new Action<dynamic, ClayEventArgs>((sender, args) =>
        {
            events[1] = nameof(clay.Changed);
        })).AddEvent(nameof(Clay.Removing), new Action<dynamic, ClayEventArgs>((sender, args) =>
        {
            events[2] = nameof(clay.Removing);
        })).AddEvent(nameof(Clay.Removed), new Action<dynamic, ClayEventArgs>((sender, args) =>
        {
            events[3] = nameof(clay.Removed);
        }));

        clay.OnChanging("name");
        clay.OnChanged(0);
        clay.OnRemoving("name");
        clay.OnRemoved(0);

        Assert.Equal(["Changing", "Changed", "Removing", "Removed"], events);
    }
}