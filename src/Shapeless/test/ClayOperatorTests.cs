// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Shapeless.Tests;

public class ClayOperatorTests
{
    [Fact]
    public void Equals_ReturnOK()
    {
        var clay1 = Clay.Parse("""{"id":1,"name":"furion"}""");
        var clay2 = Clay.Parse("""{"name":"furion","id":1,}""");
        var clay3 = Clay.Parse("""{"name":"furion","id":1,"age":30}""");

        Assert.True(clay1.Equals(clay1));
        Assert.True(clay1.Equals(clay2));
        Assert.False(clay1.Equals(clay3));

        Assert.True(clay1.Equals((object)clay1));
        Assert.True(clay1.Equals((object)clay2));
        Assert.False(clay1.Equals((object)clay3));

        Assert.False(clay1.Equals(new { }));
    }

    [Fact]
    public void Operator_ReturnOK()
    {
        var clay1 = Clay.Parse("""{"id":1,"name":"furion"}""");
        var clay2 = Clay.Parse("""{"name":"furion","id":1,}""");
        var clay3 = Clay.Parse("""{"name":"furion","id":1,"age":30}""");

        Assert.True(clay1 == clay2);
        Assert.False(clay1 == clay3);
        Assert.True(clay1 != clay3);

        var clay4 = Clay.Parse("[1,2,3,true,false,{},12.3,\"string\",null,{\"id\":1,\"name\":\"furion\"}]");
        var clay5 = Clay.Parse("[1,2,3,true,false,{},12.3,\"string\",{\"id\":1,\"name\":\"furion\"},null]");
        Assert.False(clay4 == clay5);

        var clay7 = new Clay.Object { ["id"] = 1, ["name"] = "furion" };
        Assert.True(clay1 == clay7);
    }

    [Fact]
    public void GetHashCode_ReturnOK()
    {
        var clay1 = Clay.Parse("""{"id":1,"name":"furion"}""");
        var clay2 = Clay.Parse("""{"name":"furion","id":1,}""");
        var clay3 = Clay.Parse("""{"name":"furion","id":1,"age":30}""");
        Assert.Equal(clay1.GetHashCode(), clay2.GetHashCode());
        Assert.NotEqual(clay2.GetHashCode(), clay3.GetHashCode());

        var clay4 = Clay.Parse("[1,2,3,true,false,{},12.3,\"string\",null,{\"id\":1,\"name\":\"furion\"}]");
        var clay5 = Clay.Parse("[1,2,3,true,false,{},12.3,\"string\",{\"id\":1,\"name\":\"furion\"},null]");
        Assert.NotEqual(clay4.GetHashCode(), clay5.GetHashCode());

        var clay6 = new Clay.Object { ["id"] = 1, ["name"] = "furion" };
        Assert.Equal(clay1.GetHashCode(), clay6.GetHashCode());
    }

    [Fact]
    public void AreObjectEqual_ReturnOK()
    {
        var clay1 = Clay.Parse("""{"id":1,"name":"furion"}""");
        var clay2 = Clay.Parse("""{"name":"furion","id":1,}""");
        var clay3 = Clay.Parse("""{"name":"furion","id":1,"age":30}""");

        Assert.True(Clay.AreObjectEqual(clay1, clay2));
        Assert.False(Clay.AreObjectEqual(clay2, clay3));
    }

    [Fact]
    public void AreArrayEqual_ReturnOK()
    {
        var clay1 = Clay.Parse("[1,2,3,true,false,{},12.3,\"string\",null,{\"id\":1,\"name\":\"furion\"}]");
        var clay2 = Clay.Parse("[1,2,3,true,false,{},12.3,\"string\",{\"id\":1,\"name\":\"furion\"},null]");

        Assert.True(Clay.AreArrayEqual(clay1,
            Clay.Parse("[1,2,3,true,false,{},12.3,\"string\",null,{\"id\":1,\"name\":\"furion\"}]")));
        Assert.False(Clay.AreArrayEqual(clay1, clay2));
    }
}