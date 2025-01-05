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

        clay.ValueChanging += (s, e) =>
        {
            events[0] = nameof(clay.ValueChanging);
        };
        clay.ValueChanged += (s, e) =>
        {
            events[1] = nameof(clay.ValueChanged);
        };
        clay.IndexRemoving += (s, e) =>
        {
            events[2] = nameof(clay.IndexRemoving);
        };
        clay.IndexRemoved += (s, e) =>
        {
            events[3] = nameof(clay.IndexRemoved);
        };

        clay.OnValueChanging("name");
        clay.OnValueChanged(0);
        clay.OnIndexRemoving("name");
        clay.OnIndexRemoved(0);

        Assert.Equal(["ValueChanging", "ValueChanged", "IndexRemoving", "IndexRemoved"], events);
    }
}